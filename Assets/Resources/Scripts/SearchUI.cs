using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SearchUI : MonoBehaviour {

	public bool isDraggingObject;
	GameObject dragShadow;

	// Use this for initialization
	void Start () {
		dragShadow = transform.Find("DragShadow").gameObject;
		dragShadow.GetComponent<Image>().enabled = false;
	}

	public void EnableDragShadow (Sprite sprite) {
		isDraggingObject = true;
		dragShadow.GetComponent<Image>().enabled = true;
		dragShadow.GetComponent<Image>().sprite = sprite;
	}

	public void DisableDragShadow () {
		isDraggingObject = false;
		dragShadow.GetComponent<Image>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (isDraggingObject) {
			dragShadow.transform.position = Input.mousePosition;
		}
	}
}
