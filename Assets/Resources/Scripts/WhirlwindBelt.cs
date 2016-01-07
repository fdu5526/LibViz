using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WhirlwindBelt : MonoBehaviour {
	[Range(0, 5)]
	public int level;
	int numOfObjectsShownOnBelt;

	float radius;
	float height;

	float prevMouseX;

	bool isOperating;

	Vector2 shiftNextPoint, shiftPrevPoint;
	bool isSlowingDown;
	bool isTransitioningToContextExam;

	Transform center;
	List<WhirlwindObject> wwObjs;
	int headIndex, tailIndex;
	WhirlwindBeltSlot[] slots;
	WhirlwindBeltEnd beltEnd;
	WhirlwindBeltLabel label;
	
	// Use this for initialization
	void Start () {
		GameObject g;

		center = GameObject.Find("WhirlwindCenter").transform;
		height = transform.position.y;
		radius = height * 0.6f + 1f;
		isOperating = false;
		numOfObjectsShownOnBelt = 3 + level * 3;

		WhirlwindObject[] w = GetComponentsInChildren<WhirlwindObject>();
		Debug.Assert(w != null);
		
		// find all the objects of this belt
		wwObjs = new List<WhirlwindObject>(w);
		wwObjs.Sort(delegate(WhirlwindObject w1, WhirlwindObject w2) { return w1.name.CompareTo(w2.name); });
		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].Initialize(this, radius, height);
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
			slots[i].transform.parent = transform;
		}

		// get the label
		label = transform.Find("Label").GetComponent<WhirlwindBeltLabel>();
		label.transform.position = transform.position + new Vector3(-radius - (float)(level * 1f) - 2f, 0f, 0f);
		label.SetToTransparent();
		
		// the end point of the belt that causes shifting
		g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltEnd"));
		g.transform.parent = transform;
		g.transform.position = transform.position + new Vector3(radius, 0f, 0f);
		beltEnd = g.GetComponent<WhirlwindBeltEnd>();
		beltEnd.belt = this;
		beltEnd.GetComponent<Collider>().enabled = false;
	}



/////// private helper functions //////
	// stir up an item one at a time
	IEnumerator StaggeredStirUp (float speed) {
		// the number of items stirred up is based on radius
		tailIndex = Mathf.Max(BeltSize - 1, 0);
		headIndex = 0;
		int slotIndex = 0;
		float t = 1f / (float)BeltSize;

		for (int i = 0; i < wwObjs.Count; i++) {
			if (!isOperating) {
				yield break;
			}

			if (IndexIsInSlots(i)) {
				wwObjs[i].StirUp(speed, slots[slotIndex].transform);
				slotIndex++;
				yield return new WaitForSeconds(t);
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


	// helper functions for wrap around indices
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

	// max number of stuffs on the belt at a time
	int BeltSize {
		get {
			return Mathf.Min(numOfObjectsShownOnBelt, wwObjs.Count);
		}
	}

	// belt has reached optimal position, slow all objects and slots
	void SlowAllToStop (bool isFastStop) {
		isSlowingDown = false;
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].SlowToStop();
			}
		}
		for (int i = 0; i < slots.Length; i++) {
			slots[i].SlowToStop(isFastStop);
		}
	}

/////// public functions used for user interaction //////
	
	// for when user initially places mouse down to drag it
	public void SetMouseDownPosition () {
		prevMouseX = Input.mousePosition.x;
	}

	// spin the belt
	public void Spin () {
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

/////// public functions used by state transition //////
	
	public bool IsAtHead (Transform slot) {
		bool isHead = (slot.position - wwObjs[headIndex].slot.position).sqrMagnitude < 1f;
		return isHead;
	}


	public bool IsAtTail (Transform slot) {
		bool isTail = (slot.position - wwObjs[tailIndex].slot.position).sqrMagnitude < 1f;
		return isTail;
	}

	// stir up objects, but stagger them so they have spaces in between them
	public void StirUp (float speed, bool shouldLoadObjects) {
		isOperating = true;
		label.Fade(isOperating);
		for (int i = 0; i < slots.Length; i++) {
			slots[i].StirUp();
		}
		if (shouldLoadObjects) {
			StartCoroutine(StaggeredStirUp(speed));
		}
	}

	// whether all the slots are filled
	public bool IsDoneStirUp { 
		get {
			bool allDone = true;
			for (int i = 0; i < wwObjs.Count; i++) {
				if (IndexIsInSlots(i)) {
					allDone &= wwObjs[i].IsDoneStirUp;
				}
			}
			return allDone;
		}
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
			wwObjs[NextIndex(tailIndex)].StirUpByShift(100f, slot);
		} else { 							// shift prev
			Transform slot = wwObjs[tailIndex].slot;
			Debug.Assert(slot != null);

			if (wwObjs[PrevIndex(headIndex)].IsInWhirlwind) {
				return;
			}

			wwObjs[tailIndex].End();
			wwObjs[PrevIndex(headIndex)].StirUpByShift(100f, slot);
		}

		headIndex = ShiftIndexByDirection(headIndex, direction);
		tailIndex = ShiftIndexByDirection(tailIndex, direction);
	}


	// slow down initial spin
	public void SlowToStop (bool isTransitioningToContextExam) {
		isSlowingDown = true;
		this.isTransitioningToContextExam = isTransitioningToContextExam;
		beltEnd.GetComponent<Collider>().enabled = true;

		if (!isTransitioningToContextExam) {
			SlowAllToStop(false);
		}
		
	}

	// whether all the slots are at velocity zero
	public bool IsDoneSlowingDown { 
		get {
			bool allDone = true;
			for (int i = 0; i < slots.Length; i++) {
				allDone &= slots[i].IsDoneSlowingDown;
			}
			return allDone;
		}
	}

	// is able to interact, spin the entire whirlwind
	public void WhirlExam () {
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].WhirlExam();
			}
		}
	}


	// is able to interact, spin individual belts
	public void ContextExam () {
		beltEnd.isInContextExam = true;

		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].ContextExam();
			}
		}
	}

	// end the entire belt, return all objects
	public void End () {
		isOperating = false;
		label.Fade(isOperating);
		beltEnd.GetComponent<Collider>().enabled = false;
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
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].SetInteractable(false);
			}
		}

		for (int i = 0; i < slots.Length; i++) {
			slots[i].Freeze();
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		for (int i = 0; i < wwObjs.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwObjs[i].SetInteractable(true);
			}
		}
	}

	// update all the ones that are in slots
	public void ComputeState (Whirlwind.State currentState) {
		
		// swirl belt to correct locatin
		if (isSlowingDown && isTransitioningToContextExam) {
			if (beltEnd.mostRecentCollisionIsTail) {
				SlowAllToStop(true);
			}
		}

		for (int i = 0; i < wwObjs.Count; i++) {
			wwObjs[i].ComputeState();
		}
	}


/////// inherited from MonoBehaviour //////
	// FixedUpdate is called at set intervals
	void FixedUpdate () { }
}
