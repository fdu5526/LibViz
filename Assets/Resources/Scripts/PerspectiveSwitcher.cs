using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MatrixBlender))]
public class PerspectiveSwitcher : MonoBehaviour
{
	
	public float fov = 60f;
	public float near = 0.3f;
	public float far = 1000f;
	public float orthographicSize = 50f;

	Camera camera;
	Matrix4x4 ortho;
	Matrix4x4 perspective;
	float aspect;
	MatrixBlender blender;
	bool orthoOn;
	
	void Start()
	{
		camera = GetComponent<Camera>();
		aspect = (float) Screen.width / (float) Screen.height;
		ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
		perspective = Matrix4x4.Perspective(fov, aspect, near, far);
		camera.projectionMatrix = perspective;
		orthoOn = false;
		blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));
	}
	
	public void switchPerspective(){
		if(true)
		{
			orthoOn = !orthoOn;
			if (orthoOn){

				blender.BlendToMatrix(ortho, 1f, true);
			}
			else{
				blender.BlendToMatrix(perspective, 1f, false);
			}
		}

		else{
			Debug.Log("Button is not ready yet");
		}
	}

    public void Activate()
    {
        switchPerspective();
    }
}