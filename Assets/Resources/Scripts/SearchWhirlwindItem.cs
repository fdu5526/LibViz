using UnityEngine;
using System.Collections;

public class SearchWhirlwindItem {
	public Sprite sprite;
	public string id;

	public SearchWhirlwindItem (WhirlwindItem wwItem) {
		// TODO have a better way of getting ID
		id = wwItem.name;
		sprite = wwItem.ItemSprite;
	}
}