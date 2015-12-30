using UnityEngine;
using System.Collections;

public class WhirlwindBelt : MonoBehaviour {
	[Range(1, 5)]
	public int level;

	float radius;
	float height;
	float speed;

	float prevMouseX;

	public bool isInteractable;

	WhirlwindObject[] wwObjs;

	// Use this for initialization
	void Start () {
		height = (float)level * 2f + 1f;
		radius = height / 9f * 8f;
		speed = 1.5f;
		isInteractable = true;

		wwObjs = GetComponentsInChildren<WhirlwindObject>();
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
			wwObjs[i].radius = radius;
		}
	}


	// change orbiting speed of this belt
	void OnMouseDrag () {
		if (isInteractable) {
			float mouseX = Input.mousePosition.x;
			float d = mouseX - prevMouseX;
			prevMouseX = mouseX;
			float s = Mathf.Max(Mathf.Min(Mathf.Abs(d/10f), 5f), 1f);

			if (wwObjs[0].currentState == WhirlwindObject.State.Orbit) {
				for (int i = 0; i < wwObjs.Length; i++) {
					wwObjs[i].speed = speed * s;
					wwObjs[i].direction = d > 1f ? 1f : -1f;
					
					if (Mathf.Abs(d) > 1f) {
						wwObjs[i].speed = speed * s;
					} else {
						wwObjs[i].speed = 0f;
					}
				}
			}
		}
	}


	public void StirUp () {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].FlyToOrbit();
		}
	}


	public void SlowToStop () {

	}



	public void Freeze () {
		isInteractable = false;
	}


	public void UnFreeze () {
		isInteractable = true;
	}


	public void End () {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].FlyToDormant();
		}
	}
	

	void OnmouseDown () {
		prevMouseX = Input.mousePosition.x;
	}


	public void ComputeState () {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].ComputeState();
		}
	}

	
	// Update is called once per frame
	void FixedUpdate () {
	
	}
}
