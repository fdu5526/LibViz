using UnityEngine;
using System.Collections;

public class WhirlwindBelt : MonoBehaviour {
	[Range(1, 5)]
	public int level;

	float radius;
	float height;
	float speed;

	float prevMouseX;

	bool isInteractable;

	WhirlwindObject[] wwObjs;
	int headIndex, tailIndex;

	// Use this for initialization
	void Start () {
		height = transform.position.y;
		radius = height / 9f * 8f;
		speed = 1.5f;
		isInteractable = false;

		wwObjs = GetComponentsInChildren<WhirlwindObject>();
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
			wwObjs[i].radius = radius;
		}
	}


	void OnmouseDown () {
		prevMouseX = Input.mousePosition.x;
	}

	// change orbiting speed of this belt
	void OnMouseDrag () {
		if (isInteractable) {
			float mouseX = Input.mousePosition.x;
			float d = mouseX - prevMouseX;
			prevMouseX = mouseX;
			float s = Mathf.Max(Mathf.Min(Mathf.Abs(d/10f), 5f), 1f);

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


	IEnumerator StaggeredStirUp () {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].StirUp();
			yield return new WaitForSeconds(0.4f);
		}
	}


/////// public functions for setting whirlwindObject state //////
	public void StirUp () {
		StartCoroutine(StaggeredStirUp());
	}


	public void SlowToStop () {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].SlowToStop();
		}
	}


	public void End () {
		isInteractable = false;
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].End();
		}
	}


	public void Freeze () {
		isInteractable = false;
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].Freeze();
		}
	}


	public void UnFreeze () {
		isInteractable = true;
	}


	public void CanInteract () {
		isInteractable = true;
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].CanInteract();
		}
	}

	public void ComputeState (Whirlwind.State currentState) {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].ComputeState();
		}
	}

	
	// Update is called once per frame
	void FixedUpdate () {
	
	}
}
