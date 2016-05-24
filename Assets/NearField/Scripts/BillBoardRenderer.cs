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
	Timer nextFrameTimer;
    
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
		nextFrameTimer = new Timer(1f / 20f);
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

  public void LoadMovieTest () {
  	model.currentFrameIndex = 0;
  	isPlaying = false;

  	string[] mxr = {"fml_0206_boxP6.mp4", "specol_0117_box14_3.mp4", "specol_0123_box2_6.mp4", "specol_0315_box199_1.mp4", "specol_6044_box7_envelope4.mp4", "PR4612_EM37_c2013.mp4", "specol_0117_box60.mp4", "specol_0123_box2_7.mp4", "specol_0315_box199_2.mp4", "specol_6047_box1_folder9.mp4", "PR462_EA45_1960z.mp4", "specol_0123_box1_folder2.mp4", "specol_0123_box2_8.mp4", "specol_0315_box55_1.mp4", "specol_6407_box1_folder1.mp4", "specol_0110_box92_1.mp4", "specol_0123_box2_1.mp4", "specol_0155_Box22.mp4", "specol_0315_box55_2.mp4", "specol_N7433_4_R543_C43_2006.mp4", "specol_0110_box92_2.mp4", "specol_0123_box2_2.mp4", "Specol_0293_box9.mp4", "specol_0315_box55_3.mp4", "specol_NC982_5_L5_1562.mp4", "specol_0110_box92_3.mp4", "specol_0123_box2_3.mp4", "specol_0304_box1_2.mp4", "specol_0315_box55_4.mp4", "specol_PS3566_A92_H23_1996.mp4", "specol_0117_box14_1.mp4", "specol_0123_box2_4.mp4", "specol_0304_box1_3.mp4", "specol_0315_box55_5.mp4", "specol_0117_box14_2.mp4", "specol_0123_box2_5.mp4", "specol_0315_box199.mp4", "specol_6002_1.mp4"};

  	string[] caramel = {"specol_0037_box1_folder23.mov", "specol_0388_box62_2.mov", "specol_N7433_4_G65_M36_1995.mov", "specol_N7433_4_T78J45_2006.mov", "specol_0110_box41_folder3.mov", "specol_6067_box1_folder23.mov", "specol_N7433_4_H34S6_2005.mov", "specol_N7433_4_W6_1996.mov", "specol_0110_box41_folder5.mov", "specol_6067_box1_folder24.mov", "specol_N7433_4_H396_R45_1995.mov", "specol_N7433_4_Y36_N3_2001.mov", "specol_0304_box1_1.mov", "specol_7011_box1.mov", "specol_N7433_4_M558_A84_2002.mov", "specol_N7433_4M86S4_1996.mov", "specol_0315_box55.mov", "specol_GV1235I73_2003.mov", "specol_N7433_4_M558_C67_2010.mov", "specol_N7433_7_L36_A45_2005.mov", "specol_0388_box4_folder10.mov", "specol_GV1525_L36_1851.mov", "specol_N7433_4_M558C67_2010.mov", "specol_PR4611_B63_2012.mov", "specol_0388_box4_folder11.mov", "specol_N7433_3T537_C53_2011.mov", "specol_N7433_4_M86_C38_2005.mov", "specol_PR4854_J88A6_1997.mov", "specol_0388_box4_folder12.mov", "specol_N7433_4_A34_S8_2010.mov", "specol_N7433_4_M86_S2_2005.mov", "specol_PS3554_O884_L3_1994.mov", "specol_0388_box57_folder3_ov10.mov", "specol_N7433_4_B38_I57_1993.mov", "specol_N7433_4_N44_M66_2011.mov", "specol_PS3561_A8612_D44_1998.mov", "specol_0388_box57_folder3_ov3_1.mov", "specol_N7433_4_C44_L54_1996.mov", "specol_N7433_4_O23_D66_1990.mov", "specol_T785_L1_H37_1939.mov", "specol_0388_box57_folder3_ov4.mov", "specol_N7433_4_C44_W67_1999.mov", "specol_N7433_4_O74_S77_2007.mov", "specol_Z105_5_1450_C378.mov", "specol_0388_box57_folder3_ov5.mov", "specol_N7433_4_C55_B66_1998.mov", "specol_N7433_4_R55H67_2004.mov", "specol_Z105_5_1460_C378.mov", "specol_0388_box57_folder3_ov7.mov", "specol_N7433_4_D4285S65_2007.mov", "specol_N7433_4_R83_E53_2006.mov", "specol_0388_box57_folder3_ov8.mov", "specol_N7433_4_F47_L58_2005.mov", "specol_N7433_4_S35_O53_2000.mov", "specol_0388_box62_1.mov", "specol_N7433_4_G648_O26_1992.mov", "specol_N7433_4_T47G55_2011.mov"};

  	string mxrFolderPath = model.videoFolderPath + "MxR\\";
  	string mxrFileName = mxr[(int)UnityEngine.Random.Range(0, mxr.Length)];

  	string caramelCornFolderPath = model.videoFolderPath + "CaramelCorn\\";
  	string caramelCornFileName = caramel[(int)UnityEngine.Random.Range(0, caramel.Length)];

  	if (UnityEngine.Random.value > 0.5f) { // it is an MxR video
	  		movie._folder = mxrFolderPath;
			movie._filename = mxrFileName;
			movie.LoadMovie();

			GetComponent<Renderer>().material.color = Color.white;
			currentVideoType = VideoType.MxR;
			model.currentRotation = 180f;
  	} else { // it is a caramel corn video
  		movie._folder = caramelCornFolderPath;
			movie._filename = caramelCornFileName;
			movie.LoadMovie();

			GetComponent<Renderer>().material.color = Color.white;
			currentVideoType = VideoType.CaramelCorn;
			model.currentRotation = -100f;
  	}
  }

  // only call this from the PlayPause button in the UI and spriteModel.cs
	public void PlayPause () {
		isPlaying = !isPlaying;
	}

	// only call this from FullscreenSelectionUI.cs
	public void SetFrameIndex (int index) {
		if (IsCurrentVideoMxR) {
			model.currentFrameIndex = index;
		} else if (IsCurrentVideoCaramelCorn) {
			movie._moviePlayer.Frame = (uint)Mathf.RoundToInt((float)index * caramelCornFramesPerView);
			fullscreenSelectionUI.SetProgress((float)movie._moviePlayer.Frame/(float)movie._moviePlayer.FrameCount);
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
			 		float prevRotation = model.currentRotation;
			 		model.currentRotation -= 0.4f;
			 		model.currentRotation = model.currentRotation % 360f;

			 		// rotated past the original point, time to move to the next frame
			 		float prevRotationClamped = (Mathf.Abs(prevRotation - 180f)) % 360f;
			 		float currentRotationClamped = (Mathf.Abs(model.currentRotation - 180f)) % 360f;
			 		if (currentRotationClamped < prevRotationClamped) {
			 			int i = (model.currentFrameIndex + 1) % model.frameCount;
			 			fullscreenSelectionUI.SetCurrentFrame(i);
			 			fullscreenSelectionUI.SetProgress((float)(i)/Mathf.Max((float)(model.frameCount - 1), 1f));
			 		}

			 	}
			} else if (IsCurrentVideoCaramelCorn && isPlaying) {
				if (movie._moviePlayer.FrameCount > 0 && nextFrameTimer.IsOffCooldown) {
					frame = (movie._moviePlayer.Frame + 1) % movie._moviePlayer.FrameCount;

					fullscreenSelectionUI.SetHighlightedButton(Mathf.FloorToInt((float)frame / caramelCornFramesPerView));
					fullscreenSelectionUI.SetProgress((float)frame/(float)movie._moviePlayer.FrameCount);

					nextFrameTimer.Reset();
				}
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
