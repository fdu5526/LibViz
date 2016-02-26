using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour {

	[SerializeField] SQLConnector connector {get { return SQLConnector.Instance;}}

	int numOfFields = Enum.GetNames(typeof(Field)).Length;


	public void Login() {
		string res = connector.TryConnectSQL();
		if (res == "") {
			print("success!");
		}
		else {
			print("failure...");
		}
	}

	// search by single bookinfo, this is for enlarge view
	public List<WhirlwindBeltInfo> Search(BookInfo inputInfo) {
		Debug.Assert(inputInfo != null);

		List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();
		// search by all fields, get all the results
		for (int i = 0; i < numOfFields; i++) {
			Field f = (Field)i;
			
			// search each field
			List<BookInfo> b = connector.Search(inputInfo.GetField(f), f);
			WhirlwindBeltInfo wwbi = new WhirlwindBeltInfo(b, Global.Field2String(f));
			retVal.Add(wwbi);
		}

		// TODO sort the search results
		retVal.Sort(delegate(WhirlwindBeltInfo b1, WhirlwindBeltInfo b2) { return b1.InfosCount.CompareTo(b2.InfosCount); });

		// return the top 5 fields results (pad if necessary)
		retVal = retVal.GetRange(0, 5);
		return retVal;
	}

	// search via the search bar and "explore" button
	public List<List<BookInfo>> Search(List<BookInfo> inputInfos) {
		Debug.Assert(inputInfos != null);
		Debug.Assert(inputInfos.Count > 0);

		// search by all fields, get all the results
		for (int i = 0; i < numOfFields; i++) {
			Field f = (Field)i;

		}

		// TODO sort the search results

		List<BookInfo> outputInfos = connector.Search(inputInfos);
		
		// return the top 5 fields results
		
		//TODO
		return null;
	}

	public List<BookInfo> AdvancedSearch(int dropdownValue, string searchText)
	{
		List<BookInfo> infos = new List<BookInfo>();
		if (dropdownValue <= 3)
		{
			string field = "";
			if (dropdownValue == 0)
				field = "Title";
			if (dropdownValue == 1)
				field = "Name";
			if (dropdownValue == 2)
				field = "Location";
			if (dropdownValue == 3)
				field = "Description";
			infos = connector.Search(searchText, field );
		}
		if (dropdownValue == 4)
		{
			if (searchText.EndsWith("-"))
				infos = connector.Search( Global.IntFilter(searchText) , "Time" , -1);
			else
			if (searchText.EndsWith("+"))
				infos = connector.Search( Global.IntFilter(searchText) , "Time" , 1);
			else
				infos = connector.Search( Global.IntFilter(searchText) , "Time" , 0);
		}

		return infos;
	}

	public List<BookInfo> SearchTag(string tag)
	{
		return connector.SearchByTag(tag);
	}
}
