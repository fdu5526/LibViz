using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnlargedSelectionUI : MonoBehaviour {

	Image itemImage;
	Text title;
	
	Image background;
	Timer flashUITimer;
	Color origBackgroundColor;
	Color flashBackgroundColor;
	bool isFlashing;

	// Use this for initialization
	void Start () {
		itemImage = transform.Find("ItemImage").GetComponent<Image>();
		title = transform.Find("Title").GetComponent<Text>();
		background = transform.Find("Background").GetComponent<Image>();
		
		flashUITimer = new Timer(1f);
		origBackgroundColor = background.color;
		flashBackgroundColor = new Color(0.5f, 0.5f, 0.5f, origBackgroundColor.a);
		isFlashing = false;
	}

	public void SetBookInfo (BookInfo bookInfo, Sprite sprite) {
		title.text = bookInfo.Title + "\n" + bookInfo.Author + ", " + bookInfo.GetData("pub_date");
		itemImage.sprite = sprite;
		isFlashing = true;
		flashUITimer.Reset();
	}


	// Update is called once per frame
	void FixedUpdate () {
		if (isFlashing) {
			if (flashUITimer.IsOffCooldown) {
				isFlashing = false;
				background.color = origBackgroundColor;
			} else {
				if (flashUITimer.PercentTimePassed < 0.7f) {
					background.color = Color.Lerp(origBackgroundColor, flashBackgroundColor, flashUITimer.PercentTimePassed/0.7f);
				} else {
					background.color = Color.Lerp(origBackgroundColor, flashBackgroundColor, flashUITimer.PercentTimeLeft/0.3f);
				}
			}
		}
	}
}
