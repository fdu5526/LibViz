using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	// assigned
	public float speed;
	public float height;
	public float radius;
	public float direction;

	// generated
	Vector3 dormantPosition;

	// properties
	Transform center;
	GameObject trail;
	Vector3 defaultScale;

	// Use this for initialization
	void Start () {
		direction = 1f;
	
		defaultScale = GetComponent<Transform>().localScale;
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


	// fly into orbit
	public void FlyToOrbit () {
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Collider>().enabled = false;
		//trail.GetComponent<ParticleSystem>().Play();
		Vector3 v = new Vector3(RandomAngularVelocityRange, 
														RandomAngularVelocityRange, 
														RandomAngularVelocityRange);
		GetComponent<Rigidbody>().angularVelocity = v;
	}


	// spin around
	void Orbit (Whirlwind.State currentState) {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;

		if (!GetComponent<Collider>().enabled) {
			GetComponent<Collider>().enabled = true;
		}
		
		// vertical velocity
		if (currentState == Whirlwind.State.StirUp) {
			if (GetComponent<Transform>().position.y < height) {
				dy = speed;
			}
		}

		// d is directional vector to player, d2 is the 2D vector
		p = GetComponent<Transform>().position;
		d = center.position - p;
		d2 = new Vector2(d.x, d.z);

		// small corrections to prevent objects from escaping orbit
		d2n = d2.normalized;
		float rd = radius - d2.magnitude;
		if (rd > 0.05f) {
			GetComponent<Transform>().position = p - 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		} else if (rd < -0.05f) {
			GetComponent<Transform>().position = p + 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		}

		// rotation based on rotation matrix		
		if (currentState == Whirlwind.State.StirUp) {
			xc = 0.17f;
			yc = 0.985f;
		} else {
			xc = 0.0698f;
			yc = 0.998f;
		}

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		Vector3 nv = new Vector3(v.x, dy, v.y) * speed * direction;
		GetComponent<Rigidbody>().velocity = nv;//Vector3.Lerp(GetComponent<Rigidbody>().velocity, nv, 0.5f);

		if (currentState == Whirlwind.State.Interacting) {
			if (speed > 0f) {
				speed = Mathf.Max(0f, speed - 0.05f);
			}
		}
	}

	public void FlyToDormant () {
		GetComponent<Transform>().localScale = defaultScale;
		GetComponent<Rigidbody>().useGravity = true;
		//trail.GetComponent<ParticleSystem>().Stop();
		GetComponent<Rigidbody>().velocity = speed * 0.3f * (dormantPosition - GetComponent<Transform>().position).normalized;
	}



	// do everything state machine here
	public void ComputeState (Whirlwind.State currentState) {
		Vector3 p = GetComponent<Transform>().position;

		// state machine transitions
		switch (currentState) {
			case Whirlwind.State.Idle:
				break;
			case Whirlwind.State.StirUp:
				if (Mathf.Abs(p.y - height) < 1f) { // StirUp => SlowToStop TODO transition here
					GetComponent<Rigidbody>().angularVelocity = new Vector3(0.5f, 0.5f, 0.5f);
				} else {
					Orbit(currentState);
				}
				break;
			case Whirlwind.State.SlowToStop:
				// TODO
				break;
			case Whirlwind.State.Interacting:
				Orbit(currentState);
				break;
			case Whirlwind.State.End:
				if (p.y < 2f) { // End => Idle TODO
					//currentState = State.Dormant; 
				} else {
					GetComponent<Rigidbody>().velocity = speed * (dormantPosition - GetComponent<Transform>().position).normalized;
				}
				break;
		}
	}

	
	void FixedUpdate () {
	}
}
