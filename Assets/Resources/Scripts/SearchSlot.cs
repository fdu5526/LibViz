﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IBeginDragHandler {

	int index;

	bool isFilled;
	bool isSelected;
	Whirlwind whirlwind;

	// Use this for initialization
	void Start () {
		index = transform.GetSiblingIndex();
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		isFilled = false;
		isSelected = false;
	}

	public void SetDraggedSearchItem (SearchWhirlwindItem s) {
		GetComponent<Image>().sprite = s.sprite;
		isFilled = true;
		index = transform.GetSiblingIndex();
	}

	public int Index { get { return index; } }

	public void OnBeginDrag(PointerEventData eventData) {
		print("dragging a slot");
		if (isFilled) {
		}
	}
}
