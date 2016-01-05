using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	// state machine
	public enum State {Idle, StirUp, SlowToStop, ContextExam, End, Frozen };
	public State currentState;

	Vector3 enlargedObjectPosition;
	WhirlwindObject enlargedObject;
	GameObject enlargedSelectionUI;

	// a whirlwind is defined as an array of WhirlWindBelt
	WhirlwindBelt[] belts;

	// Use this for initialization
	void Start () {
		currentState = State.Idle;

		enlargedObjectPosition = new Vector3(0f, 11.24f, -15.8f);
		enlargedSelectionUI = GameObject.Find("EnlargedSelectionUI");
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
		
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
			SlowToStop();
		} else if (Input.GetKeyDown("d") &&
							 currentState == State.ContextExam) {
			End();
		}
	}

/////// functions for setting whirlwind state //////
	void StirUp (float speed) {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].StirUp(speed);
		}
		currentState = State.StirUp;
	}


	void SlowToStop () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop();
		}
		currentState = State.SlowToStop;
		Invoke("ContextExam", Global.TransitionToContextExamTime);
	}


	void ContextExam () {
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
		currentState = State.Frozen;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].Freeze();
		}
	}

	void UnFreeze () {
		currentState = State.ContextExam; // TODO watch for edge case
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
	// only call this from WhirlwindObject.Enlarge()
	public void EnterEnlargeSelection (WhirlwindObject wwObj) {
		Debug.Assert(enlargedObject == null);
		Debug.Assert(wwObj != null);

		Freeze();
		enlargedObject = wwObj;
		wwObj.transform.position = enlargedObjectPosition;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
	}

	public void ExitEnlargeSelection () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		
		UnFreeze();
		enlargedObject.UnEnlarge();
		enlargedObject = null;
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
	}


	public void EnterFullScreen () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(enlargedSelectionUI.GetComponent<Canvas>().enabled);
		enlargedSelectionUI.GetComponent<Canvas>().enabled = false;
	}


	public void ExitFullScreen () {
		Debug.Assert(enlargedObject != null);
		Debug.Assert(!enlargedSelectionUI.GetComponent<Canvas>().enabled);
		enlargedSelectionUI.GetComponent<Canvas>().enabled = true;
	}


/////// inherited from MonoBehaviour //////
	// Update is called once per frame
	void Update () {
		CheckInteractionWithWhirlwind();
	}

	void FixedUpdate () {
		if (currentState != State.Frozen) {
			ComputeState();
		}
	}
}
