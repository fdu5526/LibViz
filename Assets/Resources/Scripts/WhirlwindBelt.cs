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
	WhirlwindBeltEnd beltEnd;
	
	// Use this for initialization
	void Start () {
		GameObject g;

		center = GameObject.Find("WhirlwindCenter").transform;
		height = transform.position.y;
		radius = height * 0.6f + 1f;
		speed = 50f;
		isInteractable = false;
		numOfObjectsShownOnBelt = 3 + level * 2;

		WhirlwindObject[] w = GetComponentsInChildren<WhirlwindObject>();
		Debug.Assert(w != null && w.Length > 0);
		
		// find all the objects of this belt
		wwObjs = new List<WhirlwindObject>(w);
		wwObjs.Sort(delegate(WhirlwindObject w1, WhirlwindObject w2) { return w1.name.CompareTo(w2.name); });
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].Initialize(this, speed, height);
		}

		// initialize the slots
		slots = new WhirlwindBeltSlot[numOfObjectsShownOnBelt];
		float deltaDegree = (360f / (float)(numOfObjectsShownOnBelt));
		for (int i = 0; i < numOfObjectsShownOnBelt; i++) {
			g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltSlot"));
			slots[i] = g.GetComponent<WhirlwindBeltSlot>();

			float t = deltaDegree * (float)(numOfObjectsShownOnBelt - i) * Mathf.Deg2Rad;
			Vector3 v = new Vector3(center.position.x + radius * Mathf.Cos(t),
															height,
															center.position.z + radius * Mathf.Sin(t));
			slots[i].Initialize(v, height, radius);
		}
		
		// the end point of the belt that causes shifting
		g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltEnd"));
		g.transform.parent = transform;
		g.transform.position = transform.position + new Vector3(radius, 0f, 0f);
		beltEnd = g.GetComponent<WhirlwindBeltEnd>();
		beltEnd.belt = this;
	}


	float RandomStirUpWaitTime { get { return UnityEngine.Random.Range(0.4f, 1f) / (float)BeltSize; } }


/////// private helper functions //////
	// stir up an item one at a time
	IEnumerator StaggeredStirUp () {
		// the number of items stirred up is based on radius
		tailIndex = Mathf.Max(BeltSize - 1, 0);
		headIndex = 0;
		int slotIndex = 0;

		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].StirUp(speed, slots[slotIndex].transform);
				slotIndex++;
				yield return new WaitForSeconds(RandomStirUpWaitTime);
			} else {
				yield return null;
			}
		}
	}

	// to handle wrapping around of indices in array
	bool IndexIsInSlots (int i) {
		return (headIndex <= i && i <= tailIndex) || 
					  (tailIndex < headIndex && (i >= headIndex || i <= tailIndex));
	}


	// helper function for wrap around indices
	int PrevIndex (int index) {
		int i = index - 1;
		return i < 0 ? wwObjs.Count - 1 : i;
	}

	int NextIndex (int index) {
		return (index + 1) % wwObjs.Count;
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

	int BeltSize {
		get {
			return Mathf.Min(numOfObjectsShownOnBelt, wwObjs.Count);
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
			float s = Mathf.Min(Mathf.Abs(d), 50f);
			s = s > 1f ? s : 0f;
			prevMouseX = mouseX;

			// actually spin the belt here
			for (int i = 0; i < slots.Length; i++) {
				slots[i].direction = direction;
				slots[i].speed = s;
			}

			for (int i = 0; i < wwObjs.Count; i++) {
				if (IndexIsInSlots(i)) {
					wwObjs[i].isBeltBeingDragged = true;
			}
		}
		}
	}

/////// public functions for setting whirlwindObject state //////
	
	public bool IsAtHead (Transform slot) {
		bool isHead = (slot.position - wwObjs[headIndex].slot.position).sqrMagnitude < 1f;
		return isHead;
	}


	public bool IsAtTail (Transform slot) {
		bool isTail = (slot.position - wwObjs[tailIndex].slot.position).sqrMagnitude < 1f;
		return isTail;
	}

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

			if (wwObjs[NextIndex(tailIndex)].IsInWhirlwind) {
				return;
			}

			wwObjs[headIndex].End();
			wwObjs[NextIndex(tailIndex)].StirUpByShift(speed, slot);
		} else { 							// shift prev
			Transform slot = wwObjs[tailIndex].slot;
			Debug.Assert(slot != null);

			if (wwObjs[PrevIndex(headIndex)].IsInWhirlwind) {
				return;
			}

			wwObjs[tailIndex].End();
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
		beltEnd.isInContextExam = true;

		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].ContextExam();
			}
		}
	}

	// end the entire belt
	public void End () {
		isInteractable = false;
		beltEnd.isInContextExam = false;
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].End();
			}
		}
	}

	// when a belt's items are all returned to position, reset them to a stack
	public void ResetToIdle () {
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].ResetToIdle();
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

		for (int i = 0; i < slots.Length; i++) {
			slots[i].Freeze();
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		isInteractable = true;
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].UnFreeze();
			}
		}
	}

	// update all the ones that are in slots
	public void ComputeState (Whirlwind.State currentState) {
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].ComputeState();
		}
	}


/////// inherited from MonoBehaviour //////
	// FixedUpdate is called at set intervals
	void FixedUpdate () { }
}
