using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	// assigned
	[Range(5f, 18.0f)]
	public float speed;

	[Range(2.5f, 9.5f)]
	public float height;

	// state machine
	public enum State { Dormant, FlyInto, Orbit, Grid, FlyBack };
	public State currentState;


	// generated
	float radius;
	Vector3 dormantPosition;

	Transform center;
	GameObject trail;

	// for going up and down
	int verticalCounter;
	int currentVerticalCounterMax;
	bool isGoingUp;



	// Use this for initialization
	void Start () {

		currentState = State.Dormant;
		radius = height / 9f * 5f;
		speed *= 0.7f; // TODO

		dormantPosition = GetComponent<Transform>().position;
		center = GameObject.Find("WhirlwindCenter").GetComponent<Transform>();
		trail = GetComponent<Transform>().Find("Trail").gameObject;
		trail.GetComponent<ParticleSystem>().Stop();

		isGoingUp = UnityEngine.Random.Range(0f, 1f) > 0.5f;
	}




	// for setting initial angular velocity
	float RandomAngularVelocityRange { 
		get { 
			float f = 5f * UnityEngine.Random.Range(0.3f, 1f);
			return UnityEngine.Random.Range(0f, 1f) > 0.5f ? f : -f;
		} 
	}
	int NewVerticalCounterMax { get { return (int)UnityEngine.Random.Range(50f, 150f); } }



	public void FlyInto () {
		currentState = State.FlyInto;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Collider>().enabled = false;
		currentVerticalCounterMax = NewVerticalCounterMax;
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
		if (currentState == State.FlyInto) {
			if (GetComponent<Transform>().position.y < height) {
				dy = speed / 45f;
			}
		} else {
			// go up and down
			if (verticalCounter > currentVerticalCounterMax) {
				verticalCounter = 0;
				isGoingUp = !isGoingUp;	
			}
			verticalCounter++;
			dy = isGoingUp ? 0.1f : -0.1f;
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
		if (currentState == State.FlyInto) {
			xc = 0.17f;
			yc = 0.985f;
		} else {
			xc = 0.0698f;
			yc = 0.998f;
		}

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		GetComponent<Rigidbody>().velocity = new Vector3(v.x, dy, v.y) * speed;
	}



	public void FlyBack () {
		currentState = State.FlyBack;
		GetComponent<Rigidbody>().useGravity = true;
		trail.GetComponent<ParticleSystem>().Stop();
		GetComponent<Rigidbody>().velocity = speed * (dormantPosition - GetComponent<Transform>().position).normalized;
	}


	void OnMouseOver () {
		if (Input.GetMouseButtonDown(0) && currentState != State.Grid) {
			// TODO
			GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindObject");
			for (int i = 0; i < gl.Length; i++) {
				gl[i].GetComponent<WhirlwindObject>().currentState = State.Grid;
				//gl[i].GetComponent<Transform>().
			}
		}
		
	}

	
	void FixedUpdate () {
			
		Vector3 p = GetComponent<Transform>().position;

		// state machine transitions
		switch (currentState) {
			case State.Dormant:
				break;
			case State.FlyInto:
				if (Mathf.Abs(p.y - height) < 1f) { // FlyInto => Orbit
					currentState = State.Orbit;
				} else {
					Orbit();
				}
				break;
			case State.Orbit:
				Orbit();
				break;
			case State.Grid:
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				GetComponent<Transform>().eulerAngles = Vector3.zero;
				trail.GetComponent<ParticleSystem>().Stop();
				break;
			case State.FlyBack:
				if (p.y < 2f) { // FlyBack => Dormant
					currentState = State.Dormant;
				} else {
					GetComponent<Rigidbody>().velocity = speed * (dormantPosition - GetComponent<Transform>().position).normalized;
				}
				break;
		}
	}
}
