using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

	int index;

	bool isSelected;
	Whirlwind whirlwind;
	SearchWhirlwindItem searchWhirlwindItem;
	SearchBar searchBar;
	Timer holdTimer;
	bool isHeld;

	// Use this for initialization
	void Awake () {
		index = transform.GetSiblingIndex();
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		searchBar = GameObject.Find("SearchUI/SearchBar").GetComponent<SearchBar>();
		isSelected = false;
		isHeld = false;
		holdTimer = new Timer(0.5f);
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

	public void OnPointerDown (PointerEventData eventData) {
		holdTimer.Reset();
		isHeld = true;
	}

	public void OnPointerUp (PointerEventData eventData) {
		if (isHeld) {
			FlipSelection();
			isHeld = false;
		}
	}

	void FixedUpdate () {
		if (isHeld && holdTimer.IsOffCooldown) {
			whirlwind.EnterEnlargeSelection(searchWhirlwindItem);
			isHeld = false;
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if (IsFilled) {
			// TODO determine if this is vertical
			whirlwind.DragItemImage(searchWhirlwindItem);
			GetComponent<Image>().raycastTarget = false;
		}
	}

	public void OnDrag(PointerEventData data) { }

	public void OnEndDrag (PointerEventData data) {
		whirlwind.DropItemImage();
		searchBar.RemoveSlot(this);
		DestroySelf();
	}
}
