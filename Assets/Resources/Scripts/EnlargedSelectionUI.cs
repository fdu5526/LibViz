using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnlargedSelectionUI : MonoBehaviour {

	Image objectImage;

	// Use this for initialization
	void Start () {

		objectImage = transform.Find("ObjectImage").GetComponent<Image>();
	}



	public Sprite ObjectSprite {
		set {
			objectImage.sprite = value;
		}
	}
	

	// Update is called once per frame
	void Update () {
	
	}
}
