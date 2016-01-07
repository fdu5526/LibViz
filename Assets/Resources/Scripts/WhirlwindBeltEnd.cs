using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class WhirlwindBeltEnd : MonoBehaviour {

	public WhirlwindBelt belt;			// the belt this is a member of
	public bool isInContextExam;
	public bool mostRecentCollisionIsTail;


	// Use this for initialization
	void Start () {
		isInContextExam = false;
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
		mostRecentCollisionIsTail = isTail;
	}

	// collision is checked on enter and exit
	void OnTriggerEnter(Collider other) {
		CheckCollision(other);
	}
	void OnTriggerExit(Collider other) {
		CheckCollision(other);
	}
}
