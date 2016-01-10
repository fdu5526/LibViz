using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FullscreenSelectionUI : MonoBehaviour {

	Text fields;
	Image objectImage;

	// Use this for initialization
	void Start () {
		fields = transform.Find("Fields/Viewport/Content/Text").GetComponent<Text>();

		objectImage = transform.Find("ObjectBackground/ObjectImage").GetComponent<Image>();
	}


	public string Fields {
		set {
			fields.text = value;
		}
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
