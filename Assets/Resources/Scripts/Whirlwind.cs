using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Whirlwind : MonoBehaviour {

	// state machine
	enum State { Idle, 
							 StirUp, StirUpAutoStopWhirlExam, StirUpNewContextExam,
							 SlowToStopWhirlExam, WhirlExam, 
							 SlowToStopContextExam, ContextExam, 
							 End };
	State currentState;

	// related to user inputs
	public bool isBeingSpun;

	// set the whirlwind to Idle if it is
	Timer userInputTimer;

	// for enlarge and fullscreen selection
	MainCamera mainCamera;
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

	//TODO
	string[][] defaultIds;


	// Use this for initialization
	void Start () {
		currentState = State.Idle;
		userInputTimer = new Timer(60f);
		itemsInSearch = new List<SearchWhirlwindItem>();

		// establish enlarge and fullscreen game objects
		enlargedItemPosition = new Vector3(0f, 11.24f, -15.8f);
		mainCamera = GameObject.Find("Main Camera").GetComponent<MainCamera>();
		searchUI = GameObject.Find("SearchUI");
		enlargedSelectionUI = GameObject.Find("EnlargedSelectionUI");
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		fullscreenSelectionUI = GameObject.Find("FullscreenSelectionUI");
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = false;
		
		// get the belts
		GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindBelt");
		belts = new WhirlwindBelt[gl.Length];
		for (int i = 0; i < gl.Length; i++) {
			belts[i] = gl[i].GetComponent<WhirlwindBelt>();
		}

		//TODO make this not a placeholder
		defaultIds =  new string [5][] {
			new string[] {"2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5"},
			new string[] {"2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5"},
			new string[] {"2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5"},
			new string[] {"2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5"},
			new string[] {"2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5"}
		};
	}

	// current debugging based state machine triggers
	// TODO remove me at the end
	void CheckInteractionWithWhirlwind () {
		if (Input.GetKeyDown("a") &&
				currentState == State.Idle) {
			LoadNewItems(defaultIds);
			StirUp(Global.StirUpSpeed);
		} else if (Input.GetKeyDown("s") &&
							 currentState == State.StirUp && 
							 IsDoneStirUp) {
			SlowToStopWhirlExam();
		} else if (Input.GetKeyDown("d") && 
							 IsDoneStirUp &&
							 currentState != State.End && 
							 currentState != State.Idle) {
			End();
		}
	}

/////// functions for manipulating data //////
	// Set state to Idle, and load new items
	void LoadNewItems (string[][] itemIDs) {
		Debug.Assert(currentState == State.Idle ||
								 currentState == State.ContextExam || 
								 currentState == State.StirUpNewContextExam ||
								 currentState == State.SlowToStopContextExam ||
								 currentState == State.WhirlExam);

		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		ResetToIdle();
		for (int i = 0; i < itemIDs.Length; i++) {
			belts[i].LoadNewItems(itemIDs[i]);
		}
	}

	void LoadNewWhirlwindBasedOnItem (WhirlwindItem wwItem) {
		Debug.Assert(IsEnlargedOrFullscreen);

		// TODO actual database query here
		string[][] ids =  new string [5][] {
			new string[] {"1", "1", "1", "1", "1", "1", "1", "1"},
			new string[] {"2", "2", "2", "2", "2", "2", "2", "2", "2", },
			new string[] {"3", "3", "3", "3", "3", "3", "3", "3", "3", "3", "3"},
			new string[] {"4", "4", "4", "4", "4", "4", "4", "4", "4", "4", "4", "4", "4", "4", "4", "4"},
			new string[] {"5", "5", "5", "5", "5", "5", "5"},
		};

		Debug.Assert(ids.Length == belts.Length);
		
		LoadNewItems(ids);
		StirUp(Global.StirUpSpeed);
		currentState = State.StirUpNewContextExam;
	}

/////// private functions for setting whirlwind state //////
	// ContextExam => WhirlExam by exiting enlarge view
	void StirUpAutoStopWhirlExam (float speed) {
		StirUp(speed);
		currentState = State.StirUpAutoStopWhirlExam;
		LogUserInput();
	}

	// WhirlExam => ContextExam by enlarging
	// ContextExam => ContextExam by enlarging another item
	void SlowToStopContextExam () {
		Debug.Assert(currentState == State.StirUpNewContextExam ||
								 currentState == State.StirUp);

		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(true);
		}
		currentState = State.SlowToStopContextExam;
		LogUserInput();
	}

	void WhirlExam () {
		Debug.Assert(currentState == State.SlowToStopWhirlExam ||
								 currentState == State.ContextExam);

		currentState = State.WhirlExam;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].WhirlExam();
		}
		LogUserInput();
	}

	void ContextExam () {
		Debug.Assert(currentState == State.SlowToStopContextExam);

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

	// do automatic state transitions here
	void ComputeState () {
		switch (currentState) {
			case State.StirUpAutoStopWhirlExam:
				if (IsDoneStirUp) {
					SlowToStopWhirlExam();
				}
				break;
			case State.StirUpNewContextExam:
				if (IsDoneStirUp) {
					SlowToStopContextExam();
				}
				break;
			case State.SlowToStopWhirlExam:
			case State.SlowToStopContextExam:
				if (IsDoneSlowingDown) {
					if (currentState == State.SlowToStopWhirlExam) {
						WhirlExam();
					} else {
						ContextExam();
					}
				}
				break;
		}

		// make sure all the belts are computed also
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ComputeState();
		}
	}


/////// public functions for setting whirlwind state //////
	public void StirUp (float speed) {
		Debug.Assert(currentState == State.Idle || 
								 currentState == State.WhirlExam);

		bool shouldLoadItems = currentState == State.Idle;

		for (int i = 0; i < belts.Length; i++) {
			belts[i].StirUp(speed, shouldLoadItems);
		}
		currentState = State.StirUp;
		LogUserInput();
	}

	public void SlowToStopWhirlExam () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(false);
		}
		currentState = State.SlowToStopWhirlExam;
		LogUserInput();
	}


	public void End () {
		if (IsEnlargedOrFullscreen) {
			ExitFullScreen();
			ExitEnlargeSelection(true);
		}

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

	public bool IsDoneSlowingDown {
		get {
			bool allDone = true;
			for (int i = 0; i < belts.Length; i++) {
				allDone &= belts[i].IsDoneSlowingDown;
			}
			return allDone;
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
		enlargedItem = wwItem;
		LoadNewWhirlwindBasedOnItem(wwItem);
		mainCamera.ZoomIn();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		enlargedSelectionUI.GetComponent<EnlargedSelectionUI>().ItemSprite = wwItem.ItemSprite;
		LogUserInput();
	}

	// close the UI for enlarge selection, return item to slot
	public void ExitEnlargeSelection (bool isEnding) {
		Debug.Assert(IsEnlargedOrFullscreen);

		mainCamera.ZoomOut();
		enlargedItem.UnEnlarge();
		enlargedItem.DestroyInSeconds(0f);
		enlargedItem = null;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;

		// go back to WhirlExam if we aren't actually ending
		if (!isEnding) {
			LoadNewItems(defaultIds);
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
