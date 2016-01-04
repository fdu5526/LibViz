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

			if (belt.IsAtHeadOrTail(other.transform)) {
				WhirlwindBeltSlot w = other.GetComponent<WhirlwindBeltSlot>();
				belt.ShiftByOne((int)w.direction);
			}
		}
	}
}
