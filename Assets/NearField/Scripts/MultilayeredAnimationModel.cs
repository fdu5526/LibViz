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
 * MultilayeredAnimationModel represents a near field model that renders multi layered animating 
 * near field models in VR applications. Multilayered animations are those animations where in 
 * multiple actions are taken by the character at once. For example, a character could raise either 
 * of the hand in any possible combination and order, and all the frames for all possible combinations 
 * must be captured in layered sequences. This model uses sprite rendering technique without 
 * light-field interpolation. Note that the left eye camera must be having "LeftEye" layer included, 
 * but "RightEye" layer excluded, and right eye camera must have "RightEye" layer included, 
 * but "RightEye" layer excluded.
 * 
 * Properties under MultilayeredAnimationModel script object must be set as follows:
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
 *		- Layers : 
 *			Layer Count : Number of layers in the animation. For example, if any combination of either of the 
 *							hand raise animation exist, it has 2 layers - 1 layer for each hand.
 *			For each layer:
 *				Action Name : A string representing the action taken in each leyer (eg. "Raise Left Hand" in layer 1 
 *							  / "Raise Right Hand" in Layer 2)
 *				Frame Count: Number of frame the particular action takes
 *		- Current Layer <n> Frame : These field expose the control to select which frame must be chosen for each layer 
 *									so that any possible action sequence could be rendered. These fields could be used 
 *									by the Animator component or an external script to render an action sequence.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MultilayeredAnimationModel : SpriteModel {

	[Serializable]
	public class StopMotionLayer {

		public string actionName;
		public int frameCount;
	}

	public List<StopMotionLayer> layers;

	public float currentLayer1Frame;
	public float currentLayer2Frame;
	public float currentLayer3Frame;
	public float currentLayer4Frame;
	public float currentLayer5Frame;

	void Update ()
	{
		int multiplier = 1;
		int index = 0;

		int iterator = 0;
		float[] currentFrames = {	currentLayer1Frame,
									currentLayer2Frame,
									currentLayer3Frame,
									currentLayer4Frame,
									currentLayer5Frame };

		foreach (StopMotionLayer layer in layers) {

			index += multiplier * Mathf.FloorToInt (Mathf.Clamp (currentFrames[iterator], 0, (float)layer.frameCount - 0.5f));
			multiplier *= layer.frameCount;
			iterator ++;
		}

		billboardFrameIndex = index;
	}
}