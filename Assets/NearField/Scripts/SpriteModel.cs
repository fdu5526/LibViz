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
 * SpriteModel renders near field models in VR / monoscopic applications. This model uses simple 
 * billboard technique without light-field interpolation. Note that the left eye camera must be having 
 * "LeftEye" layer included, but "RightEye" layer excluded, and right eye camera must have "RightEye" 
 * layer included, but "RightEye" layer excluded.
 * 
 * Properties under SpriteModel script object must be set as follows:
 *		- Rotation Offset : The rotation angle in degree applied as a correction to orient the object 
 *							to look forward. Default is -90, because the current camera rig captures the 
 *							frame 0 at 90 degree from the front
 *		- CurrentRotation : The rotation that is applied to the model in the current frame. This could be 
 *							used by another script or an Animator component to rotate the object around 
 *							the center in an animation
 *
 *		- Show Camera Bounds : Enabling this control shows the bounding frustums in the Unity Editor's 
 *								scene window where the camera must be placed to avoid the undesirable 
 *								perspective warp that could be seen on a near field model
 *		- Camera Vertical FOV : The vertical FOV of the camera used to capture the model. This is needed 
 *								to compute the bounds in which the model will appear less skewed. The default 
 *								is set as 27, which is the vertical FOV of the 4K camera used in the MxR studio
 *		- Capture Angular Depression : The downward angle with which the camera captured the object. This 
 *										is needed to compute the bounds in which the model will appear less skewed.
 *		- Tolerable Angular Error : The maximum angular error that could be tolerated within which the skew 
 *									is not apparent. Default is set as 20. This is needed to compute the bounds 
 *									in which the model will appear less skewed.
 *
 *		- Images Per Frame : Number of images that are stored in the video to complete 1 rotation. For high 
 *								quality we create the interpolated video with 720 images per frame. So default 
 *								is set as 720
 *		- Clockwise capture : This checkbox could be used if the object was captured in the clockwise direction 
 *								on the turntable, or the image was captured upside down, but was later corrected 
 *								in the video.
 *		- Frame Count : This is the number of frames that are recorded in the video. For static objects, it is 
 *						set as 1 by default.
 *		- Video Folder Path : The folder path in which the video is placed. Please not that the video must be 
 *								placed on an SSD for optimal performance
 *		- Video File Name : The file name of the video holding the frames of the object.
 *		- Current Frame Index : The index of the current frame being rendered. This could be used by another 
 *								script or an Animator component to change the frame that is currently being 
 *								rendered in a variable fashion.
 */

using UnityEngine;
using System.Collections;

public class SpriteModel : BillBoardModel {

	public int imagesPerFrame = 720;
	public int currentFrameIndex = 0;
	public bool clockwiseCapture = false;
	public int frameCount;

	public string videoFolderPath;
	public string videoFileName;

	float prevMouseX;
	InputManager inputManager;

	void Start () {
		inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();		
	}

	private void Update () {
	
		billboardFrameIndex = currentFrameIndex;
	}
	
	public uint GetFrameIndex (float objectAngle)
	{
		int intFrameIndex = Mathf.FloorToInt (billboardFrameIndex);
		intFrameIndex %= frameCount;
		
		float newAngle = objectAngle + currentRotation + rotationOffset;
		newAngle *= -1f;
		newAngle = newAngle - 360.0f * Mathf.Floor (newAngle / 360.0f);
		
		float processedAngle = (newAngle * (imagesPerFrame / 360f)) % imagesPerFrame;
		if (clockwiseCapture) processedAngle = imagesPerFrame - processedAngle;
		
		float frameIndex = intFrameIndex * imagesPerFrame + processedAngle;
		
		return (uint)(frameIndex);
	}


	void OnMouseDown () {
		prevMouseX = Input.mousePosition.x;
	}


	void OnMouseDrag () {
		if (inputManager.IsDragging) {
			float mouseX = Input.mousePosition.x;
			float d = (mouseX - prevMouseX) / 3f;
			prevMouseX = mouseX;

			// ignore extraneous input
			if (Mathf.Abs(d) > 1f) {
				currentRotation += d;
			}
		}
	}
}
