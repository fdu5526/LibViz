using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class WhirlwindBeltMarker : MonoBehaviour {

	const float StirUpSpeed = 10f;
	
	// assigned
	public float speed;
	public float height;
	public float radius;
	public float direction;

	// properties
	WhirlwindBelt belt;
	Transform center;

	// aliases
	Collider collider;
	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {	
		Vector3 p = transform.position;
		center = GameObject.Find("WhirlwindCenter").transform;
		belt = transform.parent.GetComponent<WhirlwindBelt>();

		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}


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

		// small corrections to prevent objects from escaping orbit
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

		speed *= 0.9f;
	}

/////// public functions for setting whirlwindObject state //////
	public void Freeze () {
		rigidbody.velocity = Vector3.zero;
	}

/////// inherited functions //////
	void OnMouseDown () {
		belt.SetMouseDownPosition();
	}

	void OnMouseDrag () {
		belt.Spin();
	}

	
	void FixedUpdate () {
		Orbit();
	}
}