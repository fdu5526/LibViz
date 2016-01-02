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

	Vector2 shiftNextPoint, shiftPrevPoint;

	Transform center;
	List<WhirlwindObject> wwObjs;
	int headIndex, tailIndex;
	WhirlwindBeltSlot[] slots;

	// Use this for initialization
	void Start () {
		center = GameObject.Find("WhirlwindCenter").transform;
		height = transform.position.y;
		radius = height / 2f + 1f;
		speed = 50f;
		isInteractable = false;
		numOfObjectsShownOnBelt = 3 + level * 2;

		WhirlwindObject[] w = GetComponentsInChildren<WhirlwindObject>();
		Debug.Assert(w != null && w.Length > 0);
		
		// find all the objects of this belt
		wwObjs = new List<WhirlwindObject>(w);
		wwObjs.Sort(delegate(WhirlwindObject w1, WhirlwindObject w2) { return w1.name.CompareTo(w2.name); });
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
		}

		// initialize the slots
		slots = new WhirlwindBeltSlot[numOfObjectsShownOnBelt];
		float deltaDegree = (360f / (float)(numOfObjectsShownOnBelt));
		for (int i = 0; i < numOfObjectsShownOnBelt; i++) {
			GameObject g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltSlot"));
			slots[i] = g.GetComponent<WhirlwindBeltSlot>();

			float t = deltaDegree * (float)(numOfObjectsShownOnBelt - i) * Mathf.Deg2Rad;
			Vector3 v = new Vector3(center.position.x + radius * Mathf.Cos(t),
															height,
															center.position.z + radius * Mathf.Sin(t));
			slots[i].Initialize(v, this, height, radius);
		}
		
		// TODO compute shiftPrevPoint also
		float theta = 270f * Mathf.Deg2Rad;
		Vector2 down = new Vector2(0f, -radius);
		shiftNextPoint = new Vector2(down.x * Mathf.Cos(theta) + down.y * Mathf.Sin(theta),
														down.y * Mathf.Cos(theta) - down.x * Mathf.Sin(theta));
	}


/////// private helper functions //////

	// stir up an item one at a time
	IEnumerator StaggeredStirUp () {
		// the number of items stirred up is based on radius
		tailIndex = Mathf.Min(numOfObjectsShownOnBelt, wwObjs.Count);
		headIndex = 0;
		int slotIndex = 0;

		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].StirUp(speed, slots[slotIndex].transform);
				slotIndex++;
				yield return new WaitForSeconds(0.3f);
			} else {
				yield return null;
			}
		}
	}

	// to handle wrapping around of indices in array
	bool IndexIsInSlots (int i) {
		return (headIndex <= i && i < tailIndex) || 
					  (tailIndex < headIndex && (i >= headIndex || i < tailIndex));
	}


	// helper function for wrap around indices
	int PrevIndex (int index) {
		int i = index - 1;
		return i < 0 ? wwObjs.Count - 1 : i;
	}

	// wrap around shifting indices
	int ShiftIndexByDirection (int index, int direction) {
		Debug.Assert(direction == -1 || direction == 1);

		int i = index + direction;
		if (direction > 0) {
			return i > wwObjs.Count - 1 ? 0 : i;
		} else {
			return i < 0 ? wwObjs.Count - 1 : i;
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
			
			// ignore extraneous input
			if (Mathf.Abs(d) < 1f) {
				return;
			}

			float direction = d > 0f ? 1f : -1f;
			int di = (int)direction;
			float s = Mathf.Min(Mathf.Abs(d), 50f);
			s = s > 1f ? s : 0f;
			prevMouseX = mouseX;

			// check for shifting the contents of the belt
			if (ShouldShift(di)) { // at the edge
				ShiftByOne(di);
			}
					
			// actually spin the belt here
			for (int i = 0; i < wwObjs.Count; i++) {
				if (IndexIsInSlots(i)) {
					if (wwObjs[i].IsInContextExam) { // only spin what should be spun
						wwObjs[i].speed = s;
					}
				}
			}

			// actually spin the belt here
			for (int i = 0; i < slots.Length; i++) {
				slots[i].direction = direction;
				slots[i].speed = s;
			}
		}
	}

	// is the belt in a position such that we need to shift?
	bool ShouldShift (int direction) {
		bool needToShift = wwObjs.Count > slots.Length;
		Vector3 p = wwObjs[headIndex].transform.position;
		Vector2 p2 = new Vector2(p.x, p.z);
		bool canShiftNext = direction > 0 && (p2 - shiftNextPoint).sqrMagnitude < 10f;
		p = wwObjs[PrevIndex(tailIndex)].transform.position;
		p2 = new Vector2(p.x, p.z);
		bool canShiftPrev = direction < 0 && (p2 - shiftNextPoint).sqrMagnitude < 10f;
		
		return needToShift && (canShiftPrev || canShiftNext);
	}

	// check if we are within bounds to shift in and out items
	bool CanShift (int direction) {
		bool isNotOutOfBounds = headIndex + direction >= 0 && tailIndex + direction <= wwObjs.Count;
		return isNotOutOfBounds;
	}


/////// public functions for setting whirlwindObject state //////
	
	// stir up objects, but stagger them so they have spaces in between them
	public void StirUp () {
		for (int i = 0; i < slots.Length; i++) {
			slots[i].StirUp();
		}
		StartCoroutine(StaggeredStirUp());
	}

	// shift to the left or right by one
	public void ShiftByOne (int direction) {
		Debug.Assert(direction == -1 || direction == 1);

		if (direction == 1) { // shift next
			Transform slot = wwObjs[headIndex].slot;
			Debug.Assert(slot != null);

			wwObjs[headIndex].End();
			wwObjs[tailIndex].StirUpByShift(speed, slot);
		} else { 							// shift prev
			Transform slot = wwObjs[PrevIndex(tailIndex)].slot;
			Debug.Assert(slot != null);

			wwObjs[PrevIndex(tailIndex)].End();
			wwObjs[PrevIndex(headIndex)].StirUpByShift(speed, slot);
		}

		headIndex = ShiftIndexByDirection(headIndex, direction);
		tailIndex = ShiftIndexByDirection(tailIndex, direction);
	}

	// slow down initial spin
	public void SlowToStop () {
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].SlowToStop();
			}
		}
		for (int i = 0; i < slots.Length; i++) {
			slots[i].SlowToStop();
		}
	}

	// is able to interact
	public void ContextExam () {
		isInteractable = true;
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].ContextExam();
			}
		}
	}

	// end the entire belt
	public void End () {
		isInteractable = false;
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].End();
			}
		}
	}

	// when a belt's items are all returned to position, reset them to a stack
	public void ResetToIdle () {
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].ResetToIdle();
			}
		}
	}

	// freeze the entire belt from moving
	public void Freeze () {
		isInteractable = false;
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].Freeze();
			}
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		isInteractable = true;
	}

	// update all the ones that are in slots
	public void ComputeState (Whirlwind.State currentState) {
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].ComputeState();
			}
		}
	}


/////// inherited from MonoBehaviour //////
	// FixedUpdate is called at set intervals
	void FixedUpdate () { }
}
