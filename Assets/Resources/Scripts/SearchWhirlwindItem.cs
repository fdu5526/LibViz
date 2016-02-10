using UnityEngine;
using System.Collections;

public class SearchWhirlwindItem {
	public Sprite sprite;
	public BookInfo bookInfo;

	public SearchWhirlwindItem (WhirlwindItem wwItem) {
		bookInfo = wwItem.bookInfo;
		sprite = wwItem.ItemSprite;
	}

	public SearchWhirlwindItem (SearchWhirlwindItem s) {
		this.bookInfo = s.bookInfo;
		this.sprite = s.sprite;
	}
}