using UnityEngine;
using System.Collections;

public class SearchWhirlwindItem {
	public WhirlwindItem wwItem;
	public bool isSelected;

	public SearchWhirlwindItem (WhirlwindItem wwItem) {
		this.wwItem = wwItem;
		this.isSelected = true;
	}
}