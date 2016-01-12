using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchUI : MonoBehaviour {

	public bool isDraggingItem;
	GameObject dragShadow;

	// Use this for initialization
	void Start () {
		dragShadow = transform.Find("DragShadow").gameObject;
		dragShadow.GetComponent<Image>().enabled = false;
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
	
	// Update is called once per frame
	void Update () {
		if (isDraggingItem) {
			dragShadow.transform.position = Input.mousePosition;
		}
	}
}
