using UnityEngine;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class SQLConnector : MonoBehaviour {

	[SerializeField] string server = "localhost";
	[SerializeField] string database = "test";
	// string user ="";
	// string pass = "";
	[SerializeField] string defaultUser = "root";
	[SerializeField] string defaultPass = "7144234alqn";
	// Use this for initialization

	public SQLConnector() { if (s_Instance == null) s_Instance = this; }
	public static SQLConnector Instance { get { return s_Instance; } }
	private static SQLConnector s_Instance;

	MySqlConnection connection;

	[SerializeField] string tableName="InfoTable";

	public string TryConnectSQL( string user = "" , string pass = "")
	{
		if (user == "" ) {
			user = defaultUser;
		}
		if (pass == "" ) {
			pass = defaultPass;
		}
		
		string result = "";
		try {
			string connectionString = string.Format("Server={0};port={4};Database={1};User ID = {2};Password={3};",server,database,user,pass,"3306");
			Debug.Log("connect info " + connectionString);
			connection = new MySqlConnection(connectionString);
			connection.Open();
		}catch (Exception e) {
			result = e.Message.ToString();
		}
		return result;
	}

	public List<BookInfo> GetBookListByCommand(string command)
	{
		//the result
		List<BookInfo> res = new List<BookInfo>();

		//init the command
		MySqlCommand dbcmd  = new MySqlCommand( command , connection );


		// execute the command
		// turn the result to the List<BookInfo>
	    using(MySqlDataReader reader = dbcmd.ExecuteReader())
	    {
	    	try
	    	{
			    while(reader.Read())
			    {
			    	BookInfo info = new BookInfo();

				    Debug.Log("field count " + reader.FieldCount);
				    for ( int i = 0 ; i < reader.FieldCount; ++ i )
				    {	
				    	info.AddData(i, reader.GetString(i));
				    }

			    	res.Add(info);
			    }
	    	}
	    	catch(Exception e)
	    	{
	    		 Debug.LogError(e.Message);
	    	}
	    }

		return res;
 	}

	public List<BookInfo> Search(string text)
	{
		Debug.Log("search for " + text );
		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `Title` LIKE '%" +  text + "%' OR"
	    	+ " `Name` LIKE '%" +  text + "%' OR"
	    	+ " `Note` LIKE '%" +  text + "%' OR"
	    	+ " `Pub Place` LIKE '%" +  text + "%' OR"
	    	+ " `Pub Publisher` LIKE '%" +  text + "%' ";

		return EvaluateBookByKeyWord( GetBookListByCommand(command) , text );
	}

	public List<BookInfo> Search(string text, Field field)
	{
		if (field.Equals(Field.TIME))
		{
			return Search(Int32.Parse(text),Global.Field2String(field), 0, 2);
		}
		return Search(text,Global.Field2String(field));
	}

	public List<BookInfo> Search(string text, string field)
	{

		Debug.Log("Called");
		if (text == null)
			text = "";

		Debug.Log(field + " " + Global.ConvertFieldName2DataBase(field) );
		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `" +  Global.ConvertFieldName2DataBase(field)  + "` LIKE '%" +  text + "%'";

	    return GetBookListByCommand(command);
	}

	//////////
	// search a key word in mutiple fields
	//////////

	public List<BookInfo> Search( List<string> texts, Field field)
	{
		if (field.Equals(Field.TIME))
		{
			List<int> times = new List<int>();
			foreach(string time in texts)
			{
				times.Add(Int32.Parse(time));
			}
			return Search(times,Global.Field2String(field), 2);
		}
		return Search(texts,Global.Field2String(field));
	}

	public List<BookInfo> Search( List<string> texts, string field)
	{
		// return null if there is no texts
		if (texts == null || texts.Count <= 0 )
			return null;

		string command = "SELECT * FROM `" + tableName + "` WHERE ";

		foreach(string text in texts)
		{
	    	command +=  " `" + Global.ConvertFieldName2DataBase(field) + "` LIKE '%" +  text + "%' OR";
		}
		command = command.Substring(0, command.Length-2);

	    return GetBookListByCommand(command);
	}	




	public List<BookInfo> Search(List<int> values , string field , int range = 2)
	{   
		string command = "SELECT * FROM `" + tableName + "` WHERE ";

		foreach( int value in values )
		{
	    	command += "( `" + Global.ConvertFieldName2DataBase(field) + "` " + "<=" + (value + range).ToString() + " AND "
	    	+ " `" + Global.ConvertFieldName2DataBase(field) + "` " + ">=" + (value - range).ToString() + ") OR";
		}

		command = command.Substring(0, command.Length-2);

	    return GetBookListByCommand(command);
	}

	public List<BookInfo> Search(int time , string field , int relation = 1 , int range = 2)
	{
		string opea=" = ";
		if (relation < 0) opea = " <= ";
		if (relation > 0) opea = " >= ";

		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `" + Global.ConvertFieldName2DataBase(field) + "` " + opea + time.ToString();

	    if (relation == 0 )
	    {
			command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ "( `" + Global.ConvertFieldName2DataBase(field) + "` " + "<=" + (time + range).ToString() + " AND "
	    	+ " `" + Global.ConvertFieldName2DataBase(field) + "` " + ">=" + (time - range).ToString() + ")";

	    }

	    return GetBookListByCommand(command);
	}

	public List<BookInfo> Search(BookInfo book)
	{
		if (book == null)
			book = new BookInfo();
		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `Name` LIKE '%" +  book.Author + "%' OR"
	    	+ " ( `Time` <= " + (book.Time + 2).ToString() + " AND `Time` >= " + (book.Time - 2).ToString() + ") OR"
	    	+ " `Location` LIKE '%" +  book.Location + "%' ";

		return EvaluateBookByBook( GetBookListByCommand(command) , book );
	}


	public List<BookInfo> Search(List<BookInfo> booklist)
	{
		if (booklist == null || booklist.Count <= 0 )
			return new List<BookInfo>();

		List<List<BookInfo>> bookSave = new List<List<BookInfo>>();
		for(int i = 0 ; i < booklist.Count ; ++i)
		{
			bookSave.Add(Search(booklist[i]));
		}

		return EvaluateBookByBookList(bookSave,booklist);
	}

	public List<BookInfo> SearchByTag(string tag)
	{

		return SearchBySubject(tag);
	}

	public List<BookInfo> SearchBySubject(string key)
	{

		string command = "SELECT * FROM `" + tableName + "` WHERE ";

		foreach(string s in Global.SubjectColumnList)
		{
			command += " `" + s + "` LIKE '%" +  key + "%' OR";
		}

		command = command.Remove(command.Length-3, 2);

		return EvaluateBookByKeyTag( GetBookListByCommand(command) , tag );
	}


	public List<BookInfo> EvaluateBookByKeyWord(List<BookInfo> list , string keyWord)
	{
		foreach(BookInfo b in list)
		{
			b.v = UnityEngine.Random.Range(-0.05f, 0.05f);
			if (b.Title.Contains(keyWord))
				b.v += 3f;
			if (b.Author.Contains(keyWord) || b.Location.Contains(keyWord))
				b.v += 2f;
			if (b.Note.Contains(keyWord))
				b.v += 1f;
		}

		List<BookInfo> res = new List<BookInfo>(list);
		res.Sort((x,y) => x.v.CompareTo(y.v));
		return res;
	}

	public List<BookInfo> EvaluateBookByKeyTag(List<BookInfo> list , string keyWord)
	{
		foreach(BookInfo b in list)
		{
			b.v = UnityEngine.Random.Range(-0.05f, 0.05f);
			if (b.genre.Contains(keyWord))
				b.v += 3f;
			if (b.topical_term.Contains(keyWord))
				b.v += 2f;
			if (b.chronological_subdivision.Contains(keyWord) ||
				b.form_subdivision.Contains(keyWord) ||
				b.general_subdivision.Contains(keyWord) ||
				b.geographic_subdivision.Contains(keyWord))
				b.v += 1f;
		}

		List<BookInfo> res = new List<BookInfo>(list);
		res.Sort((x,y) => x.v.CompareTo(y.v));
		return res;
	}

	public List<BookInfo> EvaluateBookByBook(List<BookInfo> list , BookInfo keyBook)
	{

		foreach(BookInfo b in list)
		{
			b.v = UnityEngine.Random.Range(-0.05f, 0.05f);
			if (b.Author.Contains(keyBook.Author))
				b.v += 3f;
			if (b.Location.Contains(keyBook.Location))
				b.v += 2f;
		}
		List<BookInfo> res = new List<BookInfo>(list);
		res.Sort((x,y) => x.v.CompareTo(y.v));
		return res;
	}

	public List<BookInfo> EvaluateBookByBookList(List<List<BookInfo>> list , List<BookInfo> keyBookList)
	{
		List<BookInfo> res = new List<BookInfo>();

		for(int i = 0 ; i < list.Count && i < keyBookList.Count; ++ i )
		{
			for (int j = 0 ; j < list[i].Count ; ++ j)
			{
				BookInfo newBook = null;
				foreach(BookInfo b in res)
				{
					if (b.Title.Equals(list[i][j]))
					{
						newBook = b;
						break;
					}
				}
				if (newBook == null )
				{
					newBook = list[i][j];
					res.Add(newBook);
				}else
				{
					newBook.v += list[i][j].v + UnityEngine.Random.Range(-0.05f, 0.05f);
				}

			}
		}

		res.Sort((x,y) => x.v.CompareTo(y.v));
		return res;
	}
}
