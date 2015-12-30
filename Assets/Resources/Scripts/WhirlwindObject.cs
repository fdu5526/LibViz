using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
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

	// aliases
	Collider collider;
	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		direction = 1f;
	
		defaultScale = transform.localScale;
		Vector3 p = transform.position;
		dormantPosition = new Vector3(p.x, 0f, p.y);
		center = GameObject.Find("WhirlwindCenter").transform;
		trail = transform.Find("Trail").gameObject;
		trail.GetComponent<ParticleSystem>().Stop();

		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}


	// for setting initial angular velocity
	float RandomAngularVelocityRange { 
		get { 
			float f = 2f * UnityEngine.Random.Range(0.3f, 1f);
			return UnityEngine.Random.Range(0f, 1f) > 0.5f ? f : -f;
		} 
	}


	// fly into orbit
	public void StirUp () {
		rigidbody.useGravity = false;
		collider.enabled = false;
		//trail.GetComponent<ParticleSystem>().Play();
		Vector3 v = new Vector3(RandomAngularVelocityRange, 
														RandomAngularVelocityRange, 
														RandomAngularVelocityRange);
		rigidbody.angularVelocity = v;
	}


	public void SlowToStop () {

	}


	// spin around
	void Orbit (Whirlwind.State currentState) {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;

		if (!collider.enabled) {
			collider.enabled = true;
		}
		
		// vertical velocity
		if (currentState == Whirlwind.State.StirUp) {
			if (GetComponent<Transform>().position.y < height) {
				dy = speed / 30f;
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
		rigidbody.velocity = nv;//Vector3.Lerp(GetComponent<Rigidbody>().velocity, nv, 0.5f);

		if (currentState == Whirlwind.State.Interacting) {
			if (speed > 0f) {
				speed = Mathf.Max(0f, speed - 0.05f);
			}
		} else if (currentState == Whirlwind.State.SlowToStop) {
			if (speed > 0f) {
				speed = Mathf.Max(0f, speed - 0.1f);
			}
		}
	}

	public void End () {
		transform.localScale = defaultScale;
		rigidbody.useGravity = true;
		//trail.GetComponent<ParticleSystem>().Stop();
		rigidbody.velocity = speed * 0.3f * (dormantPosition - transform.position).normalized;
	}

	public void Freeze () {
		rigidbody.velocity = Vector3.zero;
	}



	// do everything state machine here
	public void ComputeState (Whirlwind.State currentState) {
		Vector3 p = transform.position;

		// state machine transitions
		switch (currentState) {
			case Whirlwind.State.Idle:
				if (p.y > 2f) {
					//rigidbody.velocity = speed * (dormantPosition - transform.position).normalized;
				}
				break;
			case Whirlwind.State.SlowToStop:
			case Whirlwind.State.Interacting:

				rigidbody.angularVelocity = Vector3.zero;

				Quaternion q = Quaternion.Slerp(rigidbody.rotation, Quaternion.identity, 0.08f);
				rigidbody.rotation = q;
				Orbit(currentState);
				break;
			case Whirlwind.State.StirUp:
				speed = Mathf.Lerp (speed, 10f, 0.01f); // TODO no hardcode
				Orbit(currentState);
				break;
			default:
				break;
		}
	}

	
	void FixedUpdate () {
	}
}