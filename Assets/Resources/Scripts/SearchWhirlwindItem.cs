using UnityEngine;
using System.Collections;

public class SearchWhirlwindItem {
	Sprite sprite;
	BookInfo bookInfo;

	public SearchWhirlwindItem (WhirlwindItem wwItem) {
		this.bookInfo = wwItem.BookInfo;
		this.sprite = wwItem.Sprite;
	}

	public Sprite Sprite { get { return sprite; } }
	public BookInfo BookInfo { get { return bookInfo; } }
}