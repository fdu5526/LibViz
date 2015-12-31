using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhirlwindBelt : MonoBehaviour {
	[Range(0, 5)]
	public int level;

	float radius;
	float height;
	float speed;

	float prevMouseX;

	bool isInteractable;

	List<WhirlwindObject> wwObjs;
	int headIndex, tailIndex;

	// Use this for initialization
	void Start () {
		height = transform.position.y;
		radius = height / 10f * 5f + 1f; // TODO better radius formula
		speed = 50f;
		isInteractable = false;

		WhirlwindObject[] w = GetComponentsInChildren<WhirlwindObject>();
		wwObjs = new List<WhirlwindObject>(w);
		wwObjs.Sort(delegate(WhirlwindObject w1, WhirlwindObject w2) { return w1.name.CompareTo(w2.name); });
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
			wwObjs[i].radius = radius;
		}
	}

	// stir up an item one at a time
	IEnumerator StaggeredStirUp () {
		headIndex = 0;
		tailIndex = 3 + level * 2;

		Debug.Assert(tailIndex <= wwObjs.Count );

		// TODO the amount should be based on radius
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].StirUp();
			yield return new WaitForSeconds(0.4f);
		}
	}

/////// public functions used by whirlwindObjects //////
	
	// for when user initially places mouse down to drag it
	public void SetMouseDownPosition () {
		if (isInteractable) {
			prevMouseX = Input.mousePosition.x;
		}
	}

	// spin the entire belt
	public void Spin () {
		if (isInteractable) {
			float mouseX = Input.mousePosition.x;
			float d = mouseX - prevMouseX;
			prevMouseX = mouseX;
			//float s = Mathf.Max(Mathf.Min(Mathf.Abs(d/10f), 20f), 1f);
			float s = Mathf.Min(Mathf.Abs(d), 50f);

			for (int i = 0; i < wwObjs.Count; i++) {
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
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].SlowToStop();
		}
	}

	// is able to interact
	public void CanInteract () {
		isInteractable = true;
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].CanInteract();
		}
	}

	// end the entire belt
	public void End () {
		isInteractable = false;
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].End();
		}
	}

	public void ResetToIdle () {
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].ResetToIdle();
		}
	}

	// freeze the entire belt from moving
	public void Freeze () {
		isInteractable = false;
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].Freeze();
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		isInteractable = true;
	}

	public void ComputeState (Whirlwind.State currentState) {
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].ComputeState();
		}
	}


/////// inherited from MonoBehaviour //////

	
	// Update is called once per frame
	void FixedUpdate () {
	
	}
}
