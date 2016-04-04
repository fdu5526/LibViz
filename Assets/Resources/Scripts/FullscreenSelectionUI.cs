using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FullscreenSelectionUI : MonoBehaviour {

	Text fields;
	Image itemImage;
	GameObject background;

	// Use this for initialization
	void Start () {
		fields = transform.Find("Fields/Viewport/Content/Text").GetComponent<Text>();
		itemImage = transform.Find("ItemBackground/ItemImage").GetComponent<Image>();
		background = transform.Find("Background").gameObject;
		Enable(false);
	}

	public void Enable (bool enabled) {
		background.GetComponent<Collider>().enabled = enabled;
		GetComponent<Canvas>().enabled = enabled;
	}

	public string Fields {
		set {
			fields.text = value;
		}
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
