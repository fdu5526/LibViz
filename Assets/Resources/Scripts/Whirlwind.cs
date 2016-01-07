using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	// state machine
	public enum State {Idle, StirUp, SlowToStop, WhirlExam, ContextExam, End };
	public State currentState;

	public bool isFrozen;

	Vector3 enlargedObjectPosition;
	WhirlwindObject enlargedObject;
	GameObject enlargedSelectionUI;
	GameObject fullscreenSelectionUI;
	Transform enlargedObjectSlot;

	// a whirlwind is defined as an array of WhirlWindBelt
	WhirlwindBelt[] belts;

	// Use this for initialization
	void Start () {
		currentState = State.Idle;

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
							 currentState == State.StirUp) {
			SlowToStop(false);
		} else if (Input.GetKeyDown("d")) {
			End();
		}
	}

/////// functions for setting whirlwind state //////
	void StirUp (float speed) {

		Debug.Assert(currentState == State.Idle || 
								 currentState == State.WhirlExam);

		bool shouldLoadObjects = currentState == State.Idle;

		for (int i = 0; i < belts.Length; i++) {
			belts[i].StirUp(speed, shouldLoadObjects);
		}
		currentState = State.StirUp;
	}


	void SlowToStop (bool isTransitioningToContextExam) {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop(isTransitioningToContextExam);
		}
		currentState = State.SlowToStop;
		StartCoroutine(CheckBeltsStop(isTransitioningToContextExam));
	}

	IEnumerator CheckBeltsStop (bool isTransitioningToContextExam) {
		Debug.Assert(currentState == State.SlowToStop);

		while (true) {
			if (currentState == State.End) {
				yield break;
			}

			bool allDone = true;
			for (int i = 0; i < belts.Length; i++) {
				allDone &= belts[i].IsDoneSlowingDown;
			}

			if (allDone) {
				if (isTransitioningToContextExam) {
					ContextExam();
				} else {
					WhirlExam();
				}				
				break;
			} else {
				yield return new WaitForSeconds(0.01f);
			}
		}
	}

	void WhirlExam () {
		Debug.Assert(currentState == State.SlowToStop);

		currentState = State.WhirlExam;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].WhirlExam();
		}
	}

	void ContextExam () {
		Debug.Assert(currentState == State.SlowToStop);

		currentState = State.ContextExam;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ContextExam();
		}
	}


	void End () {
		currentState = State.End;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		Invoke("ResetToIdle", Global.ResetToIdleTime);
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

	void UnFreeze () {
		isFrozen = false;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].UnFreeze();
		}
	}

	void ComputeState () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ComputeState(currentState);
		}
	}

/////// public functions for manipulating whirlwind state //////
	public void SetMouseDownPosition () {
		Debug.Assert(currentState == State.WhirlExam);

		for (int i = 0; i < belts.Length; i++) {
			belts[i].SetMouseDownPosition();
		}
	}
	public void Spin () {
		Debug.Assert(currentState == State.WhirlExam);

		for (int i = 0; i < belts.Length; i++) {
			belts[i].Spin();
		}
	}


	// only call this from WhirlwindObject.Enlarge()
	public void EnterEnlargeSelection (WhirlwindObject wwObj, Transform slot) {
		Debug.Assert(enlargedObject == null);
		Debug.Assert(wwObj != null);

		Freeze();
		enlargedObject = wwObj;
		enlargedObjectSlot = slot;
		wwObj.transform.position = enlargedObjectPosition;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;

		if (currentState == State.WhirlExam) {
			StirUp(50f);
			SlowToStop(true);
		}
	}

	public void ExitEnlargeSelection () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(enlargedObjectSlot != null);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		
		enlargedObject.UnEnlarge(enlargedObjectSlot);
		enlargedObject = null;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		UnFreeze();
	}

	public void EnterFullScreen () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		
		enlargedObject.FullScreen();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = true;
	}


	public void ExitFullScreen () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(!enlargedSelectionUI.GetComponent<Canvas>().enabled);
		Debug.Assert(fullscreenSelectionUI.GetComponent<Canvas>().enabled);
		
		enlargedObject.UnFullScreen();
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
		fullscreenSelectionUI.GetComponent<Canvas>().enabled = false;
	}


/////// inherited from MonoBehaviour //////
	// Update is called once per frame
	void Update () {
		CheckInteractionWithWhirlwind();
	}

	void FixedUpdate () {
		ComputeState();
	}
}
