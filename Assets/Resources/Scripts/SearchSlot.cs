using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IBeginDragHandler {

	int index;

	bool isSelected;
	Whirlwind whirlwind;
	Camera camera;
	SearchWhirlwindItem searchWhirlwindItem;

	// Use this for initialization
	void Start () {
		index = transform.GetSiblingIndex();
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		isSelected = false;
		searchWhirlwindItem = null;
		camera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	public bool IsFilled { get { return searchWhirlwindItem != null; } }
	public bool IsSelected { 
		get { return isSelected; } 
		set { isSelected = value; }
	}

	public float RectTransformPositionX {
		get {
			//return GetComponent<RectTransform>().localPosition.x;
			return (transform.position).x;
		}
	}

	public void SetDraggedSearchItem (SearchWhirlwindItem s) {
		searchWhirlwindItem = s;
		GetComponent<Image>().sprite = s.sprite;
		index = transform.GetSiblingIndex();
		isSelected = true;
	}

	public int Index { get { return index; } }

	public void OnBeginDrag(PointerEventData eventData) {
		print("dragging a slot");
		if (IsFilled) {

		}
	}
}
