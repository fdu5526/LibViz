using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SearchSlot : MonoBehaviour, IBeginDragHandler, IDropHandler {

	int index;

	bool isFilled;
	Whirlwind whirlwind;

	// Use this for initialization
	void Start () {
		index = transform.GetSiblingIndex();
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
	}


	public int Index { get { return index; } }

	public void OnBeginDrag(PointerEventData eventData) {
		if (isFilled) {

		}
	}
	
	public void OnDrop(PointerEventData eventData) {
		if (!isFilled) {
			GetComponent<Image>().sprite = whirlwind.EnlargedItemSprite;
			whirlwind.AddEnlargedItemToSearch(index);
			isFilled = true;
		}
		
	}

}
