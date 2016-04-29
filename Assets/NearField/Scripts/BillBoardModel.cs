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
 * BillBoardModel represents a generic near field model that projects a raster to the camera. 
 * The raster could be generated using a renderer like simple sprite renderer, light-field renderer, etc.
 */

using UnityEngine;
using System.Collections;

public class BillBoardModel : MonoBehaviour {

	[HideInInspector]
	public float billboardFrameIndex = 0;

	public float rotationOffset = 90f;
	public float currentRotation = 0f;
	
	public bool showCameraBounds = false;
	public float cameraVerticalFOV = 27.0f;
	public float captureAngularDepression = 0f;
	public float tolerableAngularError = 20f;

	public void Rotate (float rotation)
	{
		currentRotation += rotation;
	}
	
	public void SetRotation (float rotation)
	{
		currentRotation = rotation;
	}

	//TODO: Add a helper method bool IsCameraInBounds(Vector3 cameraPosition) to determine 
	//if the camera is in valid bounds to render the model without skewing artifacts.
	
	void OnDrawGizmos () {
		
		if (!showCameraBounds) return;
		
		Gizmos.color = Color.green;
		Vector3 originalPosition = transform.position;
		Vector3 originalScale = transform.localScale;
		Quaternion originalRotation = transform.rotation;
		
		
		float alpha = cameraVerticalFOV * Mathf.PI / 180f;
		float theta = captureAngularDepression * Mathf.PI / 180f;
		float delta = tolerableAngularError * Mathf.PI / 180f / 2f;
		
		float theta1 = Mathf.Atan ((Mathf.Sin (theta) + Mathf.Tan (alpha / 2f) * Mathf.Tan (theta) * Mathf.Sin (theta) - Mathf.Tan (alpha / 2f) / Mathf.Cos (theta)) /
		                           ((1f + Mathf.Tan (alpha / 2f) * Mathf.Tan (theta)) * Mathf.Cos (theta)));
		float theta2 = Mathf.Atan ((Mathf.Sin (theta) + Mathf.Tan (alpha / 2f) * Mathf.Tan (theta) * Mathf.Sin (theta) - Mathf.Tan (alpha / 2f) + 2f * Mathf.Tan (alpha / 2f) * Mathf.Cos (theta)) / 
		                           (Mathf.Cos (theta) + Mathf.Tan (alpha / 2f) * Mathf.Sin (theta) - Mathf.Tan (alpha / 2f) * Mathf.Tan (theta) / Mathf.Cos (theta) ));
		
		float gamma1 = theta1 - delta;
		float gamma2 = theta2 + delta;
		
		float l1 = Mathf.Abs (transform.localScale.y / (Mathf.Sin (gamma1) - Mathf.Cos (gamma1) * Mathf.Tan (gamma2)));
		float s = l1 * Mathf.Cos (gamma1) / Mathf.Cos (theta);
		
		//Approximate vertical offset, assuming the center pixel correspond to the vertical center on the object
		float verticalOffset = s * Mathf.Sin (theta);
		float horizonalOffset = s * Mathf.Cos (theta);
		float frustumAngle = theta2 - theta1 + 2f * delta;
		float frustumLookAt = theta2 + delta - frustumAngle / 2f;
		
		transform.position += new Vector3 (0, verticalOffset, horizonalOffset);
		transform.Rotate (Vector3.left, frustumLookAt * 180f / Mathf.PI);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawFrustum (Vector3.zero, frustumAngle * 180f / Mathf.PI, 1000, 0, 1f / Mathf.Tan (frustumAngle / 2));
		transform.position = originalPosition;
		transform.localScale = originalScale;
		transform.rotation = originalRotation;
		
		transform.position += new Vector3 (0, verticalOffset, -horizonalOffset);
		transform.Rotate (Vector3.up, 180);
		transform.Rotate (Vector3.left, frustumLookAt * 180f / Mathf.PI);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawFrustum (Vector3.zero, frustumAngle * 180f / Mathf.PI, 1000, 0, 1f / Mathf.Tan (frustumAngle / 2));
		transform.position = originalPosition;
		transform.localScale = originalScale;
		transform.rotation = originalRotation;
		
		transform.position += new Vector3 (horizonalOffset, verticalOffset, 0);
		transform.Rotate (Vector3.up, 90);
		transform.Rotate (Vector3.left, frustumLookAt * 180f / Mathf.PI);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawFrustum (Vector3.zero, frustumAngle * 180f / Mathf.PI, 1000, 0, 1f / Mathf.Tan (frustumAngle / 2));
		transform.position = originalPosition;
		transform.localScale = originalScale;
		transform.rotation = originalRotation;
		
		transform.position += new Vector3 (-horizonalOffset, verticalOffset, 0);
		transform.Rotate (Vector3.up, 270);
		transform.Rotate (Vector3.left, frustumLookAt * 180f / Mathf.PI);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawFrustum (Vector3.zero, frustumAngle * 180f / Mathf.PI, 1000, 0, 1f / Mathf.Tan (frustumAngle / 2));
		transform.position = originalPosition;
		transform.localScale = originalScale;
		transform.rotation = originalRotation;
		Gizmos.color = Color.red;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireSphere (Vector3.zero, horizonalOffset);
	}
}
