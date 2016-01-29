using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	enum State { ZoomIn, ZoomOut, Stop };
	State currentState;

	Vector3 zoomPosition, defaultPosition;
	
	// Use this for initialization
	void Start () {
		currentState = State.Stop;
		defaultPosition = new Vector3(0f, 9.97f, -20.04f);
		zoomPosition = new Vector3(0f, 8.6f, -18.02f);
	}


	public void ZoomIn () {
		currentState = State.ZoomIn;
	}

	public void ZoomOut () {
		currentState = State.ZoomOut;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		switch (currentState) {
			case State.ZoomIn:
				transform.position = Vector3.Lerp(transform.position, zoomPosition, 0.1f);
				if ((transform.position - zoomPosition).sqrMagnitude < 0.1f) {
					currentState = State.Stop;
				}
				break;
			case State.ZoomOut:
				transform.position = Vector3.Lerp(transform.position, defaultPosition, 0.1f);
				if ((transform.position - defaultPosition).sqrMagnitude < 0.1f) {
					currentState = State.Stop;
				}
				break;
			case State.Stop:
				break;
		}
	}
}
