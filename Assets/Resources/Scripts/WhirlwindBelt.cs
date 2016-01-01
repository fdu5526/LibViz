using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WhirlwindBelt : MonoBehaviour {
	[Range(0, 5)]
	public int level;
	int numOfObjectsShownOnBelt;

	float radius;
	float height;
	float speed;

	float prevMouseX;

	bool isInteractable;

	Vector2 exitPoint;

	Transform center;
	List<WhirlwindObject> wwObjs;
	int headIndex, tailIndex;
	WhirlwindBeltMarker[] markers;

	// Use this for initialization
	void Start () {
		center = GameObject.Find("WhirlwindCenter").transform;
		height = transform.position.y;
		radius = height / 10f * 5f + 1f; // TODO better radius formula

		speed = 50f;
		isInteractable = false;
		numOfObjectsShownOnBelt = 3 + level * 2;

		WhirlwindObject[] w = GetComponentsInChildren<WhirlwindObject>();
		Debug.Assert(w != null);
		
		// find all the objects of this belt
		wwObjs = new List<WhirlwindObject>(w);
		wwObjs.Sort(delegate(WhirlwindObject w1, WhirlwindObject w2) { return w1.name.CompareTo(w2.name); });
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
			wwObjs[i].radius = radius;
		}

		// initialize the markers
		markers = new WhirlwindBeltMarker[numOfObjectsShownOnBelt];
		for (int i = 0; i < numOfObjectsShownOnBelt; i++) {
			GameObject g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltMarker"));
			markers[i] = g.GetComponent<WhirlwindBeltMarker>();

			float t = (360f / (float)(numOfObjectsShownOnBelt)) * (float)i / 180f * Mathf.PI;
			Vector3 v = new Vector3(center.position.x + radius * Mathf.Cos(t),
															height,
															center.position.z + radius * Mathf.Sin(t));
			markers[i].Initialize(v, this, height, radius);
		}
		
		float theta = 90f / 180f * Mathf.PI;
		Vector2 down = new Vector2(0f, -radius);
		exitPoint = new Vector2(down.x * Mathf.Cos(theta) + down.y * Mathf.Sin(theta),
														down.y * Mathf.Cos(theta) - down.x * Mathf.Sin(theta));
	}

	// stir up an item one at a time
	IEnumerator StaggeredStirUp () {
		// the number of items stirred up is based on radius
		tailIndex = Mathf.Min(numOfObjectsShownOnBelt, wwObjs.Count);
		headIndex = 0;

		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].StirUp(speed);
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

	// spin the belt
	public void Spin () {
		if (isInteractable) {
			float mouseX = Input.mousePosition.x;
			float d = mouseX - prevMouseX;
			float direction = d > 0f ? 1f : -1f;
			int di = (int)direction;
			float s = Mathf.Min(Mathf.Abs(d), 50f);
			s = s > 1f ? s : 0f;
			prevMouseX = mouseX;
/*
			// check for shifting the contents of the belt
			if (ShouldShift(di)) { // at the edge
				if (CanShift(di)) {
					ShiftByOne(di);
					// TODO should it loop here?
				} else {
					s = 0f;
				}
			}
					
			// actually spin the belt here
			for (int i = headIndex; i < tailIndex; i++) {
				if (wwObjs[i].IsInContextExam) { // only spin what should be spun
					if (Mathf.Abs(d) > 1f) {
						wwObjs[i].direction = direction;
					}
					wwObjs[i].speed = s;
				}
			}
*/
			// actually spin the belt here
			for (int i = 0; i < markers.Length; i++) {
				markers[i].direction = direction;
				markers[i].speed = s;
			}
		}
	}

	// is the belt in a position such that we need to shift?
	bool ShouldShift (int direction) {
		Vector3 p = wwObjs[headIndex].transform.position;
		Vector2 p2 = new Vector2(p.x, p.z);
		bool canShiftNext = direction > 0 && (p2 - exitPoint).sqrMagnitude < 10f;
		p = wwObjs[tailIndex - 1].transform.position;
		p2 = new Vector2(p.x, p.z);
		bool canShiftPrev = direction < 0 && (p2 - exitPoint).sqrMagnitude < 10f;
		
		//return Input.GetKeyDown("f");
		return canShiftPrev || canShiftNext;
	}

	// check if we are within bounds to shift in and out items
	bool CanShift (int direction) {
		bool isNotOutOfBounds = headIndex + direction >= 0 && tailIndex + direction <= wwObjs.Count;
		return isNotOutOfBounds;
	}


/////// public functions for setting whirlwindObject state //////
	
	// stir up objects, but stagger them so they have spaces in between them
	public void StirUp () {
		for (int i = 0; i < markers.Length; i++) {
			markers[i].StirUp();
		}
		StartCoroutine(StaggeredStirUp());
	}

	// shift to the left or right by one
	public void ShiftByOne (int direction) {
		Debug.Assert(CanShift(direction));
		Debug.Assert(direction == -1 || direction == 1);

		if (direction == 1) { // shift next
			wwObjs[headIndex].EndByShift();
			wwObjs[tailIndex].StirUpByShift(speed);
		} else {
			wwObjs[headIndex - 1].StirUpByShift(speed);
			wwObjs[tailIndex - 1].EndByShift();
		}

		headIndex += direction;
		tailIndex += direction;
	}

	// slow down initial spin
	public void SlowToStop () {
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].SlowToStop();
		}
		for (int i = 0; i < markers.Length; i++) {
			markers[i].SlowToStop();
		}
	}

	// is able to interact
	public void ContextExam () {
		isInteractable = true;
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].ContextExam();
		}
	}

	// end the entire belt
	public void End () {
		isInteractable = false;
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].End();
		}
	}

	// when a belt's items are all returned to position, reset them to a stack
	public void ResetToIdle () {
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].ResetToIdle();
		}
	}

	// freeze the entire belt from moving
	public void Freeze () {
		isInteractable = false;
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].Freeze();
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		isInteractable = true;
	}

	public void ComputeState (Whirlwind.State currentState) {
		for (int i = headIndex; i < tailIndex; i++) {
			wwObjs[i].ComputeState();
		}
	}


/////// inherited from MonoBehaviour //////
	// FixedUpdate is called at set intervals
	void FixedUpdate () { }
}
