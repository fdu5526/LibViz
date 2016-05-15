/*************************************************
 * Copyright 2016
 * MxR Studio
 * School of Cinematic Arts, USC
 * 
 * Author: Ashok Mathew Kuruvilla
 * 		M.S. in CS (Multimedia & Creative Tech.)
 * 		Viterbi School of Engineering, USC
 * 
 * Email: akuruvil@usc.edu
 *************************************************/

/**
 * BillBoardRenderer represents a simple sprite renderer that selects a sprite from a video file,
 * and projects it on a Quad game object. It is a component under a SpriteModel that renders a 
 * near field model as monoscopic or stereoscopic content.
 */

using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent (typeof (AVProQuickTimeMovie))]
public class BillBoardRenderer : MonoBehaviour {

    public Vector3 rotationCorrection;
	SpriteModel model;
	AVProQuickTimeMovie movie;
	Renderer billboardRenderer;

	enum VideoType { MxR, CaramelCorn, None };
	VideoType currentVideoType;

	int prevFrameCount;
	FullscreenSelectionUI fullscreenSelectionUI;

	// for autoplaying
	bool isPlaying;
	const float caramelCornFramesPerView = 96f;
    
	void Awake(){

		model = transform.parent.GetComponent<SpriteModel> ();
		movie = GetComponent<AVProQuickTimeMovie> ();
        billboardRenderer = GetComponent<Renderer>();
		movie._folder = model.videoFolderPath;
		movie._filename = model.videoFileName;
		currentVideoType = VideoType.None;

		fullscreenSelectionUI = GameObject.Find("FullscreenSelectionUI").GetComponent<FullscreenSelectionUI>();
		prevFrameCount = 0;
		isPlaying = false;
    }


    public bool IsCurrentVideoMxR { get { return currentVideoType == VideoType.MxR; } }
    public bool IsCurrentVideoCaramelCorn { get { return currentVideoType == VideoType.CaramelCorn; } }

    public void LoadMovie () {
    	model.currentFrameIndex = 0;
    	isPlaying = false;

    	string mxrFolderPath = model.videoFolderPath + "MxR\\";
    	string mxrFileName = model.videoFileName + ".mp4";

    	string caramelCornFolderPath = model.videoFolderPath + "CaramelCorn\\";
    	string caramelCornFileName = model.videoFileName + ".mov";

    	if (System.IO.File.Exists(mxrFolderPath + mxrFileName)) { // it is an MxR video
    		movie._folder = mxrFolderPath;
			movie._filename = mxrFileName;
			movie.LoadMovie();

			GetComponent<Renderer>().material.color = Color.white;
			currentVideoType = VideoType.MxR;
			model.currentRotation = 180f;
    	} else if (System.IO.File.Exists(caramelCornFolderPath + caramelCornFileName)) { // it is a caramel corn video
    		movie._folder = caramelCornFolderPath;
			movie._filename = caramelCornFileName;
			movie.LoadMovie();

			GetComponent<Renderer>().material.color = Color.white;
			currentVideoType = VideoType.CaramelCorn;
			model.currentRotation = -100f;
    	} else { // does not exist
			GetComponent<Renderer>().material.color = new Color(0.082f, 0.074f, 0.082f);
    		currentVideoType = VideoType.None;
    	}
    }

    // only call this from the PlayPause button in the UI
	public void PlayPause () {
		isPlaying = !isPlaying;
	}

	// only call this from FullScreenSelectionUI.cs
	public void SetFrameIndex (int index) {
		if (IsCurrentVideoMxR) {
			model.currentFrameIndex = index;
		} else if (IsCurrentVideoCaramelCorn) {
			movie._moviePlayer.Frame = (uint)Mathf.RoundToInt((float)index * caramelCornFramesPerView);
		}
	}


	void OnWillRenderObject ()
	{
		Camera cam = Camera.current;

		bool hasEditorCam = true;
#if UNITY_EDITOR
        if (UnityEditor.SceneView.lastActiveSceneView != null)
		    hasEditorCam = cam != UnityEditor.SceneView.lastActiveSceneView.camera;
#endif

		if (cam != null && hasEditorCam && movie != null &&  movie._moviePlayer != null) {

			// calculate the number of frames based on if this is MxR video or not
			if (IsCurrentVideoMxR) {
				model.frameCount = Mathf.CeilToInt ((float)movie._moviePlayer.FrameCount / (float)model.imagesPerFrame);
			} else if (IsCurrentVideoCaramelCorn) {	
				model.frameCount = Mathf.CeilToInt ((float)movie._moviePlayer.FrameCount / (caramelCornFramesPerView));
			}
			model.frameCount = Mathf.Max (1, model.frameCount);
			if (model.frameCount != prevFrameCount) {
				fullscreenSelectionUI.SetFrameCount(model.frameCount);
				prevFrameCount = model.frameCount;
			}
			
			
			// calculate the current frame of the video (different from SpriteModel frame)
			uint frame = movie._moviePlayer.Frame;
			if (IsCurrentVideoMxR) {
				//uint prevFrame = frame;
			 	frame = model.GetFrameIndex (transform.localRotation.eulerAngles.y);
			 	if (isPlaying) {
			 		model.currentRotation -= 0.4f;
			 	}
			} else if (IsCurrentVideoCaramelCorn && isPlaying) {
				if (movie._moviePlayer.FrameCount > 0) {
					frame = (movie._moviePlayer.Frame + 1) % movie._moviePlayer.FrameCount;

					fullscreenSelectionUI.SetHighlightedButton(Mathf.FloorToInt((float)frame / caramelCornFramesPerView));
				}
				fullscreenSelectionUI.SetProgress((float)frame/(float)movie._moviePlayer.FrameCount);
			}
            movie._moviePlayer.Frame = frame;
            billboardRenderer.material.mainTexture = movie.OutputTexture;
            

			// make this quad the camera
            Vector3 camPos = new Vector3 (cam.transform.position.x, transform.position.y, cam.transform.position.z);
			transform.LookAt (camPos);
            transform.Rotate (rotationCorrection.x, rotationCorrection.y, rotationCorrection.z);
		}
	}
}
