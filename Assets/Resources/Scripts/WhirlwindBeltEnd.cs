using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class WhirlwindBeltEnd : MonoBehaviour {

	public bool isInContextExam;
	public WhirlwindBelt belt;

	public bool mostRecentCollisionIsTail;


	// Use this for initialization
	void Start () {
		isInContextExam = false;
	}


	void Shift (WhirlwindBeltSlot w, bool isHead, bool isTail) {
		if ((isHead && w.direction > 0f) || 
			 	(isTail && w.direction < 0f)) {
			belt.ShiftByOne((int)w.direction);
		}
	}


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


	void OnTriggerEnter(Collider other) {
		CheckCollision(other);
	}

	void OnTriggerExit(Collider other) {
		CheckCollision(other);
	}
}
