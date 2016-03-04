using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class WhirlwindBeltEnd : MonoBehaviour {

	WhirlwindBelt belt;			// the belt this is a member of
	bool isInContextExam;
	bool mostRecentCollisionIsHead;


	// Use this for initialization
	void Start () {
		isInContextExam = false;
	}

	// initialize key values called from the belt this belongs to
	public void Initialize (Transform parent, Vector3 position, WhirlwindBelt belt) {
		transform.parent = parent;
		transform.position = position;
		this.belt = belt;
		Enable(false);
	}

	public bool IsInContextExam { 
		get { return isInContextExam; }
		set { isInContextExam = value; } }
	public bool MostRecentCollisionIsHead { get { return mostRecentCollisionIsHead; } }


	public void Enable (bool isEnabled) {
		mostRecentCollisionIsHead = false;
		GetComponent<Collider>().enabled = isEnabled;
	}

	// an end of the belt is reached, shift the belt
	void Shift (WhirlwindBeltSlot w, bool isHead, bool isTail) {
		if ((isHead && w.direction > 0f) || 
			 	(isTail && w.direction < 0f)) {
			belt.ShiftByOne((int)w.direction);
		}
	}

	// logs whether the tail collides also to see 
	// when we can stop a belt that is looking to stop
	void CheckCollision (Collider other) {
		Debug.Assert(other.GetComponent<WhirlwindBeltSlot>() != null);
		Debug.Assert(belt != null);
		WhirlwindBeltSlot w = other.GetComponent<WhirlwindBeltSlot>();

		bool isHead = belt.IsAtHead(other.transform);
		bool isTail = belt.IsAtTail(other.transform);
		if (isInContextExam) {
			Shift(w, isHead, isTail);
		}
		mostRecentCollisionIsHead = isTail;
	}

	// collision is checked on enter and exit
	void OnTriggerEnter(Collider other) {
		CheckCollision(other);
	}
	void OnTriggerExit(Collider other) {
		CheckCollision(other);
	}
}