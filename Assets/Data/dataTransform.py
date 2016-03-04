import sys

inFilePath = 'ori/new_records_2_01_16.txt'
inFile = open(inFilePath , 'r')

outFilePath = 'sql/data.sql'
outFile = open(outFilePath , 'w') 

DOCUMENT_BOUNDARY = '*** DOCUMENT BOUNDARY ***'

TABLE_NAME = 'InfoTable'

S_NAME = '.100'
S_TITLE = '.245'
S_TIME = '.260'
S_LOCATION = '.260'
S_DESCRIPTION = '.500'


INFO_INDEX = [
		['name'						,'.100','a','tinytext'],

		['title'					,'.245','a','tinytext'], 
		['title_subtitle'			,'.245','b','tinytext'],  
		['title_author'				,'.245','c','tinytext'],  
		['title_remainder'			,'.245','p','tinytext'], 

		['pub_place'				,'.260','a','tinytext'],  
		['pub_publisher'			,'.260','b','tinytext'],   
		['pub_date'					,'.260','c','tinytext'], 

		['note'						,'.500','a','text'],  
		['scope_content'			,'.520','a','text'], 
		['history'					,'.545','a','tinytext'], 
		['language'					,'.546','a','tinytext'],
		['provenance'				,'.561','a','tinytext'],
		['binding'					,'.563','a','tinytext'],
		['pagination'				,'.505','a','tinytext'],


		['personal_name'			,'.600','a','tinytext'],
		['corporate_name'			,'.610','a','tinytext'],

		['topical'					,'.650','a','tinytext'],
		['topical_geo'				,'.650','z','tinytext'],
		['topical_heading'			,'.650','x','tinytext'],
		['topical_date'				,'.650','y','tinytext'],

		['geo'						,'.651','a','tinytext'],
		['geo_heading'				,'.651','x','tinytext'],
		['geo_genre'				,'.651','v','tinytext'],


		['genre'					,'.655','a','tinytext'],

		['other_author_personal'	,'.700','a','tinytext'],

		['other_author_corporate'	,'.710','a','tinytext'],

		['physical_description'		,'.300','a','tinytext'],

		['date'						,'.008','a','tinytext'],
		
		]
SQL_CREATE = []
SQL_INSERT = []
# SQL_CREATE = ['SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";\n',
# 		'SET time_zone = "+00:00";\n',
# 		'-- ----------------------\n',
# 		'CREATE TABLE IF NOT EXISTS `table_name` (\n',
# 		'  `Name` tinytext NOT NULL,\n',
# 		'  `Title` tinytext NOT NULL,\n',
# 		'  `Time` year(4) NOT NULL,\n',
# 		'  `Location` tinytext NOT NULL,\n',
# 		'  `Description` text NOT NULL\n',
# 		') ENGINE=InnoDB DEFAULT CHARSET=latin1;\n',
# 		'-- ----------------------\n']


# # SQL_INSERT = ['INSERT INTO `table_name` (`Name`, `Title`, `Time`, `Location`, `Description`,`Publisher`,`Publish Date`,`Topical Term`,`Form Subdivision`,`General Subdivision`,`Chronological Subdivision`,`Geographic Subdivision`,`Note`) VALUES \n',
# # 		'(\'name\', \'title\', 0000, \'location\', \'description\', \'publisher\', \'publish_date\', \'description\');\n']

# SQL_INSERT = ['INSERT INTO `table_name` (`Name`, `Title`, `Time`, `Location`, `Description`) VALUES \n',
# 		'(\'name\', \'title\', 0000, \'location\', \'description\');\n']


CLEAR_LIST = [' ',';',':','.',',','/','[',']','(',')']

#######
# All the following functions is used for 
# get the information from the info entry
#######

def GetIndexName(index):
	return index[0];

def GetIndexNumber(index):
	return index[1];

def GetIndexSubNumber(index):
	return index[2];

def GetIndexTextType(index):
	if len(index) > 3:
		return index[3];
	return 'not';

def ToFormalName(name):
	words = name.split('_')
	res = ""
	for w in words:
		res += w[0].upper()+w[1:]+' '
	return res[:-1]

def TypeToDefaultValue(t):
	if (t.startswith('year')):
		return '0000'
	return '-'


########
# Used for output
#######

def SetupSQLInstructions():
	global SQL_CREATE
	global SQL_INSERT
	SQL_CREATE = ['SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";\n',
		'SET time_zone = "+00:00";\n',
		'-- ----------------------\n',
		'CREATE TABLE IF NOT EXISTS `table_name` (\n']
	for id, index in enumerate(INFO_INDEX):
		content = '  `FormalName` Type NOT NULL,\n'
		if id >= len(INFO_INDEX) -1:
			content = '  `FormalName` Type NOT NULL\n'
		content = content.replace('FormalName',ToFormalName(GetIndexName(index)))
		content = content.replace('Type',GetIndexTextType(index))
		SQL_CREATE.append(content)
	SQL_CREATE.extend([') ENGINE=InnoDB DEFAULT CHARSET=latin1;\n',
						'-- ----------------------\n'])
	SQL_INSERT = []
	content1 = 'INSERT INTO `table_name` ('
	for index in INFO_INDEX:
		content1 = content1 + '`'+ ToFormalName(GetIndexName(index)) +'`, '
	content1 = content1[:-2] + ') VALUES \n'

	SQL_INSERT.append(content1)
	# SQL_INSERT.append(content2)


class Data:

	def __init__(self):
		self.__DICT__ = dict()

	def GetSubLine(self,line,subFile,t='tinytext'):
		lines = line.split('|')
		for l in lines:
			if l.startswith(subFile):
				res =  l[len(subFile):]
				while res[-1] in CLEAR_LIST:
					res = res[:-1]
				while res[0] in CLEAR_LIST:
					res = res[1:]
				if t.startswith('year'):
					res = ''.join(c for c in res if c.isdigit())
					if len(res) <= 3 :
						res = '0000'
				if '\'' in res:
					# print '[Problem]' + res
					res = res.replace('\'','\'\'')
					# print res
				return res
		return ""

	def ReadLine(self,line):
		for index in INFO_INDEX:
			if line.startswith(GetIndexNumber(index)):
				content = self.GetSubLine(line,GetIndexSubNumber(index),GetIndexTextType(index));
				if content != "":
					# add the entry only if the item does not exsit in the dict
					if GetIndexName(index) in self.__DICT__.keys():
						if (self.__DICT__[GetIndexName(index)]==""):
							self.__DICT__[GetIndexName(index)] = content
					else:
						self.__DICT__[GetIndexName(index)] = content

	def PrintTo(self,out):
		if not('title' in self.__DICT__.keys() ):
			return
		print 'Book [' , self.__DICT__['title'] , ']'
		MY_INSERT = list( SQL_INSERT)
		MY_INSERT[0] = MY_INSERT[0].replace('table_name',TABLE_NAME)

		content2 = '('
		for index in INFO_INDEX:
			if (GetIndexName(index) in self.__DICT__.keys()):
				content2 = content2 + '\''+ self.__DICT__[GetIndexName(index)] + '\', '
			else:
				content2 = content2 + '\''+ TypeToDefaultValue(GetIndexTextType(index)) +'\', '
		content2 = content2[:-2]  + ');\n'

		MY_INSERT.append(content2)
			
		for l in MY_INSERT:
			out.write(l)


class DataAnalyzer:

	def __init__(self):
		self.__DATA__ = []
		self.tempData = None
		SetupSQLInstructions()

	def ReadLine(self,line):
		if line.startswith(DOCUMENT_BOUNDARY):
			self.tempData = Data()
			self.__DATA__.append(self.tempData)
			return True
		elif self.tempData != None:
			self.tempData.ReadLine(line)
		return False

	def PrintTo(self,out):

		print 'print to ' , out.name
		MY_CREATE = list(SQL_CREATE)
		for l in MY_CREATE:
			l = l.replace('table_name',TABLE_NAME)
			out.write(l)

		print 'sql create table' , TABLE_NAME

		print "============================"

		for d in self.__DATA__ :
			d.PrintTo(out)

		print "============================"


if __name__ == '__main__':

	if (len(sys.argv) > 1 ):
		inFilePath = sys.argv[1]

	print " INPUT FILE : " , inFilePath	

	dataAnalyzer = DataAnalyzer()
	i = 0 
	last = ''
	for line in inFile:
		line = line.strip()
		if line.startswith('.') or line.startswith('*'):
			if ( dataAnalyzer.ReadLine(last) ):
				i = i + 1 
			last = ''
		last = last + line + ' '

	dataAnalyzer.ReadLine(last)
 
	print '[Total Books] ' , i 
	dataAnalyzer.PrintTo(outFile)
