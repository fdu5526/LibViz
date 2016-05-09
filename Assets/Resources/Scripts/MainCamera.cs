using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	enum State { ZoomIn, ZoomOut, StoppedIn, StoppedOut };
	State currentState;

	Vector3 zoomPosition, defaultPosition;
	float defaultFov, zoomFov;

	Timer transitionTimer;
	
	// Use this for initialization
	void Start () {
		currentState = State.StoppedOut;
		defaultPosition = new Vector3(0f, 9.4f, -20.04f);
		defaultFov = 60f;

		zoomPosition = new Vector3(0f, 8.6f, -28.93f);
		zoomFov = 40f;

		GetComponent<Camera>().fieldOfView = defaultFov;
		transform.position = defaultPosition;

		transitionTimer = new Timer(2f);
	}


	public void ZoomIn () {
		if (currentState != State.StoppedIn) {
			currentState = State.ZoomIn;
			transitionTimer.Reset();
		}
	}

	public void ZoomOut () {
		if (currentState != State.StoppedOut) {
			currentState = State.ZoomOut;
			transitionTimer.Reset();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		switch (currentState) {
			case State.ZoomIn:
				transform.position = Vector3.Lerp(defaultPosition, zoomPosition, transitionTimer.PercentTimeLeft);
				GetComponent<Camera>().fieldOfView = Mathf.Lerp(defaultFov, zoomFov, transitionTimer.PercentTimeLeft);
				if (transitionTimer.IsOffCooldown) {
					currentState = State.StoppedIn;
				}
				break;
			case State.ZoomOut:
				transform.position = Vector3.Lerp(zoomPosition, defaultPosition, transitionTimer.PercentTimeLeft);
				GetComponent<Camera>().fieldOfView = Mathf.Lerp(zoomFov, defaultFov, transitionTimer.PercentTimeLeft);
				if (transitionTimer.IsOffCooldown) {
					currentState = State.StoppedOut;
				}
				break;
			default:
				break;
		}
	}
}
