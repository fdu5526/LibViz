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
    
	void Awake(){

		model = transform.parent.GetComponent<SpriteModel> ();
		movie = GetComponent<AVProQuickTimeMovie> ();
        billboardRenderer = GetComponent<Renderer>();
		movie._folder = model.videoFolderPath;
		movie._filename = model.videoFileName;
    }

    public void LoadMovie () {
    	if (System.IO.File.Exists(model.videoFolderPath + model.videoFileName)) {
    		movie._folder = model.videoFolderPath;
			movie._filename = model.videoFileName;
			movie.LoadMovie();
    	} else {
    		movie._folder = model.videoFolderPath;
			movie._filename = "placeholder.mp4";
			movie.LoadMovie();
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

			model.frameCount = Mathf.RoundToInt ((float)movie._moviePlayer.FrameCount / (float)model.imagesPerFrame);
			model.frameCount = Mathf.Max (1, model.frameCount);
			model.SetFullscreenSelectionUIFrameCount();

            Vector3 camPos = new Vector3 (cam.transform.position.x, transform.position.y, cam.transform.position.z);
			transform.LookAt (camPos);
			uint frame = model.GetFrameIndex (transform.localRotation.eulerAngles.y);
			
            movie._moviePlayer.Frame = frame;
            billboardRenderer.material.mainTexture = movie.OutputTexture;

            transform.Rotate (rotationCorrection.x, rotationCorrection.y, rotationCorrection.z);
		}
	}
}
