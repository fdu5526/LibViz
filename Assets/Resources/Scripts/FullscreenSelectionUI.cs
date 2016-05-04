using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FullscreenSelectionUI : MonoBehaviour {

	Text fields;
	Image itemImage;
	GameObject background;

	[SerializeField]
	GameObject spriteModelPrefab;
	GameObject currentSpriteModel;

	// Use this for initialization
	void Start () {
		fields = transform.Find("Fields/Viewport/Content/Text").GetComponent<Text>();
		itemImage = transform.Find("ItemBackground/ItemImage").GetComponent<Image>();
		background = transform.Find("Background").gameObject;
		//spriteModel = GameObject.Find("StaticSpriteModel-Mono").GetComponent<SpriteModel>();

		//spriteModel.videoFileName = "";
		Enable(false);
	}

	public void Enable (bool enabled) {
		background.GetComponent<Collider>().enabled = enabled;
		GetComponent<Canvas>().enabled = enabled;

		if (enabled) {
			currentSpriteModel = GameObject.Instantiate(spriteModelPrefab);
			currentSpriteModel.GetComponent<SpriteModel>().videoFileName = "specol_0315_box199_2";
		} else {
			Destroy(currentSpriteModel);
		}

	}

	public void SetBookInfo (BookInfo bookInfo, Sprite sprite) {
		fields.text = 
			bookInfo.Title + "\n\n" + 
			bookInfo.Author + "\n\n" + 
			bookInfo.GetData("pub_date") + "\n\n";
		itemImage.sprite = sprite;
	}

	// Update is called once per frame
	void Update () {
	}
}
