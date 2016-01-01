using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class WhirlwindObject : MonoBehaviour {
	
	// assigned
	public float speed;
	public float height;
	public float radius;
	public float direction;

	// generated
	Vector3 idlePosition;

	enum State { Idle, StirUp, SlowToStop, ContextExam, EnlargeSelect, FullscreenSelect, End, Frozen };
	State currentState;

	// properties
	WhirlwindBelt belt;
	Transform center;
	GameObject trail;
	Vector3 defaultScale;

	// aliases
	Collider collider;
	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		currentState = State.Idle;
	
		defaultScale = transform.localScale;
		Vector3 p = transform.position;
		idlePosition = p;
		center = GameObject.Find("WhirlwindCenter").transform;
		belt = transform.parent.GetComponent<WhirlwindBelt>();
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

	// for when an item is stirred up while whirlwind is in ContextExam state
	IEnumerator CheckWhenToStop () {
		while (true) {
			if (height - transform.position.y < 0.1f) {
				SlowToStopByShift();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}


	// spin around, most important function
	void Orbit () {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;

		if (!collider.enabled) {
			collider.enabled = true;
		}
		
		// vertical velocity
		if (currentState == State.StirUp) {
			if (height - transform.position.y > 0.1f ) {
				dy = speed / 50f;
			}
		}

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
		if (currentState == State.StirUp) {
			xc = 0.17f;
			yc = 0.985f;
		} else {
			xc = 0.0698f;
			yc = 0.998f;
		}

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		Vector3 nv = new Vector3(v.x, dy, v.y) * speed * direction;
		rigidbody.velocity = nv;//Vector3.Lerp(rigidbody.velocity, nv, 0.5f);

		if (currentState == State.ContextExam) {
			if (speed > 0f) {
				//speed = Mathf.Max(0f, speed - 0.05f);
				speed *= 0.9f;
			}
		} else if (currentState == State.SlowToStop) {
			if (speed > 0f) {
				speed *= 0.9f;
			}
		}
	}

/////// public functions for setting whirlwindObject state //////
	// fly into orbit
	public void StirUp (float speed) {
		ResetToIdle();

		Debug.Assert(currentState == State.Idle);

		this.speed = speed;
		rigidbody.useGravity = false;
		collider.enabled = false;
		//trail.GetComponent<ParticleSystem>().Play();
		Vector3 v = new Vector3(RandomAngularVelocityRange, 
														RandomAngularVelocityRange, 
														RandomAngularVelocityRange);
		rigidbody.angularVelocity = v;
		currentState = State.StirUp;
		direction = 1f;
	}

	public void StirUpByShift (float speed) {
		StirUp(speed);
		StartCoroutine(CheckWhenToStop());
	}

	public void SlowToStopByShift () {
		SlowToStop();
		Invoke("ContextExam", Global.TransitionToContextExamTime);
	}

	public void SlowToStop () {
		Debug.Assert(currentState == State.StirUp);

		rigidbody.angularVelocity = Vector3.zero;
		currentState = State.SlowToStop;
	}

	public bool IsInContextExam { get { return currentState == State.ContextExam; } }

	public void ContextExam () {
		Debug.Assert(currentState == State.SlowToStop);

		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
		rigidbody.freezeRotation = true;
		currentState = State.ContextExam;
	}


	public void EndByShift () {
		End();
		Invoke("ResetToIdle", Global.ResetToIdleTime);
	}

	public void End () {
		Debug.Assert(currentState == State.ContextExam);

		currentState = State.End;
		transform.localScale = defaultScale;
		rigidbody.useGravity = true;
		rigidbody.freezeRotation = false;
		Vector3 v = (idlePosition - transform.position).normalized * 30f;
		v.Set(v.x, v.y + 5f, v.z);
		rigidbody.velocity = v;
		v = new Vector3(RandomAngularVelocityRange, 
										RandomAngularVelocityRange, 
										RandomAngularVelocityRange);
		rigidbody.angularVelocity = v;
	}

	public void ResetToIdle () {
		currentState = State.Idle;
		transform.position = idlePosition;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
	}

	public void Freeze () {
		rigidbody.velocity = Vector3.zero;
	}


	// do everything state machine here
	public void ComputeState () {
		Vector3 p = transform.position;

		// state machine transitions
		switch (currentState) {
			case State.SlowToStop:
				Quaternion q = Quaternion.Slerp(rigidbody.rotation, Quaternion.identity, 0.08f);
				rigidbody.rotation = q;
				float h = Mathf.Lerp(p.y, height, 0.08f);
				transform.position = new Vector3(p.x, h, p.z);
				Orbit();
				break;
			case State.ContextExam:
				Orbit();
				break;
			case State.StirUp:
				speed = Mathf.Lerp(speed, Global.StirUpSpeed, 0.02f);
				Orbit();
				break;
			default:
				break;
		}
	}


/////// inherited functions //////
	void OnMouseUp () {
		// TODO do enlarge view here
	}

	
	void FixedUpdate () {
	}
}