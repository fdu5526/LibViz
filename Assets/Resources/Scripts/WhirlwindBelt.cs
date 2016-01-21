using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WhirlwindBelt : MonoBehaviour {
	[Range(0, 5)]
	public int level;
	int numOfItemsShownOnBelt;

	float radius;
	float height;

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

		defaultItemPosition = new Vector3(-14.47f, 1.3f, -0.77f);

		center = GameObject.Find("WhirlwindCenter").transform;
		whirlwind = center.GetComponent<Whirlwind>();
		height = transform.position.y;
		radius = height * 0.6f + 1f;
		isOperating = false;
		numOfItemsShownOnBelt = 3 + level * 3;

		WhirlwindItem[] w = GetComponentsInChildren<WhirlwindItem>();
		Debug.Assert(w != null);
		
		// find all the items of this belt
		wwItems = new List<WhirlwindItem>(w);
		wwItems.Sort(delegate(WhirlwindItem w1, WhirlwindItem w2) { return w1.name.CompareTo(w2.name); });
		for (int i = 0; i < wwItems.Count; i++) {
			wwItems[i].Initialize(this, radius, height);
		}

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

		for (int i = 0; i < wwItems.Count; i++) {
			if (!isOperating) {
				yield break;
			}

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
	void SlowAllToStop (bool isFastStop) {
		isSlowingDown = false;
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].SlowToStop();
			}
		}
		for (int i = 0; i < slots.Length; i++) {
			slots[i].SlowToStop(isFastStop);
		}
	}

/////// public functions used for manipulating data //////
	public void LoadNewItems (string[] itemIDs) {
		GameObject g;

		// wipe old items away
		End();
		for (int i = 0; i < wwItems.Count; i++) {
			wwItems[i].DestroyInSeconds(3f);
		}

		// reinstantiate new items
		wwItems = new List<WhirlwindItem>();
		for (int i = 0; i < itemIDs.Length; i++) {
			g = (GameObject)MonoBehaviour.Instantiate(Resources.Load("Prefabs/WhirlwindItem"));
			g.transform.position = defaultItemPosition;
			WhirlwindItem wwi = g.GetComponent<WhirlwindItem>();
			wwi.ItemSprite = (Sprite)Resources.Load("Sprites/Items/" + itemIDs[i]);
			wwItems.Add(wwi);
		}

		StirUp(Global.StirUpSpeed, true);
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
		bool isHead = (slot.position - wwItems[headIndex].slot.position).sqrMagnitude < 1f;
		return isHead;
	}


	public bool IsAtTail (Transform slot) {
		bool isTail = (slot.position - wwItems[tailIndex].slot.position).sqrMagnitude < 1f;
		return isTail;
	}

	// stir up items, but stagger them so they have spaces in between them
	public void StirUp (float speed, bool shouldLoadItems) {
		isOperating = true;
		for (int i = 0; i < slots.Length; i++) {
			slots[i].StirUp();
		}
		if (shouldLoadItems) {
			StartCoroutine(StaggeredStirUp(speed));
		}
	}

	// whether all the slots are filled
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
			Transform slot = wwItems[headIndex].slot;
			Debug.Assert(slot != null);

			if (wwItems[NextIndex(tailIndex)].IsInWhirlwind) {
				return;
			}

			wwItems[headIndex].End();
			wwItems[NextIndex(tailIndex)].StirUpByShift(100f, slot);
		} else { 							// shift prev
			Transform slot = wwItems[tailIndex].slot;
			Debug.Assert(slot != null);

			if (wwItems[PrevIndex(headIndex)].IsInWhirlwind) {
				return;
			}

			wwItems[tailIndex].End();
			wwItems[PrevIndex(headIndex)].StirUpByShift(100f, slot);
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
		beltEnd.isInContextExam = false;
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].WhirlExam();
			}
		}
	}


	// is able to interact, spin individual belts
	public void ContextExam () {
		beltEnd.isInContextExam = true;

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
		beltEnd.GetComponent<Collider>().enabled = false;
		beltEnd.isInContextExam = false;
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].End();
			}
		}
	}

	// when a belt's items are all returned to position, reset them to a stack
	public void ResetToIdle () {
		for (int i = 0; i < wwItems.Count; i++) {
			wwItems[i].ResetToIdle();
		}
	}

	// freeze the entire belt from moving
	public void Freeze () {
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].SetInteractable(false);
			}
		}

		for (int i = 0; i < slots.Length; i++) {
			slots[i].Freeze();
		}
	}

	// unfreeze the belt, can move again
	public void UnFreeze () {
		for (int i = 0; i < wwItems.Count; i++) {
			if (IndexIsInSlots(i)) {
				wwItems[i].SetInteractable(true);
			}
		}
	}

	// update all the ones that are in slots
	public void ComputeState () {
		
		// swirl belt to correct locatin
		if (isSlowingDown && isTransitioningToContextExam) {
			if (beltEnd.mostRecentCollisionIsTail) {
				label.Fade(isOperating);
				SlowAllToStop(true);
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
