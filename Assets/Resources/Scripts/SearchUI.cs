using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SearchUI : MonoBehaviour {

	public bool isDraggingItem;

	GameObject dragShadow;
	Whirlwind whirlwind;

	bool dragging;

	// Use this for initialization
	void Start () {
		dragShadow = transform.Find("DragShadow").gameObject;
		dragShadow.GetComponent<Image>().enabled = false;
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
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
			whirlwind.LogUserInput();
		}


		// UI raycast out
		if (Input.GetKey("n")) {
			GraphicRaycaster gr = this.GetComponent<GraphicRaycaster>();
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			gr.Raycast(ped, results);

			if (results.Count > 1) {
				for (int i = 0; i < results.Count; i++) {
					if (!dragging) {
						ExecuteEvents.Execute(results[i].gameObject, ped, ExecuteEvents.beginDragHandler);
					} else {
						ExecuteEvents.Execute(results[i].gameObject, ped, ExecuteEvents.dragHandler);
					}
					print(results[i].gameObject.name);
				}
			}
			dragging = true;

		} else {
			if (dragging) {
				GraphicRaycaster gr = this.GetComponent<GraphicRaycaster>();
				PointerEventData ped = new PointerEventData(null);
				ped.position = Input.mousePosition;
				List<RaycastResult> results = new List<RaycastResult>();
				gr.Raycast(ped, results);

				if (results.Count > 1) {
					for (int i = 0; i < results.Count; i++) {
						ExecuteEvents.Execute(results[i].gameObject, ped, ExecuteEvents.endDragHandler);
						ExecuteEvents.Execute(results[i].gameObject, ped, ExecuteEvents.dropHandler);
					}
				}
				dragging = false;
			}
			
		}
		
	}
}