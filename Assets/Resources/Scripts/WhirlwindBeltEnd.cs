using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class WhirlwindBeltEnd : MonoBehaviour {

	int count;
	public bool isInContextExam;
	public WhirlwindBelt belt;

	// Use this for initialization
	void Start () {
		count = 0;
		isInContextExam = false;
	}


	void Shift (Collider other) {
		if (isInContextExam) {
			Debug.Assert(other.GetComponent<WhirlwindBeltSlot>() != null);
			Debug.Assert(belt != null);

			WhirlwindBeltSlot w = other.GetComponent<WhirlwindBeltSlot>();
			if ((belt.IsAtHead(other.transform) && w.direction > 0f) || 
				 	(belt.IsAtTail(other.transform) && w.direction < 0f)) {
				belt.ShiftByOne((int)w.direction);
			}
		}
	}


	void OnTriggerEnter(Collider other) {
		Shift(other);
	}

	void OnTriggerExit(Collider other) {
		Shift(other);
	}
}
