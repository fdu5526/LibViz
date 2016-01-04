using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class WhirlwindObject : MonoBehaviour {
	
	// assigned
	public float speed;
	public float height;

	// generated
	Vector3 idlePosition;

	enum State { Idle, StirUp, SlowToStop, ContextExam, EnlargeSelect, FullscreenSelect, Frozen };
	State currentState;
	
	public Transform slot;
	bool isLockedToSlot;

	// properties
	Whirlwind whirlwind;
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
		isLockedToSlot = false;
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		belt = transform.parent.GetComponent<WhirlwindBelt>();
		trail = transform.Find("Trail").gameObject;
		trail.GetComponent<ParticleSystem>().Stop();

		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}


/////// private helper functions //////
	// for setting initial angular velocity
	float RandomAngularVelocityRange { 
		get { 
			float f = 2f * UnityEngine.Random.Range(0.3f, 1f);
			return UnityEngine.Random.Range(0f, 1f) > 0.5f ? f : -f;
		}
	}

	// for when an item is stirred up while whirlwind is in ContextExam state
	IEnumerator CheckWhenToStop () {
		while (currentState == State.StirUp) {
			if (height - transform.position.y < 1f) {
				SlowToStopByShift();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	// lock onto slot, hopefully slot isn't null
	void LockToSlot () {
		Debug.Assert(slot != null);

		isLockedToSlot = true;
		rigidbody.velocity = Vector3.zero;
	}

/////// public functions for setting whirlwindObject state //////
	// fly into orbit
	public void StirUp (float speed, Transform slot) {
		ResetToIdle();
		
		Debug.Assert(currentState == State.Idle);
		Debug.Assert(slot != null);
		Debug.Assert(slot.GetComponent<WhirlwindBeltSlot>() != null);

		this.speed = speed;
		this.slot = slot;
		this.slot.GetComponent<WhirlwindBeltSlot>().AttachWwObject(this);
		rigidbody.useGravity = false;
		collider.enabled = false;
		//trail.GetComponent<ParticleSystem>().Play();
		Vector3 v = new Vector3(RandomAngularVelocityRange, 
														RandomAngularVelocityRange, 
														RandomAngularVelocityRange);
		rigidbody.angularVelocity = v;
		currentState = State.StirUp;
	}

	public void StirUpByShift (float speed, Transform slot) {
		StirUp(speed, slot);
		StartCoroutine(CheckWhenToStop());
	}

	public void SlowToStopByShift () {
		LockToSlot();
		SlowToStop();
		ContextExam();
	}

	public void SlowToStop () {
		Debug.Assert(isLockedToSlot);

		rigidbody.angularVelocity = Vector3.zero;
		currentState = State.SlowToStop;
	}

	public bool IsInWhirlwind { get { return slot != null; } }
	public bool IsInContextExam { get { return currentState == State.ContextExam; } }

	public void ContextExam () {
		Debug.Assert(currentState == State.SlowToStop);

		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
		rigidbody.freezeRotation = true;
		currentState = State.ContextExam;
		slot.GetComponent<WhirlwindBeltSlot>().ContextExam();
	}

	public void Enlarge () {
		currentState = State.EnlargeSelect;
		whirlwind.Freeze();
		transform.localScale = defaultScale * 2f;
	}

	public void End () {
		isLockedToSlot = false;
		slot.GetComponent<WhirlwindBeltSlot>().DetachWwObject();
		slot = null;
		currentState = State.Idle;
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
		currentState = State.Frozen;
		rigidbody.velocity = Vector3.zero;
	}

	public void UnFreeze () {
		Debug.Assert(currentState == State.Frozen);
		currentState = State.ContextExam;
	}


	// do everything state machine here
	public void ComputeState () {
		Vector3 p = transform.position;

		// state machine transitions
		switch (currentState) {
			case State.ContextExam:
			case State.SlowToStop:
				Debug.Assert(isLockedToSlot);

				Quaternion q = Quaternion.Slerp(rigidbody.rotation, Quaternion.identity, 0.08f);
				rigidbody.rotation = q;
				break;
			case State.StirUp:
				Debug.Assert(slot != null);

				Vector3 d = (slot.position - p);
				if (!isLockedToSlot && d.sqrMagnitude < 10f) { // dock at slot
					LockToSlot();
				} else if (!isLockedToSlot) {
					speed = Mathf.Lerp(speed, Global.StirUpSpeed, 0.02f);
					rigidbody.velocity = d.normalized * speed;
				}
				break;
			default:
				break;
		}

		if (isLockedToSlot) {
			Debug.Assert(slot != null);
			transform.position = Vector3.Lerp(transform.position, slot.position, 0.5f);
		}
	}


/////// inherited functions //////	
	void FixedUpdate () {
	}
}