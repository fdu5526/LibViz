using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IBeginDragHandler {

	int index;

	bool isSelected;
	Whirlwind whirlwind;
	SearchWhirlwindItem searchWhirlwindItem;

	// Use this for initialization
	void Start () {
		index = transform.GetSiblingIndex();
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		isSelected = false;
		searchWhirlwindItem = null;
	}

	public bool IsFilled { get { return searchWhirlwindItem != null; } }
	public bool IsSelected { 
		get { return isSelected; } 
		set { isSelected = value; }
	}

	public void SetDraggedSearchItem (SearchWhirlwindItem s) {
		searchWhirlwindItem = s;
		GetComponent<Image>().sprite = s.sprite;
		index = transform.GetSiblingIndex();
		isSelected = true;
	}

	public int Index { get { return index; } }

	public void OnBeginDrag(PointerEventData eventData) {
		print("dragging a slot");
		if (IsFilled) {

		}
	}
}
