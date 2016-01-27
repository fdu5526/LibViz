using UnityEngine;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class SQLConnector : MonoBehaviour {

	[SerializeField] string server = "localhost/~atwood";
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
		if (user == "" )
			user = defaultUser;
		if (pass == "" )
			pass = defaultPass;
		try
		{

			string connectionString = string.Format("Server={0};port={4};Database={1};User ID = {2};Password={3};",server,database,user,pass,"3306");
			Debug.Log("connect info " + connectionString);
			connection = new MySqlConnection(connectionString);
			connection.Open();
			return "";
		}catch (Exception e)
		{
			return e.Message.ToString();
	        throw new Exception("Fail to connect the server. Please check if the MySql is opened..." + e.Message.ToString());  
 	
		}

		return "Failed";

		//We pull up a command
	    MySqlCommand dbcmd  = new MySqlCommand("SELECT * FROM `InfoTable`", connection);
		//And we execute it
	    MySqlDataReader reader = dbcmd.ExecuteReader();

	    while(reader.Read())
	    {
	    	Debug.Log("Read " + reader.GetString(0) + "|" + reader.GetString(1) + "|" + reader.GetString(2) + "|" + reader.GetString(3) + "|" + reader.GetString(4));
	    }

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

			    	info.Init(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(4),reader.GetString(3));
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
		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `Title` LIKE '%" +  text + "%' OR"
	    	+ " `Name` LIKE '%" +  text + "%' OR"
	    	+ " `Description` LIKE '%" +  text + "%' OR"
	    	+ " `Publisher` LIKE '%" +  text + "%' OR"
	    	+ " `Location` LIKE '%" +  text + "%' ";


		return EvaluateBookByKeyWord( GetBookListByCommand(command) , text );
	}

	public List<BookInfo> Search(string text, string field)
	{
		if (text == null)
			text = "";
		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `" + field + "` LIKE '%" +  text + "%'";

	    return GetBookListByCommand(command);
	}

	public List<BookInfo> Search(int time , string field , int relation = 1)
	{
		string opea=" = ";
		if (relation < 0) opea = " <= ";
		if (relation > 0) opea = " >= ";

		string command = "SELECT * FROM `" + tableName + "` WHERE " 
	    	+ " `" + field + "` " + opea + time.ToString();

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


	public List<BookInfo> EvaluateBookByKeyWord(List<BookInfo> list , string keyWord)
	{
		foreach(BookInfo b in list)
		{
			b.v = 0;
			if (b.Title.Contains(keyWord))
				b.v += 3f;
			if (b.Author.Contains(keyWord) || b.Location.Contains(keyWord))
				b.v += 2f;
			if (b.Description.Contains(keyWord))
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
			b.v = 0;
			if (b.Author.Contains(keyBook.Author))
				b.v += 3f;
			if (b.Location.Contains(keyBook.Location))
				b.v += 2f;
		}
		List<BookInfo> res = new List<BookInfo>(list);
		res.Sort((x,y) => x.v.CompareTo(y.v));
		return res;
	}
}
