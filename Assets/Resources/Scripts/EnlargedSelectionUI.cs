using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnlargedSelectionUI : MonoBehaviour {

	Image itemImage;

	// Use this for initialization
	void Start () {

		itemImage = transform.Find("ItemImage").GetComponent<Image>();
	}



	public Sprite ItemSprite {
		set {
			itemImage.sprite = value;
		}
	}
	

	// Update is called once per frame
	void Update () {
	
	}
}
