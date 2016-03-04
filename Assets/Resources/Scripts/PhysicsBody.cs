using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public abstract class PhysicsBody : MonoBehaviour {

	// aliases
	protected Rigidbody rigidbody;
	protected Collider collider;

	// Use this for initialization
	protected virtual void Awake () {
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
	}

}
