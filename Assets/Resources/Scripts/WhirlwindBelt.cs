using UnityEngine;
using System.Collections;

public class WhirlwindBelt : MonoBehaviour {
	[Range(1, 5)]
	public int level;

	float radius;
	float height;
	float speed;

	float prevMouseX;

	bool isTakingUserInput;

	WhirlwindObject[] wwObjs;
	int headIndex, tailIndex;

	// Use this for initialization
	void Start () {
		height = transform.position.y;
		radius = height / 9f * 8f; // TODO better radius formula
		speed = 1.5f;
		isTakingUserInput = false;

		wwObjs = GetComponentsInChildren<WhirlwindObject>();
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
			wwObjs[i].radius = radius;
		}
	}

	// stir up an item one at a time
	IEnumerator StaggeredStirUp () {
		// TODO the amount should be based on radius
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].StirUp();
			yield return new WaitForSeconds(0.2f + 0.2f * (float)level);
		}
	}

/////// public functions used by whirlwindObjects //////
	
	// for when user initially places mouse down to drag it
	public void SetMouseDownPosition () {
		if (isTakingUserInput) {
			prevMouseX = Input.mousePosition.x;
		}
	}

	// spin the entire belt
	public void Spin () {
		if (isTakingUserInput) {
			float mouseX = Input.mousePosition.x;
			float d = mouseX - prevMouseX;
			prevMouseX = mouseX;
			//float s = Mathf.Max(Mathf.Min(Mathf.Abs(d/10f), 20f), 1f);
			float s = Mathf.Min(Mathf.Abs(d), 50f);

			for (int i = 0; i < wwObjs.Length; i++) {
				wwObjs[i].direction = d > 0f ? 1f : -1f;
				wwObjs[i].speed = s > 1f ? s : 0f;
			}
		}
	}


/////// public functions for setting whirlwindObject state //////
	
	// stir up objects, but stagger them so they have spaces in between them
	public void StirUp () {
		StartCoroutine(StaggeredStirUp());
	}

	public void SlowToStop () {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].SlowToStop();
		}
	}

	// is able to interact
	public void CanInteract () {
		isTakingUserInput = true;
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].CanInteract();
		}
	}

	// end the entire belt
	public void End () {
		isTakingUserInput = false;
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].End();
		}
	}

	// freeze the entire belt from moving
	public void Freeze () {
		isTakingUserInput = false;
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].Freeze();
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		isTakingUserInput = true;
	}

	public void ComputeState (Whirlwind.State currentState) {
		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].ComputeState();
		}
	}


/////// inherited from MonoBehaviour //////

	
	// Update is called once per frame
	void FixedUpdate () {
	
	}
}
