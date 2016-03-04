using System.Runtime.InteropServices;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/*
 * a bunch of global stuff that everyone needs put into the same place
 */
public class Global
{
	public const float TransitionToContextExamTime = 1f;
	public const float ResetToIdleTime = 2.5f;
	public const float StirUpSpeed = 50f;
	public const float SpinSpeed = 10f;
	public const float EnlargeMultipler = 1.5f;

	public static bool FiftyFifty { get { return UnityEngine.Random.value > 0.5f; } }
	
	// find facing angle based on default sprite direction
	public static float Angle (Vector2 defaultDirection, Vector2 direction) {
		float theta = Vector2.Angle(defaultDirection, direction);
		if (defaultDirection == Vector2.down) {
			theta = direction.x < 0f ? -theta : theta;
		} else if (defaultDirection == Vector2.right) {
  		theta = direction.y > 0f ? -theta : theta;
		} else if (defaultDirection == Vector2.up) {
			theta = direction.x > 0f ? -theta : theta;
		} else if (defaultDirection == Vector2.left) {
  		theta = direction.y < 0f ? -theta : theta;
		} 
		return theta;
	}


	// hacky but efficient Fast inverse square root algorithm
	public static float FastSqrt(float z)
	{
		if (z == 0) return 0;
		FloatIntUnion u;
		u.tmp = 0;
		u.f = z;
		u.tmp -= 1 << 23; /* Subtract 2^m. */
		u.tmp >>= 1; /* Divide by 2. */
		u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
		return u.f;
	}
	// C style union what could go wrong?
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int tmp;
	}


	static public int IntFilter(string num)
	{
		string res = Regex.Replace(num, @"[^\d]*", "");
		return int.Parse(res);
	}

	static public string RemoveCharacters (string s) {
		return s.Replace("'", "''");
	}


	static public string Field2String(Field field)
	{
		switch(field)
		{
			case Field.TITLE:
				return "Title";
			case Field.AUTHOR:
				return "Name";
			case Field.TIME:
				return "Time";
			case Field.NOTE:
				return "Note";
			case Field.PUBLISH_LOCATION:
				return "Location";
			case Field.GENRE:
				return "Description"; //TODO no genre in database as of now
				//return "Genre";
			case Field.TOPICAL_TERM:
				return "Topical Term";
			case Field.FORM_SUBDIVISION:
				return "Form Subdivision";
			case Field.GENERAL_SUBDIVISION:
				return "General Subdivision";
			case Field.CHRONOLOGICAL_SUBDIVISION:
				return "Chronological Subdivision";
			case Field.GEOGRAPHIC_SUBDIVISION:
				return "Geographic Subdivision";
			default:
				break;
		}
		return "";
	}

	static public string[] TAG_FIELDS = 
	{
		"Genre",
		"Topical Term",
		"Form Subdivision",
		"General Subdivision",
		"Chronological Subdivision",
		"Geographic Subdivision",

	};

	static public string[] ColunmNameDataBase = 
	{
		"name",
		"title",
		"title_subtitle",
		"title_author",
		"title_remainder",

		"pub_place",
		"pub_publisher",
		"pub_date",

		"note",
		"scope_content",
		"history",
		"language",
		"provenance",
		"binding",
		"pagination",

		"personal_name",
		"corporate_name",

		"topical",
		"topical_geo",
		"topical_heading",
		"topical_date",

		"geo",
		"geo_heading",
		"geo_genre",

		"genre",

		"other_author_personal",

		"other_author_corporate",

		"physical_description",

		"date",
	};

	static public string[] SubjectColumnList =
	{
		"personal_name",
		"corporate_name",
		"topical",
		"topical_geo",
		"topical_heading",
		"topical_date",
		"geo",
		"geo_heading",
		"geo_genre",
		"genre",

	};

	static public string Index2ColunmName(int i )
	{
		if ( i >= 0 && i < ColunmNameDataBase.Length)
			return ColunmNameDataBase[i];
		return "";
	}
}




public class BookInfo{
	public int id;
	public string Title
	{
		get {
			return GetData(Field.TITLE);
		}
	}
	public string Author
	{
		get {
			return GetData(Field.AUTHOR);
		}
	}
	public int Time
	{
		get {
			return int.Parse(GetData(Field.TIME));
		}
	}
	public int TimeFrom
	{
		get {
			if (GetData("date") != null && GetData("date").Length > 11)
				return int.Parse(GetData("date").Substring(7, 4));
			return int.Parse(GetData("date"));
		}
	}
	public int TimeTo
	{
		get {
			if (GetData("date") != null && GetData("date").Length > 15)
				if ( GetData("date").Substring(11, 4).Equals("    ") )
					return -1;
				else
					return int.Parse(GetData("date").Substring(11, 4));
			return int.Parse(GetData("date"));
		}
	}
	public string Note
	{
		get {
			return GetData(Field.NOTE);
		}
	}
	public string Location
	{
		get {
			return GetData(Field.PUBLISH_LOCATION);
		}
	}

	public string genre;
	public string topical_term;
	public string form_subdivision;
	public string general_subdivision;
	public string chronological_subdivision;
	public string geographic_subdivision;

	public float v;

	Dictionary<string,string> dict = new Dictionary<string,string>();


	public void AddData(int i , string value )
	{
		AddData( Global.Index2ColunmName(i),  value);
	}
	public void AddData(string key , string value)
	{
		if ( !dict.ContainsKey(key))
			dict.Add(key, value);
	}

	public string GetData(Field f)
	{
		switch(f)
		{
		case Field.TITLE:
			return GetData("title");
			break;
		case Field.AUTHOR:
			return GetData("name");
			break;
		case Field.TIME:
			return GetData("date");
			break;
		case Field.NOTE:
			return GetData("note");
			break;
		case Field.PUBLISH_LOCATION:
			return GetData("pub_place");
			break;
		case Field.GENRE:
			return GetData("genre");
			break;
		default:
			return "";
			break;
		};
	}

	public List<string> GetSubjects()
	{
		List<string> res = new List<string>();
		for(int i = 0 ; i < Global.SubjectColumnList.Length; ++i)
		{
			res.Add(GetData(Global.SubjectColumnList[i]));
		}
		return res;
	}

	public string GetData(int i)
	{
		return GetData(Global.Index2ColunmName(i));
	}

	public string GetData(string key)
	{
		string res = "";
		dict.TryGetValue( key, out res);
		return res;
	}



	// public void Init(string _title, string _author, int _time , string _location , string _note 
	// 	, string _genre , string _topical_term , string _form_subdivision , string _general_subdivision
	// 	, string _chronological_subdivision , string _geographic_subdivision) {
	// 	Title = Global.RemoveCharacters(_title);
	// 	Author = Global.RemoveCharacters(_author);
	// 	Time = _time;
	// 	Note =  Global.RemoveCharacters(_note);
	// 	Location = Global.RemoveCharacters(_location);

	// 	genre = Global.RemoveCharacters(_genre);
	// 	topical_term = Global.RemoveCharacters(_topical_term);
	// 	form_subdivision = Global.RemoveCharacters(_form_subdivision);
	// 	general_subdivision = Global.RemoveCharacters(_general_subdivision);
	// 	chronological_subdivision = Global.RemoveCharacters(_chronological_subdivision);
	// 	geographic_subdivision = Global.RemoveCharacters(_geographic_subdivision);
	// }

	public BookInfo () {

	}

	public BookInfo(string _title, string _author, int _time , string _location , string _note 
		, string _genre , string _topical_term , string _form_subdivision , string _general_subdivision
		, string _chronological_subdivision , string _geographic_subdivision) {

		dict.Add("title", _title);
		dict.Add("name", _author);
		dict.Add("date", _time.ToString());
		dict.Add("pub_place", _location);
		dict.Add("note", _note);
		dict.Add("genre", _genre);
		dict.Add("topical", _topical_term); 
		dict.Add("geo_genre", _form_subdivision); 
		dict.Add("topical_heading", _general_subdivision); 
		dict.Add("topical_date", _chronological_subdivision); 
		dict.Add("topical_geo", _geographic_subdivision); 
	}

	public string ToString () {
		string s = "\"" + Title + "\"," + 
			   "\"" + Author + "\"," + 
			   "\"" + Time + "\"," + 
			   "\"" + Location + "\"," + 
			   "\"" + Note + "\"," + 
			   "\"" + genre + "\"," + 
			   "\"" + topical_term + "\"," + 
			   "\"" + form_subdivision + "\"," + 
			   "\"" + general_subdivision + "\"," + 
			   "\"" + chronological_subdivision + "\"," + 
			   "\"" + geographic_subdivision + "\"";

		return s;
	}


	public string GetField (Field field) {

		string retVal = null;
		switch (field) {
			case Field.TITLE:
				retVal = this.Title;
				break;
			case Field.AUTHOR:
				retVal = this.Author;
				break;
			case Field.TIME:
				retVal = Time.ToString();
				break;
			case Field.NOTE:
				retVal = this.Note;
				break;
			case Field.PUBLISH_LOCATION:
				retVal = this.Location;
				break;
			case Field.GENRE:
				retVal = this.genre;
				break;
			case Field.TOPICAL_TERM:
				retVal = this.topical_term;
				break;
			case Field.FORM_SUBDIVISION:
				retVal = this.form_subdivision;
				break;
			case Field.GENERAL_SUBDIVISION:
				retVal = this.general_subdivision;
				break;
			case Field.CHRONOLOGICAL_SUBDIVISION:
				retVal = this.chronological_subdivision;
				break;
			case Field.GEOGRAPHIC_SUBDIVISION:
				retVal = this.geographic_subdivision;
				break;
		}

		Debug.Assert(retVal != null);
		return retVal;
	}
}

public enum Field{
	TITLE,
	AUTHOR,
	TIME,
	NOTE,
	PUBLISH_LOCATION,
	GENRE,
	TOPICAL_TERM,
	FORM_SUBDIVISION,
	GENERAL_SUBDIVISION,
	CHRONOLOGICAL_SUBDIVISION,
	GEOGRAPHIC_SUBDIVISION,
}



