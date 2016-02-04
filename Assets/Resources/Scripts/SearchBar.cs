﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SearchBar : MonoBehaviour, IBeginDragHandler, IDropHandler {
	Whirlwind whirlwind;

	List<SearchSlot> slots;
	Transform content;

	// Use this for initialization
	void Start () {
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		content = transform.Find("Viewport/Content");
		slots = new List<SearchSlot>();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		print("on begin drag");
	}
	
	public void OnDrop(PointerEventData eventData) {
		if (whirlwind.IsDraggingSearchItem) {
			SearchSlot newSlot = ((GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/SearchSlot"))).GetComponent<SearchSlot>();
			newSlot.transform.SetParent(content);
			newSlot.transform.localScale = Vector3.one;
			newSlot.SetDraggedSearchItem(whirlwind.DraggedSearchItem);

			// TODO search through all the slots, find the index to put this guy in
		}
		
	}

}
