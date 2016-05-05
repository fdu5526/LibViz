using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FrameButton : MonoBehaviour, IPointerClickHandler {

	int index;
	FullscreenSelectionUI fullscreenSelectionUI;

	// Use this for initialization
	void Start () {
		fullscreenSelectionUI = GameObject.Find("FullscreenSelectionUI").GetComponent<FullscreenSelectionUI>();
	}


	public int Index {
		get { return index; }
		set { index = value; }
	}

	public void OnPointerClick(PointerEventData eventData) {
		fullscreenSelectionUI.SetCurrentFrame(index);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
