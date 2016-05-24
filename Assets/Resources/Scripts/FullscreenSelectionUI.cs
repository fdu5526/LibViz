using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FullscreenSelectionUI : MonoBehaviour {

	Text fields;
	GameObject background;

	BookInfo currentBookInfo;
	
	SpriteModel spriteModel;
	BillBoardRenderer billBoardRenderer;
	Slider progressBar;
	GameObject frameSelector;
	List<Button> frameButtons;

	// Use this for initialization
	void Start () {
		fields = transform.Find("Fields/Viewport/Content/Text").GetComponent<Text>();
		background = transform.Find("Background").gameObject;
		spriteModel = GameObject.Find("StaticSpriteModel-Mono").GetComponent<SpriteModel>();
		billBoardRenderer = GameObject.Find("StaticSpriteModel-Mono/BillboardRenderer").GetComponent<BillBoardRenderer>();
		progressBar = transform.Find("ProgressBar").GetComponent<Slider>();

		frameSelector = transform.Find("FrameSelector").gameObject;
		frameButtons = new List<Button>();
		frameButtons.Add(frameSelector.transform.Find("Button").GetComponent<Button>());
		frameButtons[0].GetComponent<FrameButton>().Index = 0;
		Enable(false);
	}

	// turn on the UI, called from Whirlwind.cs
	public void Enable (bool enabled) {
		if (enabled) { // please have a bookInfo
			Debug.Assert(currentBookInfo != null);
		}

		// make everything visible
		spriteModel.gameObject.SetActive(enabled);
		background.GetComponent<Collider>().enabled = enabled;
		GetComponent<Canvas>().enabled = enabled;

		// if turning on, load the video
		if (enabled) {
			spriteModel.videoFileName = currentBookInfo.FileName;
			billBoardRenderer.LoadMovie();
			SetHighlightedButton(0);
		}	
	}

	// because spriteModel doesn't update instanteously
	public void SetFrameCount (int frameCount) {
		Debug.Assert(frameCount > 0);
		Debug.Assert(frameButtons.Count > 0);
		frameCount = 10;
		
		// add or remove buttons till we get the amount we want
		while (frameCount != frameButtons.Count) {
			Debug.Assert(frameButtons.Count > 0);

			if (frameCount > frameButtons.Count) { // add a button
				GameObject g = (GameObject)Instantiate(frameButtons[0].gameObject, frameButtons[0].transform.position, Quaternion.identity);
				g.transform.SetParent(frameSelector.transform);
				g.transform.localScale = Vector3.one;
				g.GetComponent<FrameButton>().Index = frameButtons.Count;

				ColorBlock cb = g.GetComponent<Button>().colors;
				cb.normalColor = Color.white;
				cb.highlightedColor = Color.white;
				g.GetComponent<Button>().colors = cb;

				frameButtons.Add(g.GetComponent<Button>());
			} else { // remove a button
				Destroy(frameButtons[frameButtons.Count - 1].gameObject);
				frameButtons.RemoveAt(frameButtons.Count - 1);
			}
		}

		SetProgress(0f);
		float y = progressBar.GetComponent<RectTransform>().offsetMax.y;
		progressBar.GetComponent<RectTransform>().offsetMax = new Vector2(-135f + (frameButtons.Count - 1) * 90f + 44f, y);
	}


	public void SetProgress (float percent) {
		progressBar.value = percent;
	}


	public void SetHighlightedButton (int index) {
		Debug.Assert(index >= 0);
		Debug.Assert(index < frameButtons.Count);

		// highlight all to the left of our current one, unlight the rest
		for (int i = 0; i < frameButtons.Count; i++) {
			Color c = i <= index ? new Color(1f, 0.887f, 0.435f) : Color.white;
			ColorBlock cb = frameButtons[i].colors;
			cb.normalColor = c;
			cb.highlightedColor = c;
			frameButtons[i].colors = cb;	
		}
	}

	// set the frame, only call this from  FrameButton.cs and BillBoardRenderer.cs
	public void SetCurrentFrame (int index) {
		Debug.Assert(index >= 0);
		Debug.Assert(index < frameButtons.Count);
		
		SetHighlightedButton(index);
		billBoardRenderer.SetFrameIndex(index);
	}

	// set this in Whirlwind.cs
	public void SetBookInfo (BookInfo bookInfo, Sprite sprite) {
		currentBookInfo = bookInfo;

		string[] columns = {"title", "title_subtitle", "name", "pub_date", "pub_place", "note", "scope_content", "history"};
		string text = "";
		for (int i = 0; i < columns.Length; i++) {
			string s = bookInfo.GetData(columns[i]);
			if (s.Length > 1) {
				text += s + "\n\n";
			}

		}

		transform.Find("Fields").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
		fields.text = text;

	}

	// Update is called once per frame
	void Update () {
	}
}
