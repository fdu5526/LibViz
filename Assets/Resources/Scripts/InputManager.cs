using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	Timer dragStartTimer;

	// Use this for initialization
	void Start () {
		dragStartTimer = new Timer(0.05f);
	}


	public bool IsDragging {
		get {
			return Input.GetMouseButton(0) && dragStartTimer.IsOffCooldown;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			dragStartTimer.Reset();
		}
	}
}
