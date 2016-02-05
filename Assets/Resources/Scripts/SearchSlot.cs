using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler {

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
	}

	void FlipSelection () {
		isSelected = !isSelected;
	}

	public BookInfo BookInfo {
		get {
			Debug.Assert(IsFilled);
			return searchWhirlwindItem.bookInfo;
		}
	}

	public int Index { get { return index; } }

	public void SetDraggedSearchItem (SearchWhirlwindItem s) {
		searchWhirlwindItem = s;
		GetComponent<Image>().sprite = s.sprite;
		index = transform.GetSiblingIndex();
		isSelected = true;
	}

	public void DestroySelf () {
		Destroy(this.gameObject);
	}

	public void OnPointerClick(PointerEventData eventData) {
		print("yay");
		FlipSelection();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		print("dragging a slot");
		if (IsFilled) {

		}
	}
}
