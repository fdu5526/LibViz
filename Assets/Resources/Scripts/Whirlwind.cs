using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	// state machine
	public enum State {Idle, StirUp, SlowToStop, Interacting, Frozen };
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
		Invoke("CanInteract", 1f);
	}

	void CanInteract () {
		currentState = State.Interacting;
	}

	void End () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].End();
		}
		currentState = State.Idle;
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
