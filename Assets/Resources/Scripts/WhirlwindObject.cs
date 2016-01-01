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
	
	public Transform marker;
	bool isLockedToMarker;

	// properties
	WhirlwindBelt belt;
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
		isLockedToMarker = false;
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

/////// public functions for setting whirlwindObject state //////
	// fly into orbit
	public void StirUp (float speed, Transform marker) {
		ResetToIdle();

		Debug.Assert(currentState == State.Idle);

		this.speed = speed;
		this.marker = marker;
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

	public void StirUpByShift (float speed, Transform marker) {
		StirUp(speed, marker);
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

		isLockedToMarker = false;
		marker = null;
		currentState = State.End;
		transform.localScale = defaultScale;
		collider.enabled = true;
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
				break;
			case State.ContextExam:
				break;
			case State.StirUp:
				Debug.Assert(marker != null);

				Vector3 d = (marker.position - p);
				if (!isLockedToMarker && d.sqrMagnitude < 10f) { // dock at marker
					isLockedToMarker = true;
				} else if (!isLockedToMarker) {
					speed = Mathf.Lerp(speed, Global.StirUpSpeed, 0.02f);
					rigidbody.velocity = (marker.position - p).normalized * speed;
				}
				break;
			default:
				break;
		}

		if (isLockedToMarker) {
			Debug.Assert(marker != null);
			transform.position = Vector3.Lerp(transform.position, marker.position, 0.5f);
		}
	}


/////// inherited functions //////	
	void FixedUpdate () {
	}
}