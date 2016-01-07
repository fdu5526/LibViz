using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	// state machine
	public enum State {Idle, StirUp, SlowToStop, WhirlExam, SlowToStopContextExam, ContextExam, End };
	public State currentState;

	// is the whirlwind taking user inputs?
	public bool isFrozen;

	// set the whirlwind to Idle if it is 
	Timer userInputTimer;

	// for enlarge and fullscreen selection
	Vector3 enlargedObjectPosition;
	WhirlwindObject enlargedObject;
	GameObject enlargedSelectionUI;
	GameObject fullscreenSelectionUI;

	// a whirlwind is defined as an array of WhirlWindBelt
	WhirlwindBelt[] belts;

	// Use this for initialization
	void Start () {
		currentState = State.Idle;

		userInputTimer = new Timer(60f);

		enlargedObjectPosition = new Vector3(0f, 11.24f, -15.8f);
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
			StirUp(50f);
		} else if (Input.GetKeyDown("s") &&
							 currentState == State.StirUp && 
							 IsDoneStirUp) {
			SlowToStop();
		} else if (Input.GetKeyDown("d") && 
							 currentState != State.End && 
							 currentState != State.Idle) {
			End();
		}
	}

/////// functions for setting whirlwind state //////
	// whether all the objects are stirred up
	public bool IsDoneStirUp {
		get {
			bool allDone = true;
			for (int i = 0; i < belts.Length; i++) {
				allDone &= belts[i].IsDoneStirUp;
			}
			return allDone;
		}
	}

	void StirUp (float speed) {

		Debug.Assert(currentState == State.Idle || 
								 currentState == State.WhirlExam);

		bool shouldLoadObjects = currentState == State.Idle;

		for (int i = 0; i < belts.Length; i++) {
			belts[i].StirUp(speed, shouldLoadObjects);
		}
		currentState = State.StirUp;
		LogUserInput();
	}


	void SlowToStop () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(false);
		}
		currentState = State.SlowToStop;
		LogUserInput();
	}

	void SlowToStopContextExam () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(true);
		}
		currentState = State.SlowToStopContextExam;
		LogUserInput();
	}

	void WhirlExam () {
		Debug.Assert(currentState == State.SlowToStop);

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
		UnFreeze();
		LogUserInput();
	}


	void End () {
		if (enlargedObject != null) {
			ExitFullScreen();
			ExitEnlargeSelection();
		}

		currentState = State.End;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		ResetToIdle();
	}

	void ResetToIdle () {
		currentState = State.Idle;
	}

	void Freeze () {
		isFrozen = true;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].Freeze();
		}
	}

	// stop 
	void UnFreeze () {
		isFrozen = false;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].UnFreeze();
		}
	}

	void ComputeState () {
		switch (currentState) {
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

		for (int i = 0; i < belts.Length; i++) {
			belts[i].ComputeState(currentState);
		}
	}

/////// public functions for manipulating whirlwind state //////
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


	// only call this from WhirlwindObject.Enlarge()
	// open the UI for enlarge selection of selected object
	public void EnterEnlargeSelection (WhirlwindObject wwObj) {
		Debug.Assert(enlargedObject == null);
		Debug.Assert(wwObj != null);

		Freeze();
		enlargedObject = wwObj;
		wwObj.transform.position = enlargedObjectPosition;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;

		if (currentState == State.WhirlExam) {
			StirUp(50f);
			SlowToStopContextExam();
		}
		LogUserInput();
	}

	// close the UI for enlarge selection, return item to slot
	public void ExitEnlargeSelection () {
		Debug.Assert(enlargedObject != null);

		if (currentState == State.ContextExam) {
			UnFreeze();
		}
		
		enlargedObject.UnEnlarge();
		enlargedObject = null;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		LogUserInput();
	}

	// show detailed information about selected item
	public void EnterFullScreen () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		
		enlargedObject.FullScreen();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = true;
		LogUserInput();
	}

	// show detailed information about selected item
	public void ExitFullScreen () {
		Debug.Assert(enlargedObject != null);
		
		enlargedObject.UnFullScreen();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = false;
		LogUserInput();
	}


/////// inherited from MonoBehaviour //////
	// checks user input
	void Update () {
		CheckInteractionWithWhirlwind();
	}

	// do all state computation here
	void FixedUpdate () {
		if (currentState != State.Idle && 
				currentState != State.End &&
				userInputTimer.IsOffCooldown) {
			End();
		}
		ComputeState();
	}
}
