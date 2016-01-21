using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Whirlwind : MonoBehaviour {

	// state machine
	enum State { Idle, StirUp, SlowToStop,
							 StirUpAutoStopWhirlExam, WhirlExam, 
							 SlowToStopContextExam, ContextExam, 
							 End };
	State currentState;

	// related to user inputs
	public bool isFrozen;
	public bool isBeingSpun;

	// set the whirlwind to Idle if it is
	Timer userInputTimer;

	// for enlarge and fullscreen selection
	WhirlwindItem enlargedItem;
	Vector3 enlargedItemPosition;
	GameObject searchUI;
	GameObject enlargedSelectionUI;
	GameObject fullscreenSelectionUI;

	// a whirlwind is defined as an array of WhirlWindBelt
	WhirlwindBelt[] belts;

	// search 
	List<SearchWhirlwindItem> itemsInSearch;
	List<SearchSlot> searchSlots;


	// Use this for initialization
	void Start () {
		currentState = State.Idle;

		userInputTimer = new Timer(60f);
		itemsInSearch = new List<SearchWhirlwindItem>();


		enlargedItemPosition = new Vector3(0f, 11.24f, -15.8f);
		searchUI = GameObject.Find("SearchUI");
		enlargedSelectionUI = GameObject.Find("EnlargedSelectionUI");
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		fullscreenSelectionUI = GameObject.Find("FullscreenSelectionUI");
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = false;
		
		GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindBelt");
		belts = new WhirlwindBelt[gl.Length];

		for (int i = 0; i < gl.Length; i++) {
			belts[i] = gl[i].GetComponent<WhirlwindBelt>();
		}
	}

	// current debugging based state machine triggers
	void CheckInteractionWithWhirlwind () {
		if (Input.GetKeyDown("a") &&
				currentState == State.Idle) {
			StirUp(Global.StirUpSpeed);
		} else if (Input.GetKeyDown("s") &&
							 currentState == State.StirUp && 
							 IsDoneStirUp) {
			SlowToStop();
		} else if (Input.GetKeyDown("w") && 
							 currentState == State.WhirlExam) {

			string[][] ids =  new string [5][] {
				new string[] {"1", "2", "3"},
				new string[] {"1", "2", "3"},
				new string[] {"1", "2", "3"},
				new string[] {"1", "2", "3"},
				new string[] {"1", "2", "3"}
			};

			LoadNewItems(ids);

		} else if (Input.GetKeyDown("d") && 
							 IsDoneStirUp &&
							 currentState != State.End && 
							 currentState != State.Idle) {
			End();
		}
	}

/////// functions for manipulating data //////
	public void LoadNewItems (string[][] itemIDs) {
		Debug.Assert(itemIDs.Length == belts.Length);

		End();
		for (int i = 0; i < itemIDs.Length; i++) {
			belts[i].LoadNewItems(itemIDs[i]);
		}
		StirUpAutoStopWhirlExam(Global.StirUpSpeed);
	}
	

/////// public functions for setting whirlwind state //////
	public void StirUp (float speed) {
		Debug.Assert(currentState == State.Idle || 
								 currentState == State.WhirlExam);

		Freeze();
		bool shouldLoadItems = currentState == State.Idle;

		for (int i = 0; i < belts.Length; i++) {
			belts[i].StirUp(speed, shouldLoadItems);
		}
		currentState = State.StirUp;
		LogUserInput();
	}

	public void SlowToStop () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(false);
		}
		currentState = State.SlowToStop;
		LogUserInput();
	}


	public void End () {
		if (IsEnlargedOrFullscreen) {
			ExitFullScreen();
			ExitEnlargeSelection(true);
		}

		UnFreeze();
		currentState = State.End;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		ResetToIdle();
	}

	// whether all the items are stirred up
	public bool IsDoneStirUp {
		get {
			bool allDone = true;
			for (int i = 0; i < belts.Length; i++) {
				allDone &= belts[i].IsDoneStirUp;
			}
			return allDone;
		}
	}

/////// private functions for setting whirlwind state //////
	// this is for contextExam => WhirlExam
	void StirUpAutoStopWhirlExam (float speed) {
		StirUp(speed);
		currentState = State.StirUpAutoStopWhirlExam;
	}


	void SlowToStopContextExam () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(true);
		}
		currentState = State.SlowToStopContextExam;
		LogUserInput();
	}

	void WhirlExam () {
		Debug.Assert(currentState == State.SlowToStop ||
								 currentState == State.ContextExam);

		UnFreeze();
		currentState = State.WhirlExam;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].WhirlExam();
		}
		LogUserInput();
	}

	void ContextExam () {
		Debug.Assert(currentState == State.SlowToStopContextExam);

		UnFreeze();
		currentState = State.ContextExam;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ContextExam();
		}
		LogUserInput();
	}

	bool IsEnlargedOrFullscreen { get { return enlargedItem != null; } }


	void ResetToIdle () {
		currentState = State.Idle;
	}

	void Freeze () {
		isFrozen = true;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].Freeze();
		}
	}

	// prevent player interaction
	void UnFreeze () {
		isFrozen = false;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].UnFreeze();
		}
	}

	// do automatic state transitions here
	void ComputeState () {
		switch (currentState) {
			case State.StirUpAutoStopWhirlExam:
				if (IsDoneStirUp) {
					SlowToStop();
				}
				break;
			case State.SlowToStop:
			case State.SlowToStopContextExam:
				bool allDone = true;
				for (int i = 0; i < belts.Length; i++) {
					allDone &= belts[i].IsDoneSlowingDown;
				}

				if (allDone) {
					if (currentState == State.SlowToStop) {
						WhirlExam();
					} else {
						ContextExam();
					}
					
				}
				break;
			default:
				break;
		}

		// make sure all the belts are computed also
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ComputeState();
		}
	}

/////// public functions for touch based whirlwind interaction //////
	// user did something, no need to reset to idle any time soon
	public void LogUserInput () {
		userInputTimer.Reset();
	}

	// log most recent mouse position for all belts
	public void SetMouseDownPosition () {
		Debug.Assert(currentState == State.WhirlExam);

		for (int i = 0; i < belts.Length; i++) {
			belts[i].SetMouseDownPosition();
		}
	}

	// spin the entire whirlwind
	public void Spin () {
		Debug.Assert(currentState == State.WhirlExam);

		for (int i = 0; i < belts.Length; i++) {
			belts[i].Spin();
		}
		LogUserInput();
	}


	// only call this from WhirlwindItem.Enlarge()
	// open the UI for enlarge selection of selected item
	public void EnterEnlargeSelection (WhirlwindItem wwItem) {
		Freeze();

		// TODO swap items in and out

		enlargedItem = wwItem;
		//wwItem.transform.position = enlargedItemPosition;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		enlargedSelectionUI.GetComponent<EnlargedSelectionUI>().ItemSprite = wwItem.ItemSprite;

		if (currentState == State.WhirlExam) {
			StirUp(Global.StirUpSpeed);
			SlowToStopContextExam();
		}
		LogUserInput();
	}

	// close the UI for enlarge selection, return item to slot
	public void ExitEnlargeSelection (bool isEnding) {
		Debug.Assert(IsEnlargedOrFullscreen);

		enlargedItem.UnEnlarge();
		enlargedItem = null;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;

		if (!isEnding) {
			End();
			StirUpAutoStopWhirlExam(Global.StirUpSpeed);
		}

		LogUserInput();
		
	}

	// show detailed information about selected item
	public void EnterFullScreen () {
		Debug.Assert(IsEnlargedOrFullscreen);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		
		enlargedItem.FullScreen();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = true;
		fullscreenSelectionUI.GetComponent<FullscreenSelectionUI>().ItemSprite = enlargedItem.ItemSprite;
		LogUserInput();
	}

	// show detailed information about selected item
	public void ExitFullScreen () {
		Debug.Assert(IsEnlargedOrFullscreen);
		
		enlargedItem.UnFullScreen();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = false;
		LogUserInput();
	}

	public Sprite EnlargedItemSprite {
		get {
			Debug.Assert(IsEnlargedOrFullscreen);
			return enlargedItem.ItemSprite;
		}
	}

	// user starts dragging an item to search bar
	public void DragItemImage () {
		Debug.Assert(IsEnlargedOrFullscreen);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled || 
								 fullscreenSelectionUI.GetComponent<Canvas>().enabled);

		searchUI.GetComponent<SearchUI>().EnableDragShadow(enlargedItem.ItemSprite);
	}

	// user starts dragging an item to search bar
	public void DropItemImage () {
		Debug.Assert(IsEnlargedOrFullscreen);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled || 
								 fullscreenSelectionUI.GetComponent<Canvas>().enabled);

		searchUI.GetComponent<SearchUI>().DisableDragShadow();
	}

	public void AddEnlargedItemToSearch (int index) {
		Debug.Assert(IsEnlargedOrFullscreen);

		index = Mathf.Min(index, itemsInSearch.Count);
		SearchWhirlwindItem swwi = new SearchWhirlwindItem(enlargedItem);
		itemsInSearch.Insert(index, swwi);
	}

	public void AddEnlargedItemToSearchEnd () {
		Debug.Assert(IsEnlargedOrFullscreen);

		int index = itemsInSearch.Count;
		SearchWhirlwindItem swwi = new SearchWhirlwindItem(enlargedItem);
		// TODO put it in the slot
		itemsInSearch.Insert(index, swwi);
	}


/////// inherited from MonoBehaviour //////
	// checks user input
	void Update () {
		CheckInteractionWithWhirlwind();
	}

	// do all state computation here
	void FixedUpdate () {

		// if no user input, end
		if (currentState != State.Idle && 
				currentState != State.End &&
				userInputTimer.IsOffCooldown) {
			End();
		}
		ComputeState();
	}
}
