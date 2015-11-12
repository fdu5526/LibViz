using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	// assigned
	[Range(5f, 25.0f)]
	public float speed;

	[Range(2.5f, 8f)]
	public float height;

	// state machine
	public enum State { Dormant, FlyInto, Orbit, FlyOut };
	public State currentState;



	// generated
	float radius;
	bool isCounterClockwise;
	Vector3 dormantPosition;
	Vector3 orbitStartPosition;

	Transform center;

	// for going up and down
	int verticalCounter;
	bool isGoingUp;



	// Use this for initialization
	void Start () {

		currentState = State.Dormant;
		radius = height / 9f * 5f;

		dormantPosition = GetComponent<Transform>().position;
		center = GameObject.Find("Center").GetComponent<Transform>();


		Vector3 v = center.position + radius * (center.position - dormantPosition).normalized;
		orbitStartPosition = new Vector3(v.x, height, v.z);
		

		isGoingUp = UnityEngine.Random.Range(0f, 1f) > 0.5f;
	}

	// for setting initial angular velocity
	float RandomAngularVelocityRange { get { return 5f * UnityEngine.Random.Range(-1f, 1f); } }


	void FlyInto () {
		GetComponent<Rigidbody>().velocity = speed * (orbitStartPosition - GetComponent<Transform>().position).normalized;
	}



	// orbit the whirlwind
	void Orbit () {
		Rigidbody rigidbody = GetComponent<Rigidbody>();		

		// go up and down
		if (verticalCounter > 100) {
			verticalCounter = 0;
			isGoingUp = !isGoingUp;
		}
		verticalCounter++;
		
		// d is directional vector to player, d2 is the 2D vector
		Vector3 p = GetComponent<Transform>().position;
		Vector3 d = center.position - p;
		Vector2 d2 = new Vector2(d.x, d.z);


		// small adjustments to prevent objects from escaping orbit
		Vector2 d2n = d2.normalized;
		float rd = radius - d2.magnitude;
		if (rd > 0.05f) {
			GetComponent<Transform>().position = p - 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		} else if (rd < -0.05f) {
			GetComponent<Transform>().position = p + 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		}
		
		// rotation based on rotation matrix
		Vector2 v;
		if (isCounterClockwise) { 
			v = new Vector2(d2.x * 0.0698f + d2.y * 0.998f, d2.x * -0.998f + d2.y * 0.0698f);
		} else {
			v = new Vector2(d2.x * 0.0698f + d2.y * -0.998f, d2.x * 0.998f + d2.y * 0.0698f);
		}
		v.Normalize();
		rigidbody.velocity = new Vector3(v.x, isGoingUp ? 0.06f : -0.06f, v.y) * speed;
	}

	
	// state machine transitions here
	void FixedUpdate () {
			
		Vector3 p = GetComponent<Transform>().position;

		switch (currentState) {
			case State.Dormant:

				if (Input.GetKeyDown("space")) {
					currentState = State.FlyInto;
				}
				break;
			case State.FlyInto:
				
				if ((p - orbitStartPosition).sqrMagnitude < 0.1f) { // FlyInto => Orbit
					isCounterClockwise = UnityEngine.Random.Range(0f, 1f) > 0.5f;
					GetComponent<Rigidbody>().angularVelocity = new Vector3(RandomAngularVelocityRange,
																																	RandomAngularVelocityRange, 
																																	RandomAngularVelocityRange);
					currentState = State.Orbit;
				} else {
					FlyInto();
				}
					

				break;
			case State.Orbit:
				break;
			case State.FlyOut:
				break;
			default:
				break;

		}
	}
}
