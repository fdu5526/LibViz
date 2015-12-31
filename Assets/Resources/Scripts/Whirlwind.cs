using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	// state machine
	public enum State {Idle, StirUp, SlowToStop, Interacting, End, Frozen };
	public State currentState;

	// a whirlwind is defined as an array of WhirlWindBelt
	WhirlwindBelt[] belts;

	// Use this for initialization
	void Start () {
		currentState = State.Idle;
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
			StirUp();
		} else if (Input.GetKeyDown("s") &&
							 currentState == State.StirUp) {
			SlowToStop();
		} else if (Input.GetKeyDown("d") &&
							 currentState == State.Interacting) {
			End();
		}
	}

/////// functions for setting whirlwind state //////
	void StirUp () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].StirUp();
		}
		currentState = State.StirUp;
	}


	void SlowToStop () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].SlowToStop();
		}
		currentState = State.SlowToStop;
		Invoke("CanInteract", 2f);
	}


	void CanInteract () {
		currentState = State.Interacting;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].CanInteract();
		}
	}


	void End () {
		currentState = State.End;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		Invoke("ResetToIdle", 2.5f);
	}

	void ResetToIdle () {
		currentState = State.Idle;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ResetToIdle();
		}
	}


	public void Freeze () {
		currentState = State.Frozen;
		for (int i = 0; i < belts.Length; i++) {
			belts[i].Freeze();
		}
	}


	public void UnFreeze () {
		currentState = State.Interacting; // TODO watch for edge case
		for (int i = 0; i < belts.Length; i++) {
			belts[i].UnFreeze();
		}
	}


	void ComputeState () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ComputeState(currentState);
		}
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
