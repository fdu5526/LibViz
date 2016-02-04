using UnityEngine;
using System.Collections;

public class SearchWhirlwindItem {
	public Sprite sprite;
	public string id;

	public SearchWhirlwindItem (WhirlwindItem wwItem) {
		id = wwItem.name;
		sprite = wwItem.ItemSprite;
	}
}