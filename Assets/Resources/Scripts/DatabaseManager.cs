using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour {

	[SerializeField] SQLConnector connector {get { return SQLConnector.Instance;}}


	public void Login() {
		string res = connector.TryConnectSQL();
		if (res == "") {
			print("success!");
		}
		else {
			print("failure...");
		}
	}

	public List<BookInfo> Search(List<BookInfo> inputInfos)
	{
		Debug.Assert(inputInfos != null);
		Debug.Assert(inputInfos.Count > 0);

		List<BookInfo> outputInfos = connector.Search(inputInfos);
		print(inputInfos.Count + " " + outputInfos.Count);
		return outputInfos;
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
