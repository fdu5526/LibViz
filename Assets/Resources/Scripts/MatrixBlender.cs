using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class MatrixBlender : MonoBehaviour
{

	Camera theCamera;
	void Start () {
		theCamera = GetComponent<Camera>();
	}

	public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
	{
		Matrix4x4 ret = new Matrix4x4();
		for (int i = 0; i < 16; i++){
			ret[i] = Mathf.Lerp(from[i], to[i], time);
		}
		return ret;
	}
	
	private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration, bool isSrcOrthographic)
	{
		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			float f = 1f - (Mathf.Log((Time.time - startTime) / duration)/Mathf.Log(2) / -10f);
			if (!isSrcOrthographic) {
				f = Mathf.Pow(2f, (Time.time - startTime) / duration * 10f - 10f);
			}
			theCamera.projectionMatrix = MatrixLerp(src, dest, f);

			yield return 1;
		}
		theCamera.projectionMatrix = dest;
	}
	
	public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration, bool isSrcOrthographic)
	{
		StopAllCoroutines();
		return StartCoroutine(LerpFromTo(theCamera.projectionMatrix, targetMatrix, duration, isSrcOrthographic));
	}
}