using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class WhirlwindItem : PhysicsBody {
	
	static Dictionary<string,Sprite> itemSprites = new Dictionary<string, Sprite>();

	public static void InitializeItemImages () {
		string path = Application.dataPath + "/Resources/Sprites/Items/";
		DirectoryInfo dir = new DirectoryInfo(path);
 		FileInfo[] info = dir.GetFiles("*.png");

 		for (int i = 0; i < info.Length; i++) {
 			string fileName = info[i].Name;
 			fileName = fileName.Substring(0, fileName.Length - 4);
 			Sprite sprite = Resources.Load<Sprite>("Sprites/Items/" + fileName);
 			itemSprites.Add(fileName, sprite);
 		}
	}

	static Sprite GetItemSprite (string fileName) {
		Sprite s = null;
		itemSprites.TryGetValue(fileName, out s);
		if (s == null) {
			// TODO this should be an assertion
			print(fileName);
		}
		return s;
	}


	// assigned
	float speed;
	float radius;
	float height;
	Transform slot;

	// for searching
	BookInfo bookInfo;

	// generated
	Vector3 idlePosition;

	// state machine
	enum State { Idle, StirUp, SlowToStop, WhirlExam, ContextExam, StirUpByShift, End };
	State currentState;
		
	// internal data reprensentations
	bool isInteractable;
	bool isEnlarged;
	bool isLockedToSlot;
	bool isToBeDestroyed;

	// other stuffs in the scene
	Whirlwind whirlwind;
	WhirlwindBelt belt;
	GameObject itemImage;
	Vector3 defaultScale;
	InputManager inputManager;

	// Use this for initialization
	protected override void Awake () {
		currentState = State.Idle;
	
		defaultScale = transform.localScale;
		whirlwind = GameObject.Find("WhirlwindCenter").GetComponent<Whirlwind>();
		itemImage = transform.Find("ItemImage").gameObject;
		inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
		isLockedToSlot = false;
		slot = null;

		base.Awake();
	}


	// initialize important information passed from the belt this item belongs to
	public void Initialize (WhirlwindBelt belt, float radius, float height, Vector3 idlePosition, BookInfo bookInfo) {
		this.belt = belt;
		this.radius = radius;
		this.height = height;
		this.idlePosition = idlePosition;
		this.bookInfo = bookInfo;

		itemImage.GetComponent<SpriteRenderer>().sprite = GetItemSprite(bookInfo.FileName);
		
		transform.position = idlePosition;
	}


	public BookInfo BookInfo { get { return bookInfo; } }
	public Transform Slot { get { return slot; } }


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
		isEnlarged = true;
		whirlwind.EnterEnlargeSelection(this);
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
		Vector3 v;

		isLockedToSlot = false;
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

	public void SlatedToBeDestroyed () {
		isToBeDestroyed = true;
	}

	public void ResetToIdle () {
		currentState = State.Idle;
		transform.position = idlePosition;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.rotation = Quaternion.identity;
		itemImage.GetComponent<Renderer>().material.color = Color.white;
	}

	public Sprite Sprite {
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
		if (isInteractable) {
			whirlwind.LogUserInput();

			if (currentState == State.ContextExam) {
				belt.SetMouseDownPosition();
			} else if (currentState == State.WhirlExam) {
				whirlwind.SetMouseDownPosition();
			}
		}
	}

	void OnMouseDrag () {
		if (isInteractable) {
			whirlwind.LogUserInput();

			if (inputManager.IsDragging) {
				if (currentState == State.ContextExam) {
					belt.Spin();
				} else if (currentState == State.WhirlExam) {
					whirlwind.Spin();
				}
			}
		}
	}

	void OnMouseUp () {
		if (isInteractable) {
			whirlwind.LogUserInput();

			if (!whirlwind.isBeingSpun && 
					(currentState == State.ContextExam || 
					 currentState == State.WhirlExam))  {
					Enlarge();
			}
			whirlwind.isBeingSpun = false;
		}
	}

	void FixedUpdate () {
		if (IsInWhirlwind) {
			UpdateFade();
		} else if (isToBeDestroyed) {
			Vector3 p = transform.position;
			float d = (p - idlePosition).sqrMagnitude;
			if (d < 1f) {
				Destroy(this.gameObject);
			} else {
				transform.position = Vector3.Lerp(p, idlePosition, 0.05f);
			}
		}
	}
}