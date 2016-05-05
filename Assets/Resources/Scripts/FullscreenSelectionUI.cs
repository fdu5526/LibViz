﻿using UnityEngine;
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
		frameButtons = new List<Button>();
		frameButtons.Add(frameSelector.transform.Find("Button").GetComponent<Button>());
		Enable(false);
	}

	// turn on the UI, called from Whirlwind.cs
	public void Enable (bool enabled) {
		if (enabled) { // please have a bookInfo
			Debug.Assert(currentBookInfo != null);
		}

		spriteModel.gameObject.SetActive(enabled);
		background.GetComponent<Collider>().enabled = enabled;
		GetComponent<Canvas>().enabled = enabled;

		// if turning on, load the video
		if (enabled) {
			spriteModel.currentFrameIndex = 0;
			spriteModel.currentRotation = 0f;

			spriteModel.videoFileName = currentBookInfo.FileName + ".mp4";
			billBoardRenderer.LoadMovie();
		}	
	}

	// because spriteModel doesn't update instanteously
	public void SetFrameCount (int frameCount) {
		Debug.Assert(frameCount > 0);
		Debug.Assert(frameButtons.Count > 0);

		// add or remove buttons till we get the amount we want
		while (frameCount != frameButtons.Count) {
			Debug.Assert(frameButtons.Count > 0);

			if (frameCount > frameButtons.Count) { // add a button
				GameObject g = (GameObject)Instantiate(frameButtons[0].gameObject, frameButtons[0].transform.position, Quaternion.identity);
				g.transform.SetParent(frameSelector.transform);
				g.transform.localScale = Vector3.one;
				g.GetComponent<FrameButton>().Index = frameButtons.Count;

				frameButtons.Add(g.GetComponent<Button>());
			} else { // remove a button
				Destroy(frameButtons[frameButtons.Count - 1].gameObject);
				frameButtons.RemoveAt(frameButtons.Count - 1);
			}
		}
	}

	// set the frame, only call this from  FrameButton.cs
	public void SetCurrentFrame (int index) {
		Debug.Assert(index >= 0);
		Debug.Assert(index < frameButtons.Count);
		
		spriteModel.currentFrameIndex = index;
	}

	// set this in Whirlwind.cs
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
