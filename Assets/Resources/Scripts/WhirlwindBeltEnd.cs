using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class WhirlwindBeltEnd : MonoBehaviour {


	public bool isInContextExam;
	public WhirlwindBelt belt;

	// Use this for initialization
	void Start () {
		isInContextExam = false;
	}


	void OnTriggerEnter(Collider other) {
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
}
