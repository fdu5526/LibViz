using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class LogicManager : MonoBehaviour {

	[SerializeField] GameObject login;
	[SerializeField] GameObject Navi;
	[SerializeField] GameObject[] fields;
	[SerializeField] GameObject bookInfoBarPrefab;
	[SerializeField] float barHeight = 35f;
	[SerializeField] Image black;

	[SerializeField] SQLConnector connector {get { return SQLConnector.Instance;}}
	// Use this for initialization
	void Awake () {

		login.SetActive(true);
		loginBlack.gameObject.SetActive(false);
		Navi.SetActive(false);
		foreach(GameObject f in fields)
		{
			f.SetActive(false);
		}
	}
	

	[SerializeField] InputField userName;
	[SerializeField] InputField password;
	[SerializeField] Text tips;
	[SerializeField] Image loginBlack;
	public void Login()
	{
		string res = connector.TryConnectSQL(userName.text, password.text);
		if (res == "")
		//login success
		{
			tips.text = "success!";
			loginBlack.gameObject.SetActive(true);
			loginBlack.DOFade(1f, 1.2f).OnComplete(()=>
			{
				Navi.SetActive(true);
				Navi.transform.DOMoveY(1280f, 1f).From();
				Show(fields[0]);
				});

		}
		else
		//error
		{
			tips.text = res;
		}
	}

	public void OnEndUserName()
	{
		password.ActivateInputField();
	}

	public void OnEndPass()
	{
		Login();
	}

	public void Show(GameObject _sobj)
	{
		black.gameObject.SetActive(true);
		black.DOFade(1f, 0.5f).OnComplete(()=>
		{
		foreach(GameObject obj in fields)
			obj.SetActive(false);
		_sobj.SetActive(true);
		black.DOFade( 0, 0.5f).OnComplete(() =>
		{
			black.gameObject.SetActive(false);
			});
		});
	}


	[SerializeField] InputField search;
	[SerializeField] RectTransform searchInfo;
	public void Search()
	{
		List<BookInfo> infos = connector.Search(search.text);

		ShowBookList(infos);
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
				field = "title";
			if (advanceDropdown.value == 1)
				field = "name";
			if (advanceDropdown.value == 2)
				field = "pub_place";
			if (advanceDropdown.value == 3)
				field = "note";
			infos = connector.Search(advanceSearch.text, field );
		}
		if (advanceDropdown.value == 4)
		{
			if (advanceSearch.text.EndsWith("-"))
				infos = connector.Search( Global.IntFilter( advanceSearch.text) , "date" , -1);	
			else
			if (advanceSearch.text.EndsWith("+"))
				infos = connector.Search( Global.IntFilter( advanceSearch.text) , "date" , 1);	
			else
				infos = connector.Search( Global.IntFilter( advanceSearch.text) , "date" , 0);	
		}
		ShowBookList(infos);
	}

	[SerializeField] InputField bookSearch;
	public void SearchBook()
	{
		List<BookInfo> infos = connector.Search( connector.Search(bookSearch.text,"Title")[0]);

		ShowBookList(infos);
	}


	[SerializeField] InputField tagSearch;
	public void SearchTag()
	{
		List<BookInfo> infos = connector.SearchByTag( tagSearch.text );

		ShowBookList(infos);
	}

	public void RemoveAllChild(Transform root )
	{
		foreach(Transform t in root)
		{
			Destroy(t.gameObject);
		}
	}

	public void ShowBookList(List<BookInfo> books )
	{
		int index = 0;
		//Clear all bars
		RemoveAllChild(searchInfo);
		CreateBookInfoBar(searchInfo,index++);

		foreach(BookInfo info in books)
		{
			CreateBookInfoBar(searchInfo,index++,info);
			if (index >= 11 )
				break;
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
					t.text = info.TimeFrom.ToString() + " " + info.TimeTo.ToString();
				if (t.name == "Note")
					t.text = info.Note;
				if (t.name == "Genre")
					t.text = info.genre;
				if (t.name == "Location")
					t.text = info.Location;

			}
		}

	}
}
