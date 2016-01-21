using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WhirlwindBeltLabel : MonoBehaviour {

	Text text;


	// Use this for initialization
	void Start () {
		text = transform.Find("Text").GetComponent<Text>();
	}


	// show or hide the text label on the side
	IEnumerator FadeLabel (bool isFadeIn) {
		float increment = 0.05f;
		for (float f = 0f; f <= 1f + increment; f += increment) {
			GetComponent<CanvasGroup>().alpha = isFadeIn ? f : 1f - f;
			yield return new WaitForSeconds(0.05f);
		}
	}

	public string Text {
		get {
			return text.text;
		}

		set {
			text.text = value;
		}
	}

	public void SetToTransparent () {
		GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void Fade (bool isFadeIn) {
		if ((isFadeIn && GetComponent<CanvasGroup>().alpha > 0.99f) ||
				(!isFadeIn && GetComponent<CanvasGroup>().alpha < 0.01f)) {
			return;
		}

		StartCoroutine(FadeLabel(isFadeIn));
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
