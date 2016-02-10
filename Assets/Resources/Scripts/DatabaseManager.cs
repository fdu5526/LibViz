using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour {

	[SerializeField] GameObject login;
	[SerializeField] GameObject Navi;
	[SerializeField] GameObject[] fields;
	[SerializeField] GameObject bookInfoBarPrefab;
	[SerializeField] float barHeight = 35f;
	[SerializeField] Image black;

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


	[SerializeField] InputField search;
	[SerializeField] RectTransform searchInfo;
	public void Search(List<BookInfo> inputInfos)
	{
		List<BookInfo> infos = connector.Search(search.text);
	}

	[SerializeField] InputField advanceSearch;
 	[SerializeField] Dropdown advanceDropdown;
	public void AdvancedSearch()
	{
		List<BookInfo> infos = new List<BookInfo>();
		if (advanceDropdown.value <= 3)
		{
			string field = "";
			if (advanceDropdown.value == 0)
				field = "Title";
			if (advanceDropdown.value == 1)
				field = "Name";
			if (advanceDropdown.value == 2)
				field = "Location";
			if (advanceDropdown.value == 3)
				field = "Description";
			infos = connector.Search(advanceSearch.text, field );
		}
		if (advanceDropdown.value == 4)
		{
			if (advanceSearch.text.EndsWith("-"))
				infos = connector.Search( Global.IntFilter( advanceSearch.text) , "Time" , -1);	
			else
			if (advanceSearch.text.EndsWith("+"))
				infos = connector.Search( Global.IntFilter( advanceSearch.text) , "Time" , 1);	
			else
				infos = connector.Search( Global.IntFilter( advanceSearch.text) , "Time" , 0);	
		}
	}

	[SerializeField] InputField bookSearch;
	public void SearchBook()
	{
		List<BookInfo> infos = connector.Search( connector.Search(bookSearch.text,"Title")[0]);
	}


	[SerializeField] InputField tagSearch;
	public void SearchTag()
	{
		List<BookInfo> infos = connector.SearchByTag( tagSearch.text );
	}

	public void RemoveAllChild(Transform root )
	{
		foreach(Transform t in root)
		{
			Destroy(t.gameObject);
		}
	}

	public void CreateBookInfoBar(RectTransform parent, int index , BookInfo info = null)
	{
		GameObject obj = Instantiate(bookInfoBarPrefab) as GameObject;
		RectTransform recTrans = obj.GetComponent<RectTransform>();
		recTrans.parent = parent;
		Vector3 pos = Vector3.zero;
		pos.y = - index * barHeight;
		recTrans.localPosition = pos;

		Text[] texts = obj.GetComponentsInChildren<Text>();
		foreach(Text t in texts)
		{
			t.DOFade(0, 1f).From().SetDelay(0.2f * index);
			if (info != null)
			{
				if (t.name == "Title")
					t.text = info.Title;
				if (t.name == "Author")
					t.text = info.Author;
				if (t.name == "Time")
					t.text = info.Time.ToString();
				if (t.name == "Note")
					t.text = info.Note;
				if (t.name == "Genre")
					t.text = info.genre+'-'+info.topical_term;
				if (t.name == "Location")
					t.text = info.Location;

			}
		}

	}
}
