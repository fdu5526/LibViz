using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

	int index;

	bool isSelected;
	Whirlwind whirlwind;
	SearchWhirlwindItem searchWhirlwindItem;
	SearchBar searchBar;

	// Use this for initialization
	void Awake () {
		index = transform.GetSiblingIndex();
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		searchBar = GameObject.Find("SearchUI/SearchBar").GetComponent<SearchBar>();
		isSelected = false;
	}

	public bool IsFilled { get { return searchWhirlwindItem != null; } }
	public bool IsSelected { 
		get { return isSelected; } 
	}

	void FlipSelection () {
		isSelected = !isSelected;
		GetComponent<Outline>().enabled = isSelected;
	}

	public BookInfo BookInfo {
		get {
			Debug.Assert(IsFilled);
			return searchWhirlwindItem.BookInfo;
		}
	}

	public int Index { get { return index; } }

	public void SetDraggedSearchItem (SearchWhirlwindItem s) {
		searchWhirlwindItem = s;
		GetComponent<Image>().sprite = s.Sprite;
		index = transform.GetSiblingIndex();
		isSelected = true;
		GetComponent<Outline>().enabled = isSelected;
	}

	public void DestroySelf () {
		Destroy(this.gameObject);
	}

	public void OnPointerClick(PointerEventData eventData) {
		// TODO if not drag
		FlipSelection();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if (IsFilled) {
			// TODO determine if this is vertical
			whirlwind.DragItemImage(searchWhirlwindItem);
		}
	}

	public void OnDrag(PointerEventData data) { }

	public void OnEndDrag (PointerEventData data) {
		whirlwind.DropItemImage();
		searchBar.RemoveSlot(this);
		DestroySelf();
	}
}
