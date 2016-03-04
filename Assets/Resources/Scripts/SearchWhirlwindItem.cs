using UnityEngine;
using System.Collections;

public class SearchWhirlwindItem {
	Sprite sprite;
	BookInfo bookInfo;

	public SearchWhirlwindItem (WhirlwindItem wwItem) {
		bookInfo = wwItem.bookInfo;
		sprite = wwItem.ItemSprite;
	}

	public Sprite Sprite { get { return sprite; } }
	public BookInfo BookInfo { get { return bookInfo; } }
}