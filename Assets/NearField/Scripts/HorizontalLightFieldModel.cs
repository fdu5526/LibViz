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
 * HorizontalLightFieldModel represents a near field model that renders the horizontal light-fields of 
 * the given model. The model is rendered by the "HorizontalLightFields_10_Atlases" shader that rebins 
 * the 360 images captured on a near-field capturing rig. The "HorizontalLightFields_10_Atlases" shader
 * consumes the 360 images provided as 10 tiled images (aka atlases), each containing 36 images for each angle. 
 * The shader then uses the captured camera parameters and rendering camera parameters to generate the 
 * raster in real-time.
 * 
 * The eclipse java project under Assets/NearField/LightField_6x6_FrameStitcher generates the tiled images 
 * that are to be provided to a HorizontalLightFieldModel. Once built, you could use the jar with the following 
 * command line options: 
 * java -jar <jar file name> <folder with frames> <image base name> <file extension> <number of images> <images per frame> <output format> 
 * You may also run the script by specifying the parameters from Eclipe's arguments field under build settings
 * 
 * The generated images (10 atlases per frame) must then be imported into a new folder under Assets/Resources. 
 * The texture properties of these 10 images must be set as follows:
 * 		- Texture Type : Advanced
 * 		- Alpha is Transparency : true
 * 		- Generate Mip Maps : false
 * 		- Wrap Mode : Clamp
 * 		- Filter Mode : Bilinear
 * 		- Max Size : 4096
 * 		- Format : RGBA Compressed DXT5
 * 
 * After importing the prefab in the scene, 
 * Properties under this HorizontalLightFieldModel script object must be set as follows:
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
 *		- Atlas Base Name : The path to the atlases under Assets/Resources for example, in the demo the 10 files 
 *							are placed under Assets/Resources/peacock_tiled_6x6. So the atlas base name is set 
 *							as "peacock_tiled_6x6/atlas"
 *		- Capture Distance Image Size Ratio : The camera distance to object height ratio that is used to compute 
 *												the camera pose information
 *		- Starting Frame Index : The starting frame number on the atlas that was generated. Default is zero. 
 *								 You can get the starting frame index by looking at the first number seperated 
 *								 by the underscore "_" in the atlas' name
 *		- Frame Count : Number of frames used in this model. Unless a light field animation is used, it must be 
 *						left as the default value of 1. Light field animation is experimental.
 *		- FPS : The FPS of the light field animation. It must be left as 12 unless a light field animation is used. 
 *				Light field animation is experimental.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HorizontalLightFieldModel : BillBoardModel
{
	public string atlasBaseName = "atlas";
	public float captureDistanceImageSizeRatio = 2.47032068094f;
	[HideInInspector]
	public int atlasCount = 10;

	public int startingFrameIndex = 0;
	public int frameCount = 1;
	public float fps = 12;

	void SetShaderParams(float surfaceSize)
	{
		for (int i = 0; i < atlasCount; i ++) {

			int frameIdx = startingFrameIndex + (Mathf.RoundToInt (Time.time * fps) % frameCount);
			Texture2D atlas = Resources.Load(atlasBaseName  + frameIdx + "_" + i) as Texture2D;
			GetComponent<Renderer>().material.SetTexture ("_Atlas" + i, atlas);
		}

		float totalRotation = rotationOffset + currentRotation;
		float netRotation = totalRotation - Mathf.Floor (totalRotation / 360f);
		GetComponent<Renderer>().material.SetFloat ("_ViewAngle", netRotation*Mathf.PI/180f);
		GetComponent<Renderer>().material.SetFloat ("_SurfaceSize", surfaceSize);
		GetComponent<Renderer>().material.SetFloat ("_CaptureDistanceSizeRatio", captureDistanceImageSizeRatio);
		GetComponent<Renderer>().material.SetFloat ("_ImagesPerTile", Mathf.Floor (360 / atlasCount));
	}

    void OnWillRenderObject()
    {
		SetShaderParams (transform.localScale.y);
    }
}