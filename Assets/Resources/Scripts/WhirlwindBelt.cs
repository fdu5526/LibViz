using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WhirlwindBelt : MonoBehaviour {
	[Range(0, 5)]
	public int level;
	int numOfItemsShownOnBelt;

	float radius;

	float prevMouseX;

	bool isOperating;

	Vector3 defaultItemPosition;
	Vector2 shiftNextPoint, shiftPrevPoint;
	bool isSlowingDown;
	bool isTransitioningToContextExam;

	Transform center;
	Whirlwind whirlwind;
	List<WhirlwindItem> wwItems;
	int headIndex, tailIndex;
	WhirlwindBeltSlot[] slots;
	WhirlwindBeltEnd beltEnd;
	WhirlwindBeltLabel label;
	
	// Use this for initialization
	void Start () {
		GameObject g;

		float height = transform.position.y;
		radius = height * 0.5f + 1.5f;

		defaultItemPosition = new Vector3(-14.47f, 1.3f, -0.77f);
		center = GameObject.Find("WhirlwindCenter").transform;
		whirlwind = center.GetComponent<Whirlwind>();
		isOperating = false;
		numOfItemsShownOnBelt = 4 + level * 3;
		wwItems = new List<WhirlwindItem>();

		// initialize the slots
		slots = new WhirlwindBeltSlot[numOfItemsShownOnBelt];
		float deltaDegree = (360f / (float)(numOfItemsShownOnBelt));
		for (int i = 0; i < numOfItemsShownOnBelt; i++) {
			g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltSlot"));
			slots[i] = g.GetComponent<WhirlwindBeltSlot>();

			float t = deltaDegree * (float)(numOfItemsShownOnBelt - i) * Mathf.Deg2Rad;
			Vector3 v = new Vector3(center.position.x + radius * Mathf.Cos(t),
															height,
															center.position.z + radius * Mathf.Sin(t));
			slots[i].Initialize(v, radius);
			slots[i].transform.parent = transform;
		}

		// get the label
		label = transform.Find("Label").GetComponent<WhirlwindBeltLabel>();
		label.transform.position = transform.position + new Vector3(-radius - (float)(level * 0.4f) - 4.5f, -0.5f, 0f);
		label.SetToTransparent();
		
		// the end point of the belt that causes shifting
		g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindBeltEnd"));
		beltEnd = g.GetComponent<WhirlwindBeltEnd>();
		beltEnd.Initialize(transform, transform.position + new Vector3(-radius, 0f, 0f), this);
		beltEnd.Enable(false);
	}


/////// private helper functions //////
	// stir up an item one at a time
	IEnumerator StaggeredStirUp (float speed, bool transitionToWhirlExam) {
		// the number of items stirred up is based on radius
		tailIndex = Mathf.Max(BeltSize - 1, 0);
		headIndex = 0;
		int slotIndex = 0;
		float t = 1f / (float)BeltSize;

		for (int i = 0; i < wwItems.Count; i++) {
			if (!isOperating) {
				yield break;
			}

			Vector3 p = transform.position;
			float ogPosition, targetPosition;
			if (!transitionToWhirlExam) {
				ogPosition = Global.defaultBeltHeights[level];
				targetPosition = Global.contextExamBeltHeights[level];
			} else {
				ogPosition = Global.contextExamBeltHeights[level];
				targetPosition = Global.defaultBeltHeights[level];
			}
			p.y = Mathf.Lerp(ogPosition, targetPosition, (float)i/(float)(BeltSize));
			transform.position = p;

			if (IndexIsInSlots(i)) {
				wwItems[i].StirUp(speed, slots[slotIndex].transform);
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
		return i < 0 ? wwItems.Count - 1 : i;
	}
	int NextIndex (int index) {
		return (index + 1) % wwItems.Count;
	}

	// wrap around shifting indices
	int ShiftIndexByDirection (int index, int direction) {
		Debug.Assert(direction == -1 || direction == 1);

		int i = index + direction;
		if (direction > 0) {
			return i > wwItems.Count - 1 ? 0 : i;
		} else {
			return i < 0 ? wwItems.Count - 1 : i;
		}
	}

	// max number of stuffs on the belt at a time
	int BeltSize {
		get {
			return Mathf.Min(numOfItemsShownOnBelt, wwItems.Count);
		}
	}

	// belt has reached optimal position, slow all items and slots
	void SlowToStopFast (bool isFastStop) {
		isSlowingDown = false;
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].SlowToStop();
			}
		}
		for (int i = 0; i < slots.Length; i++) {
			slots[i].SlowToStopFast(isFastStop);
		}
	}

/////// public functions used for manipulating data //////
	public void LoadNewItems (WhirlwindBeltInfo infos) {
		GameObject g;

		beltEnd.Enable(false);

		// wipe old items away
		for (int i = 0; i < wwItems.Count; i++) {
			wwItems[i].SlatedToBeDestroyed();
		}

		// instantiate new items
		wwItems = new List<WhirlwindItem>();
		for (int i = 0; i < infos.Infos.Count; i++) {
			g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindItem"));
			WhirlwindItem wwi = g.GetComponent<WhirlwindItem>();
			wwi.Initialize(this, radius, defaultItemPosition, infos.Infos[i]);
			wwItems.Add(wwi);
		}
		// set the label
		label.Text = infos.Label;
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
		float s = Mathf.Min(Mathf.Abs(d), Global.StirUpSpeed);
		s = s > 1f ? s : 0f;
		prevMouseX = mouseX;

		// actually spin the belt here
		for (int i = 0; i < slots.Length; i++) {
			slots[i].direction = direction;
			slots[i].speed = s;
		}

		if (s > 0f) {
			whirlwind.isBeingSpun = true;
		}
	}

/////// public functions used by state transition //////
	
	public bool IsAtHead (Transform slot) {
		if (wwItems.Count == 0) {
			return true;
		} else {
			bool isHead = (slot.position - wwItems[headIndex].Slot.position).sqrMagnitude < 1f;
			return isHead;
		}
	}


	public bool IsAtTail (Transform slot) {
		if (wwItems.Count == 0) {
			return true;
		} else {
			bool isTail = (slot.position - wwItems[tailIndex].Slot.position).sqrMagnitude < 1f;
			return isTail;
		}
	}

	// stir up items, but stagger them so they have spaces in between them
	public void StirUp (float speed, bool transitionToWhirlExam) {
		isOperating = true;
		for (int i = 0; i < slots.Length; i++) {
			slots[i].EnableCollider(false);
			slots[i].StirUp();
		}
		StartCoroutine(StaggeredStirUp(speed, transitionToWhirlExam));
	}

	// whether all the slots are filled. Expensive operation
	public bool IsDoneStirUp { 
		get {
			bool allDone = true;
			for (int i = 0; i < wwItems.Count; i++) {
				if (IndexIsInSlots(i)) {
					allDone &= wwItems[i].IsDoneStirUp;
				}
			}
			return allDone;
		}
	}

	// shift to the left or right by one
	public void ShiftByOne (int direction) {
		Debug.Assert(direction == -1 || direction == 1);

		if (direction == 1) { // shift next
			Transform slot = wwItems[headIndex].Slot;
			Debug.Assert(slot != null);

			// if the item that is coming in is already in, do nohing
			if (wwItems[NextIndex(tailIndex)].IsInWhirlwind) {
				return;
			}

			// swap
			wwItems[headIndex].End();
			wwItems[NextIndex(tailIndex)].StirUpByShift(100f, slot);
		} else { 							// shift prev
			Transform slot = wwItems[tailIndex].Slot;
			Debug.Assert(slot != null);

			// if the item that is coming in is already in, do nohing
			if (wwItems[PrevIndex(headIndex)].IsInWhirlwind) {
				return;
			}

			// swap
			wwItems[tailIndex].End();
			wwItems[PrevIndex(headIndex)].StirUpByShift(100f, slot);
		}

		// move the indices
		headIndex = ShiftIndexByDirection(headIndex, direction);
		tailIndex = ShiftIndexByDirection(tailIndex, direction);
	}


	// slow down initial spin
	public void SlowToStop (bool slowToStopLater) {
		isSlowingDown = true;
		this.isTransitioningToContextExam = slowToStopLater;
		beltEnd.Enable(true);
		for (int i = 0; i < slots.Length; i++) {
			slots[i].EnableCollider(true);
		}

		if (!slowToStopLater) {
			SlowToStopFast(false);
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
		beltEnd.IsInContextExam = false;
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].WhirlExam();
			}
		}
	}


	// is able to interact, spin individual belts
	public void ContextExam () {
		beltEnd.IsInContextExam = true;

		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].ContextExam();
			}
		}
	}

	// end the entire belt, return all items
	public void End () {
		isOperating = false;
		label.Fade(isOperating);
		beltEnd.Enable(false);
		beltEnd.IsInContextExam = false;
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].End();
			}
		}
		for (int i = 0; i < slots.Length; i++) {
			slots[i].EnableCollider(false);
		}
	}

	// when a belt's items are all returned to position, reset them to a stack
	public void ResetToIdle () {
		for (int i = 0; i < wwItems.Count; i++) {
			wwItems[i].ResetToIdle();
		}
	}

	// update all the ones that are in slots
	public void ComputeState () {
		// swirl belt to correct locatin
		if (isSlowingDown && isTransitioningToContextExam) {
			if (beltEnd.MostRecentCollisionIsHead) {
				label.Fade(isOperating);
				SlowToStopFast(true);
			}
		}

		for (int i = 0; i < wwItems.Count; i++) {
			wwItems[i].ComputeState();
		}
	}


/////// inherited from MonoBehaviour //////
	// FixedUpdate is called at set intervals
	void FixedUpdate () { }
}
