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


	void CheckInteractionWithWhirlwind () {
		if (Input.GetKeyDown("a") &&
				currentState == State.Idle) {
			for (int i = 0; i < belts.Length; i++) {
				belts[i].StirUp();
			}
			currentState = State.StirUp;
		} else if (Input.GetKeyDown("s") &&
							 currentState == State.Interacting) {
			for (int i = 0; i < belts.Length; i++) {
				belts[i].End();
			}
			currentState = State.End;
		}
	}

	void ComputeState () {
		for (int i = 0; i < belts.Length; i++) {
			belts[i].ComputeState();
		}
	}
	
	// Update is called once per frame
	void Update () {
		CheckInteractionWithWhirlwind();
	}

	void FixedUpdate () {
		ComputeState();
	}
}
