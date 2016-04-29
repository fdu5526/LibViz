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
 * SimpleBillboardModel renders animating near field models in VR applications. This model uses simple 
 * billboard technique without light-field interpolation. Note that the left eye camera must be having 
 * "LeftEye" layer included, but "RightEye" layer excluded, and right eye camera must have "RightEye" 
 * layer included, but "RightEye" layer excluded.
 * 
 * Properties under SimpleBillboardModel script object must be set as follows:
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
 *		- Captured FPS : The frame rate at which the stop motion was captured
 *		- Current Frame Index : The index of the current frame being rendered. This could be used by another 
 *								script or an Animator component to change the frame that is currently being 
 *								rendered in a variable fashion.
 *		- Auto Animate : Check this box on if the animation needs to be automatically played at Captured FPS 
 *						 when the object is enabled.
 *		- Loop Animation : Check this box on if the animation must be played in non-stop loop, if Auto Animate 
 *						   is enabled
 */

using UnityEngine;
using System.Collections;

public class SimpleBillboardModel : SpriteModel {

	public float capturedFPS = 12;
	public float currentFrameIndex = 0f;

	public bool autoAnimate = false;
	public bool loopAnimation = true;

	private bool playingAsyncLoop = false;
	
	void StartPlaying ()
	{
		if (autoAnimate && !playingAsyncLoop) {
			
			Coroutine coroutine = StartCoroutine (UpdateFrameInLoopAsync());
			if (coroutine != null) playingAsyncLoop = true;
		}
	}
	
	void Update ()
	{
		StartPlaying ();
		billboardFrameIndex = currentFrameIndex;
	}
	
	IEnumerator UpdateFrameInLoopAsync() {
		
		while (true) { // animate if loopAnimation is true or the current sequence is not complete
			
			if(currentFrameIndex > 0 || loopAnimation) {
				
				currentFrameIndex ++;
			}
			
			yield return new WaitForSeconds(1f/capturedFPS);
		}
	}
	
	public void StartAnimationLoop()
	{
		if(currentFrameIndex == 0) currentFrameIndex = Mathf.Min(1, frameCount);
	}
}
