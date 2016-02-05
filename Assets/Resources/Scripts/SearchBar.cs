using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SearchBar : MonoBehaviour, IBeginDragHandler, IDropHandler {
	Whirlwind whirlwind;
	Transform content;
	ScrollRect scrollRect;

	// keep slot sorted
	List<SearchSlot> slots;

	// Use this for initialization
	void Start () {
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		content = transform.Find("Viewport/Content");
		slots = new List<SearchSlot>();
		scrollRect = GetComponent<ScrollRect>();
		scrollRect.horizontalNormalizedPosition = 0f;
	}


	public List<BookInfo> SelectedBookInfos {
		get {
			List<BookInfo> infos = new List<BookInfo>();
			for (int i = 0; i < slots.Count; i++) {
				if (slots[i].IsSelected) {
					infos.Add(slots[i].BookInfo);
				}
			}
			return infos;
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		
	}
	
	public void OnDrop(PointerEventData eventData) {
		if (whirlwind.IsDraggingSearchItem) {

			// create a new slot
			SearchSlot newSlot = ((GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/SearchSlot"))).GetComponent<SearchSlot>();
			newSlot.transform.SetParent(content);
			newSlot.transform.localScale = Vector3.one;

			// search through all the slots, find the index to put this guy in
			int i = 0;
			float prevX = 0f;
			float mouseX = eventData.position.x;
			for (int j = 0; j < slots.Count; j++) {
				float x = slots[j].transform.position.x;
				if (prevX <= mouseX && mouseX <= x) {

					//TODO remove duplicates if necessary?

					break;
				} else {
					i++;
					prevX = x;
				}
			}

			// put the slot in the right place, give it the right things
			newSlot.SetDraggedSearchItem(whirlwind.DraggedSearchItem);
			newSlot.transform.SetSiblingIndex(i);
			slots.Insert(i, newSlot);
		}
		
	}

}
