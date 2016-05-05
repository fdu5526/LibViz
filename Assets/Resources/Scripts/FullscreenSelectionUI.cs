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

	GameObject frameSelector;
	List<Button> frameButtons;

	// Use this for initialization
	void Start () {
		fields = transform.Find("Fields/Viewport/Content/Text").GetComponent<Text>();
		background = transform.Find("Background").gameObject;
		spriteModel = GameObject.Find("StaticSpriteModel-Mono").GetComponent<SpriteModel>();
		billBoardRenderer = GameObject.Find("StaticSpriteModel-Mono/BillboardRenderer").GetComponent<BillBoardRenderer>();

		frameSelector = transform.Find("FrameSelector").gameObject;

		Enable(false);
		frameButtons = new List<Button>();
		frameButtons.Add(transform.Find("FrameSelector/Button").GetComponent<Button>());
	}

	public void Enable (bool enabled) {
		if (enabled) {
			Debug.Assert(currentBookInfo != null);
		}

		spriteModel.gameObject.SetActive(enabled);

		if (enabled) {
			spriteModel.videoFileName = currentBookInfo.FileName + ".mp4";
			billBoardRenderer.LoadMovie();
		}

		background.GetComponent<Collider>().enabled = enabled;
		GetComponent<Canvas>().enabled = enabled;
	}

	// because spriteModel doesn't update instanteously
	public void SetFrameCount (int frameCount) {
		Debug.Assert(frameCount > 0);
		Debug.Assert(frameButtons.Count > 0);

		while (frameCount != frameButtons.Count) {
			Debug.Assert(frameButtons.Count > 0);

			if (frameCount > frameButtons.Count) {
				GameObject g = (GameObject)Instantiate(frameButtons[0].gameObject, frameButtons[0].transform.position, Quaternion.identity);
				g.transform.parent = frameSelector.transform;
				g.transform.localScale = Vector3.one;
				g.GetComponent<FrameButton>().Index = frameButtons.Count;

				frameButtons.Add(g.GetComponent<Button>());
			} else {
				Destroy(frameButtons[frameButtons.Count - 1].gameObject);
				frameButtons.RemoveAt(frameButtons.Count - 1);
			}
		}
	}


	public void SetCurrentFrame (int index) {
		Debug.Assert(index >= 0);
		Debug.Assert(index < frameButtons.Count);
		
		spriteModel.currentFrameIndex = index;
	}

	public void SetBookInfo (BookInfo bookInfo, Sprite sprite) {
		currentBookInfo = bookInfo;
		fields.text = 
			bookInfo.Title + "\n\n" + 
			bookInfo.Author + "\n\n" + 
			bookInfo.GetData("pub_date") + "\n\n";
	}

	// Update is called once per frame
	void Update () {
	}
}
