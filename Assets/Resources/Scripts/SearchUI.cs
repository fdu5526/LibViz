using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SearchUI : MonoBehaviour {

	public bool isDraggingItem;
	GameObject dragShadow;

	List<SearchWhirlwindItem> itemsInSearch;

	// Use this for initialization
	void Start () {
		dragShadow = transform.Find("DragShadow").gameObject;
		dragShadow.GetComponent<Image>().enabled = false;

		itemsInSearch = new List<SearchWhirlwindItem>();
	}

	public void EnableDragShadow (Sprite sprite) {
		isDraggingItem = true;
		dragShadow.GetComponent<Image>().enabled = true;
		dragShadow.GetComponent<Image>().sprite = sprite;
	}

	public void DisableDragShadow () {
		isDraggingItem = false;
		dragShadow.GetComponent<Image>().enabled = false;
	}



	public void AddToSearch (WhirlwindItem item) {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isDraggingItem) {
			dragShadow.transform.position = Input.mousePosition;
		}
	}
}
