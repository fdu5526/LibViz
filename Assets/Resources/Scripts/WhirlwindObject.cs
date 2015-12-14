using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	// assigned
	[Range(5f, 18.0f)]
	public float speed;

	[Range(2.5f, 9.5f)]
	public float height;

	public float radius;

	// state machine
	public enum State { Dormant, FlyToOrbit, Orbit, FlyToGrid, Grid, FlyToDormant };
	public State currentState;
	bool isInGridFront;


	GameObject[] otherObjects;


	// generated
	Vector3 dormantPosition;
	public Vector3 gridPosition;

	Transform center;
	GameObject trail;

	Vector3 defaultScale;



	// Use this for initialization
	void Start () {
		currentState = State.Dormant;
		radius = height / 9f * 5f;
		isInGridFront = false;

		defaultScale = GetComponent<Transform>().localScale;

		otherObjects = GameObject.FindGameObjectsWithTag("WhirlwindObject");
		dormantPosition = GetComponent<Transform>().position;
		center = GameObject.Find("WhirlwindCenter").GetComponent<Transform>();
		trail = GetComponent<Transform>().Find("Trail").gameObject;
		trail.GetComponent<ParticleSystem>().Stop();
	}




	// for setting initial angular velocity
	float RandomAngularVelocityRange { 
		get { 
			float f = 2f * UnityEngine.Random.Range(0.3f, 1f);
			return UnityEngine.Random.Range(0f, 1f) > 0.5f ? f : -f;
		} 
	}



	public void FlyToOrbit () {
		currentState = State.FlyToOrbit;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Collider>().enabled = false;
		trail.GetComponent<ParticleSystem>().Play();
		GetComponent<Rigidbody>().angularVelocity = new Vector3(RandomAngularVelocityRange,
																														RandomAngularVelocityRange, 
																														RandomAngularVelocityRange);
	}



	void Orbit () {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;


		if (!GetComponent<Collider>().enabled) {
			GetComponent<Collider>().enabled = true;
		}
		
		// vertical velocity
		if (currentState == State.FlyToOrbit) {
			if (GetComponent<Transform>().position.y < height) {
				dy = speed / 45f;
			}
		}

		// d is directional vector to player, d2 is the 2D vector
		p = GetComponent<Transform>().position;
		d = center.position - p;
		d2 = new Vector2(d.x, d.z);

		// small adjustments to prevent objects from escaping orbit
		d2n = d2.normalized;
		float rd = radius - d2.magnitude;
		if (rd > 0.05f) {
			GetComponent<Transform>().position = p - 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		} else if (rd < -0.05f) {
			GetComponent<Transform>().position = p + 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		}

		// rotation based on rotation matrix		
		if (currentState == State.FlyToOrbit) {
			xc = 0.17f;
			yc = 0.985f;
		} else {
			xc = 0.0698f;
			yc = 0.998f;
		}

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		GetComponent<Rigidbody>().velocity = new Vector3(v.x, dy, v.y) * speed * 0.3f;
	}


	void OnMouseUp () {
		if (currentState == State.Grid) {
			if (isInGridFront) {
				MoveToBackOfGrid();
			} else {
				for (int i = 0; i < otherObjects.Length; i++) {
					otherObjects[i].GetComponent<WhirlwindObject>().MoveToBackOfGrid();
				}
				MoveToFrontOfGrid();
			}
		}
	}


	public void MoveToFrontOfGrid () {
		GetComponent<Transform>().localScale = defaultScale * 2f;
		GetComponent<Transform>().position = new Vector3(0f, 4f, -6f);
		isInGridFront = true;
	}


	public void MoveToBackOfGrid () {
		GetComponent<Transform>().localScale = defaultScale;
		GetComponent<Transform>().position = gridPosition;
		isInGridFront = false;
	}




	public void FlyToDormant () {
		GetComponent<Transform>().localScale = defaultScale;
		currentState = State.FlyToDormant;
		GetComponent<Rigidbody>().useGravity = true;
		trail.GetComponent<ParticleSystem>().Stop();
		GetComponent<Rigidbody>().velocity = speed * 0.3f * (dormantPosition - GetComponent<Transform>().position).normalized;
	}


	public void FlyToGrid () {
		currentState = State.FlyToGrid;
		GetComponent<Rigidbody>().useGravity = false;
		trail.GetComponent<ParticleSystem>().Stop();
	}

	void ComputeState () {
		Vector3 p = GetComponent<Transform>().position;

		// state machine transitions
		switch (currentState) {
			case State.Dormant:
				break;
			case State.FlyToOrbit:
				if (Mathf.Abs(p.y - height) < 1f) { // FlyToOrbit => Orbit
					currentState = State.Orbit;
				} else {
					Orbit();
				}
				break;
			case State.Orbit:
				Orbit();
				break;
			case State.FlyToGrid:
				Vector3 d = gridPosition - p;
				if (d.sqrMagnitude < 0.05f)  { // FlyToGrid => Grid
					currentState = State.Grid;
					GetComponent<Rigidbody>().velocity = Vector3.zero;
					GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				} else {
					GetComponent<Transform>().position = Vector3.Slerp(p, gridPosition, 0.1f);
					Quaternion q = GetComponent<Transform>().rotation;
					GetComponent<Transform>().rotation = Quaternion.Slerp(q, Quaternion.identity, 0.7f);
				}
				break;
			case State.Grid:
				if (!isInGridFront) {
					Quaternion q = GetComponent<Transform>().rotation;
					GetComponent<Transform>().rotation = Quaternion.Slerp(q, Quaternion.identity, 0.7f);
					GetComponent<Rigidbody>().AddForce((gridPosition - p) * 5f);
				}
				
				break;
			case State.FlyToDormant:
				if (p.y < 2f) { // FlyToDormant => Dormant
					currentState = State.Dormant;
				} else {
					GetComponent<Rigidbody>().velocity = speed * (dormantPosition - GetComponent<Transform>().position).normalized;
				}
				break;
		}
	}

	
	void FixedUpdate () {
		ComputeState();
	}
}
