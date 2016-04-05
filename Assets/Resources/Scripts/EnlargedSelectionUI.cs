using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnlargedSelectionUI : MonoBehaviour {

	Image itemImage;
	Text title;

	// Use this for initialization
	void Start () {
		itemImage = transform.Find("ItemImage").GetComponent<Image>();
		title = transform.Find("Title").GetComponent<Text>();
	}

	public void SetBookInfo (BookInfo bookInfo, Sprite sprite) {
		title.text = bookInfo.Title + "\n" + bookInfo.Author + ", " + bookInfo.GetData("pub_date");
		itemImage.sprite = sprite;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
