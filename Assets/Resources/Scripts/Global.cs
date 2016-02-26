using System.Runtime.InteropServices;
using UnityEngine;
using System.Text.RegularExpressions;

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
			return "Genre";
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
}


public class BookInfo{
	public int id;
	public string Title;
	public string Author;
	public int Time;
	public string Note;
	public string Location;

	public string genre;
	public string topical_term;
	public string form_subdivision;
	public string general_subdivision;
	public string chronological_subdivision;
	public string geographic_subdivision;

	public float v;

	public void Init(string _title, string _author, int _time , string _location , string _note 
		, string _genre , string _topical_term , string _form_subdivision , string _general_subdivision
		, string _chronological_subdivision , string _geographic_subdivision) {
		Title = _title;
		Author = _author;
		Time = _time;
		Note =  _note;
		Location = _location;

		genre = _genre;
		topical_term = _topical_term;
		form_subdivision = _form_subdivision;
		general_subdivision = _general_subdivision;
		chronological_subdivision = _chronological_subdivision;
		geographic_subdivision = _geographic_subdivision;
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


