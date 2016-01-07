using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class WhirlwindObject : MonoBehaviour {
	
	// assigned
	public float speed;
	public float radius;
	public float height;
	public Transform slot;
	public bool isBeltBeingDragged;

	// generated
	Vector3 idlePosition;

	enum State { Idle, StirUp, SlowToStop, WhirlExam, ContextExam, End, Frozen };
	State currentState;
	
	bool isInteractable;
	bool isEnlarged;
	bool isLockedToSlot;
	

	// properties
	Whirlwind whirlwind;
	WhirlwindBelt belt;
	GameObject trail;
	GameObject actualObject;
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
		trail = transform.Find("Trail").gameObject;
		actualObject = transform.Find("Object").gameObject;
		trail.GetComponent<ParticleSystem>().Stop();

		collider = GetComponent<Collider>();
		rigidbody = GetComponent<Rigidbody>();
	}


	public void Initialize (WhirlwindBelt belt, float radius, float height) {
		this.belt = belt;
		this.radius = radius;
		this.height = height;
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


	void UpdateFade () {
		Color c = actualObject.GetComponent<Renderer>().material.color;
		c.a = 1f - (transform.position.z + radius) / (2f * radius);
		actualObject.GetComponent<Renderer>().material.color = c;
	}

/////// functions for setting whirlwindObject state //////
	// fly into orbit
	public void StirUp (float speed, Transform slot) {
		ResetToIdle();
		
		Debug.Assert(currentState == State.Idle);
		Debug.Assert(slot != null);
		Debug.Assert(slot.GetComponent<WhirlwindBeltSlot>() != null);

		this.speed = speed;
		this.slot = slot;
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

		collider.enabled = true;
		rigidbody.angularVelocity = Vector3.zero;
		currentState = State.SlowToStop;
	}

	public bool IsInWhirlwind { get { return slot != null; } }


	public void WhirlExam () {
		Debug.Assert(currentState == State.SlowToStop);

		isInteractable = true;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
		rigidbody.freezeRotation = true;
		currentState = State.WhirlExam;
	}


	public void ContextExam () {
		//Debug.Assert(currentState == State.SlowToStop);
		if (currentState != State.SlowToStop) {
			print(currentState);
		}
		
		isInteractable = true;
		currentState = State.ContextExam;
	}

	public void Enlarge () {
		whirlwind.EnterEnlargeSelection(this, slot);
		slot = null;
		//isLockedToSlot = false;
		isEnlarged = true;
	}

	public void UnEnlarge (Transform slot) {
		this.slot = slot;
		//isLockedToSlot = true;
		isEnlarged = false;
	}

	public void FullScreen () {
		actualObject.GetComponent<Renderer>().enabled = false;
	}

	public void UnFullScreen () {
		actualObject.GetComponent<Renderer>().enabled = true;
	}

	public void End () {
		Vector3 v;

		isLockedToSlot = false;
		slot = null;
		isInteractable = false;
		currentState = State.End;
		transform.localScale = defaultScale;
		collider.enabled = true;
		rigidbody.useGravity = true;
		rigidbody.freezeRotation = false;
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
		isInteractable = false;
		rigidbody.velocity = Vector3.zero;
	}

	public void UnFreeze () {
		if (!isInteractable) {
			isInteractable = true;
		}
	}

	// do everything state machine here
	public void ComputeState () {
		if (isEnlarged) {
			return;
		}

		Vector3 p = transform.position;

		// state machine transitions
		switch (currentState) {
			case State.End:
				if ((idlePosition - p).sqrMagnitude < 1f) {
					currentState = State.Idle;
				} else {
					transform.position = Vector3.Lerp(p, idlePosition, 0.1f);
				}
				break;
			case State.ContextExam:
			case State.WhirlExam:
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
					speed = Mathf.Lerp(speed, slot.GetComponent<WhirlwindBeltSlot>().speed, 0.02f);
					rigidbody.velocity = d.normalized * speed;
					//transform.position = Vector3.Lerp(p, slot.position, 0.02f);
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
	void OnMouseDown () {
		if (!isInteractable || whirlwind.isFrozen) {
			return;
		}

		if (currentState == State.ContextExam) {
			belt.SetMouseDownPosition();
		} else if (currentState == State.WhirlExam) {
			whirlwind.SetMouseDownPosition();
		}
	}

	void OnMouseDrag () {
		if (!isInteractable || whirlwind.isFrozen) {
			return;
		}

		if (currentState == State.ContextExam) {
			belt.Spin();
		} else if (isEnlarged) {
			
			// TODO draggable to desk here

		} else if (currentState == State.WhirlExam) {
			whirlwind.Spin();
		}
	}

	void OnMouseUp () {
		if (!isInteractable || whirlwind.isFrozen) {
			return;
		}

		if (!isBeltBeingDragged && 
				(currentState == State.ContextExam || 
				 currentState == State.WhirlExam))  {
				Enlarge();
		}

		isBeltBeingDragged = false;		
	}

	void FixedUpdate () {
		if (IsInWhirlwind) {
			UpdateFade();
		}
	}
}