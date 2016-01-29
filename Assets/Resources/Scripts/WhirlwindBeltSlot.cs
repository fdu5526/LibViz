using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
public class WhirlwindBeltSlot : MonoBehaviour {
	
	// assigned properties
	public float speed;
	public float height;
	public float radius;
	public float direction;
	
	// internal variables
	bool shouldSlowsDown;
	float slowDownLerpFactor;
	float baseSlowDownLerpFactor = 0.1f;

	// properties
	Transform center;

	// aliases
	Rigidbody rigidbody;
	Collider collider;

	// Use this for initialization
	void Start () {	
		center = GameObject.Find("WhirlwindCenter").transform;
		shouldSlowsDown = false;
		speed = 0f;

		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<Collider>();
	}

	
/////// private helper functions //////
	// spin around, most important function
	void Orbit () {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;

		// d is directional vector to player, d2 is the 2D vector
		p = transform.position;
		d = center.position - p;
		d2 = new Vector2(d.x, d.z);

		// small corrections to prevent items from escaping orbit
		d2n = d2.normalized;
		float rd = radius - d2.magnitude;
		if (rd > 0.05f) {
			transform.position = p - 0.1f * new Vector3(d2n.x, 0f, d2n.y);
		} else if (rd < -0.05f) {
			transform.position = p + 0.1f * new Vector3(d2n.x, 0f, d2n.y);
		}

		// rotation based on rotation matrix		
		xc = 0.0698f;
		yc = 0.998f;

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		Vector3 nv = new Vector3(v.x, dy, v.y) * speed * direction;
		rigidbody.velocity = nv;

		if (shouldSlowsDown) {
			speed = Mathf.Lerp(speed, 0f, slowDownLerpFactor);
		}
		
	}


/////// public functions for setting whirlwindItem state //////
	public bool IsDoneSlowingDown {
		get {
			return speed < 0.1f;
		}
	}

	public void Initialize (Vector3 position, float height, float radius) {
		transform.position = position;
		this.height = height;
		this.radius = radius;
	}

	public void StirUp () {
		speed = Global.SpinSpeed * height / 5f;
		direction = 1f;
		shouldSlowsDown = false;
		slowDownLerpFactor = baseSlowDownLerpFactor;
	}
	
	public void SlowToStopFast (bool isFastStop) {
		shouldSlowsDown = true;
		if (isFastStop) {
			slowDownLerpFactor = 0.3f;
		}
	}

	public void EnableCollider (bool e) {
		collider.enabled = e;
	}

/////// inherited functions //////
	void FixedUpdate () {
		Orbit();
	}
}