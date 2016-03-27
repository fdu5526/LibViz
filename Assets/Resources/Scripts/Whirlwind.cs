using UnityEngine;
using System;
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
	SearchWhirlwindItem enlargedItem;
	SearchWhirlwindItem draggedSearchItem;
	GameObject searchUI;
	SearchBar searchBar;
	GameObject enlargedSelectionUI;
	GameObject fullscreenSelectionUI;

	// a whirlwind is defined as an array of WhirlWindBelt
	WhirlwindBelt[] belts;

	// database 
	DatabaseManager databaseManager;
	List<WhirlwindBeltInfo> defaultBookinfos;


	// Use this for initialization
	void Awake () {
		WhirlwindItem.InitializeItemImages();

		currentState = State.Idle;
		userInputTimer = new Timer(60f);
		draggedSearchItem = null;

		// establish enlarge and fullscreen game objects
		mainCamera = GameObject.Find("Main Camera").GetComponent<MainCamera>();
		searchUI = GameObject.Find("SearchUI");
		searchBar = GameObject.Find("SearchBar").GetComponent<SearchBar>();
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
		Array.Sort(belts, delegate(WhirlwindBelt b1, WhirlwindBelt b2) { return b2.level.CompareTo(b1.level); });

		// Log into the database
		databaseManager = GameObject.Find("DatabaseManager").GetComponent<DatabaseManager>();
		databaseManager.Login();

		//create the default whirlwind
		defaultBookinfos = databaseManager.GetDefaultBookInfos(belts.Length);
	}

	// current debugging based state machine triggers
	// TODO remove me at the end
	void CheckInteractionWithWhirlwind () {
		if (Input.GetKeyDown("a") &&
				currentState == State.Idle) {
			LoadNewItems(defaultBookinfos);
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
	public void SearchDeskExplore () {
		List<BookInfo> bookinfos = searchBar.SelectedBookInfos;
		
		if (bookinfos.Count > 0) { // actually stuffs in the search
			List<WhirlwindBeltInfo> newInfos = databaseManager.Search(bookinfos, belts.Length);

			// load a new whirlwind
			LoadNewItems(newInfos);
				
			// stir up the new items
			StirUp(Global.StirUpSpeed);
			currentState = State.StirUpNewContextExam;
		} else {
			// TODO do nothing?
		}
		
	}

	// Set state to Idle, and load new items
	void LoadNewItems (List<WhirlwindBeltInfo> infos) {
		Debug.Assert(currentState == State.Idle ||
								 currentState == State.ContextExam || 
								 currentState == State.StirUpNewContextExam ||
								 currentState == State.SlowToStopContextExam ||
								 currentState == State.WhirlExam);
		Debug.Assert(infos.Count == belts.Length);

		// drop them all
		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		ResetToIdle();

		// load the new ones
		for (int i = 0; i < infos.Count; i++) {
			belts[i].LoadNewItems(infos[i]);
		}
	}

	// clicked on a single whirlwind item, do a search based on it
	void LoadNewWhirlwindBasedOnItem (WhirlwindItem wwItem) {
		Debug.Assert(IsEnlargedOrFullscreen);

		// search based on this single item
		List<WhirlwindBeltInfo> newInfos = databaseManager.Search(wwItem.BookInfo, belts.Length);

		// load a new whirlwind
		LoadNewItems(newInfos);
			
		// stir up the new items
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
		enlargedItem = new SearchWhirlwindItem(wwItem);
		LoadNewWhirlwindBasedOnItem(wwItem);
		mainCamera.ZoomIn();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		enlargedSelectionUI.GetComponent<EnlargedSelectionUI>().ItemSprite = wwItem.Sprite;

		LogUserInput();
	}

	// close the UI for enlarge selection, return item to slot
	public void ExitEnlargeSelection (bool isEnding) {
		Debug.Assert(IsEnlargedOrFullscreen);

		mainCamera.ZoomOut();
		enlargedItem = null;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;

		// go back to WhirlExam if we aren't actually ending
		if (!isEnding) {
			LoadNewItems(defaultBookinfos);
			StirUpAutoStopWhirlExam(Global.StirUpSpeed);
		}
		LogUserInput();
	}

	// show detailed information about selected item
	public void EnterFullScreen () {
		Debug.Assert(IsEnlargedOrFullscreen);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = true;
		fullscreenSelectionUI.GetComponent<FullscreenSelectionUI>().ItemSprite = enlargedItem.Sprite;
		LogUserInput();
	}

	// show detailed information about selected item
	public void ExitFullScreen () {
		Debug.Assert(IsEnlargedOrFullscreen);
		
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = false;
		LogUserInput();
	}

	public bool IsDraggingSearchItem { get { return draggedSearchItem != null; } }

	public SearchWhirlwindItem DraggedSearchItem { 
		get { return draggedSearchItem; } 
	}

	public SearchWhirlwindItem EnlargedItem {
		get {
			Debug.Assert(IsEnlargedOrFullscreen);
			return enlargedItem;
		}
	}

	public Sprite EnlargedItemSprite {
		get {
			Debug.Assert(IsEnlargedOrFullscreen);
			return enlargedItem.Sprite;
		}
	}

	// user starts dragging the enlarged item
	public void DragItemImage () {
		Debug.Assert(IsEnlargedOrFullscreen);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled ||
								 fullscreenSelectionUI.GetComponent<Canvas>().enabled);

		draggedSearchItem = enlargedItem;
		searchUI.GetComponent<SearchUI>().EnableDragShadow(enlargedItem.Sprite);
	}

	// user starts dragging an item from the search bar
	public void DragItemImage (SearchWhirlwindItem s) {
		Debug.Assert(s != null);

		draggedSearchItem = s;
		searchUI.GetComponent<SearchUI>().EnableDragShadow(s.Sprite);
	}

	// user starts dragging an item to search bar
	public void DropItemImage () {
		Debug.Assert(IsEnlargedOrFullscreen);

		draggedSearchItem = null;
		searchUI.GetComponent<SearchUI>().DisableDragShadow();
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
			//End();
		}
		ComputeState();
	}
}
