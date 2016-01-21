using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class WhirlwindItem : MonoBehaviour {
	
	// assigned
	public float speed;
	public float radius;
	public float height;
	public Transform slot;

	// generated
	public WhirlWindItemData itemData;
	Vector3 idlePosition;

	// state machine
	enum State { Idle, StirUp, SlowToStop, WhirlExam, ContextExam, StirUpByShift, End, Frozen };
	State currentState;
		
	// internal data reprensentations
	bool isInteractable;
	bool isEnlarged;
	bool isLockedToSlot;

	// other stuffs in the scene
	Whirlwind whirlwind;
	WhirlwindBelt belt;
	GameObject itemImage;
	Vector3 defaultScale;

	// aliases
	Collider collider;
	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		Initialize();
	}

	public void Initialize () {
		currentState = State.Idle;
	
		defaultScale = transform.localScale;
		Vector3 p = transform.position;
		idlePosition = p;
		isLockedToSlot = false;
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		itemImage = transform.Find("ItemImage").gameObject;

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

	// lock onto slot, hopefully slot isn't null
	void LockToSlot () {
		Debug.Assert(slot != null);

		isLockedToSlot = true;
		rigidbody.velocity = Vector3.zero;
	}


	void UpdateFade () {
		float a = 1f - (transform.position.z + radius) / (2f * radius);
		Color c = itemImage.GetComponent<Renderer>().material.color;
		c.a = Mathf.Clamp(a, 0.1f, 1f);
		itemImage.GetComponent<Renderer>().material.color = c;
	}


	void DestroyItem () {
		Destroy(this.gameObject);
	}

/////// functions for setting whirlwindItem state //////
	// fly into orbit
	public void StirUp (float speed, Transform slot) {
		ResetToIdle();
		
		Debug.Assert(currentState == State.Idle);
		Debug.Assert(slot != null);
		Debug.Assert(slot.GetComponent<WhirlwindBeltSlot>() != null);

		this.speed = speed;
		this.slot = slot;
		isInteractable = false;
		slot.GetComponent<WhirlwindBeltSlot>().AttachItem();
		rigidbody.useGravity = false;
		collider.enabled = false;
		Vector3 v = new Vector3(RandomAngularVelocityRange, 
														RandomAngularVelocityRange, 
														RandomAngularVelocityRange);
		rigidbody.angularVelocity = v;
		currentState = State.StirUp;
	}

	public void StirUpByShift (float speed, Transform slot) {
		StirUp(speed, slot);
		currentState = State.StirUpByShift;
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

	public bool IsDoneStirUp { get { return slot != null && isLockedToSlot; } }
	public bool IsInWhirlwind { get { return slot != null; } }


	public void WhirlExam () {
		Debug.Assert(currentState == State.SlowToStop || 
								 currentState == State.ContextExam);

		isInteractable = true;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
		rigidbody.freezeRotation = true;
		currentState = State.WhirlExam;
	}


	public void ContextExam () {
		Debug.Assert(currentState == State.SlowToStop);
		
		isInteractable = true;
		currentState = State.ContextExam;
	}

	public void Enlarge () {
		whirlwind.EnterEnlargeSelection(this);
		isEnlarged = true;
	}

	public void UnEnlarge () {
		isEnlarged = false;
	}

	public void FullScreen () {
		Debug.Assert(isEnlarged);
	}

	public void UnFullScreen () {
		Debug.Assert(isEnlarged);
	}

	public void End () {
		Debug.Assert(slot != null);
		Vector3 v;

		isLockedToSlot = false;
		slot.GetComponent<WhirlwindBeltSlot>().DettachItem();
		slot = null;
		isInteractable = false;
		currentState = State.End;
		transform.localScale = defaultScale;
		collider.enabled = true;
		rigidbody.velocity = Vector3.zero;
		rigidbody.useGravity = true;
		rigidbody.freezeRotation = false;
		v = new Vector3(RandomAngularVelocityRange, 
										RandomAngularVelocityRange, 
										RandomAngularVelocityRange);
		rigidbody.angularVelocity = v;
	}

	public void DestroyInSeconds(float s) {
		Invoke("DestroyItem", s);
	}

	public void ResetToIdle () {
		currentState = State.Idle;
		transform.position = idlePosition;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
	}

	public Sprite ItemSprite {
		get {
			return itemImage.GetComponent<SpriteRenderer>().sprite;
		}
		set {
			itemImage.GetComponent<SpriteRenderer>().sprite = value;
		}
	}

	public void SetInteractable (bool isInteractable) {
		this.isInteractable = isInteractable;
		if (!isInteractable) {
			rigidbody.velocity = Vector3.zero;
		}
	}

	// do everything state machine here
	public void ComputeState () {
		/*if (isEnlarged) {
			return;
		}*/

		Vector3 d;
		Vector3 p = transform.position;

		// state machine transitions
		switch (currentState) {
			case State.End:
				if ((idlePosition - p).sqrMagnitude < 1f) {
					ResetToIdle();
				} else {
					transform.position = Vector3.Lerp(p, idlePosition, 0.1f);
				}
				break;
			case State.StirUpByShift:
				Debug.Assert(slot != null);

				d = (slot.position - p);
				if (height - p.y < 1f) {
					SlowToStopByShift();
				} else {
					speed = Mathf.Lerp(speed, slot.GetComponent<WhirlwindBeltSlot>().speed, 0.02f);
					rigidbody.velocity = d.normalized * speed;
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

				d = (slot.position - p);
				if (!isLockedToSlot && d.sqrMagnitude < 10f) { // dock at slot
					LockToSlot();
				} else if (!isLockedToSlot) {
					speed = Mathf.Lerp(speed, slot.GetComponent<WhirlwindBeltSlot>().speed, 0.02f);
					rigidbody.velocity = d.normalized * speed;
					transform.position = Vector3.Lerp(p, slot.position, 0.02f);
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
		whirlwind.LogUserInput();

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

		whirlwind.LogUserInput();

		if (currentState == State.ContextExam) {
			belt.Spin();
		} else if (currentState == State.WhirlExam) {
			whirlwind.Spin();
		}
	}

	void OnMouseUp () {
		if (!isInteractable || whirlwind.isFrozen) {
			return;
		}
		whirlwind.LogUserInput();

		if (!whirlwind.isBeingSpun && 
				(currentState == State.ContextExam || 
				 currentState == State.WhirlExam))  {
				Enlarge();
		}
		whirlwind.isBeingSpun = false;
	}

	void FixedUpdate () {
		if (IsInWhirlwind) {
			UpdateFade();
		}
	}
}