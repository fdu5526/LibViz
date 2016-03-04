using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WhirlwindBeltSlot : PhysicsBody {
	
	// assigned properties
	public float speed;
	public float height;
	public float radius;
	public float direction;
	
	// internal variables
	bool isStirup;
	bool shouldSlowsDown;
	float slowDownLerpFactor;
	float baseSlowDownLerpFactor = 0.1f;

	bool isGoingUp;
	int bobbleLifespan;
	const int maxBobbleLifespan = 30;

	// properties
	Transform center;

	// Use this for initialization
	protected override void Awake () {
		center = GameObject.Find("WhirlwindCenter").transform;
		shouldSlowsDown = false;
		speed = 0f;

		base.Awake();
	}

	
/////// private helper functions //////
	// spin around, most important function
	void Orbit () {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;

		// d is directional vector to player, d2 is the 2D vector
		p = transform.position;
		d = center.position - p;
		d2 = new Vector2(d.x, d.z);

		if (isStirup && !shouldSlowsDown) {
			bobbleLifespan--;
			//dy = isGoingUp ? 0.08f : -0.08f;
			if (bobbleLifespan <= 0) {
				bobbleLifespan = maxBobbleLifespan;
				isGoingUp = !isGoingUp;
			}
		} else {
			float h = Mathf.Lerp(p.y, height, 0.5f);
			p.y = h;
			transform.position = p;
		}

		// small corrections to prevent items from escaping orbit
		d2n = d2.normalized;
		float rd = radius - d2.magnitude;
		if (rd > 0.05f) {
			transform.position = p - 0.1f * new Vector3(d2n.x, 0f, d2n.y);
		} else if (rd < -0.05f) {
			transform.position = p + 0.1f * new Vector3(d2n.x, 0f, d2n.y);
		}

		// rotation based on rotation matrix		
		xc = 0.0698f;
		yc = 0.998f;

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		Vector3 nv = new Vector3(v.x, dy, v.y) * speed * direction;
		rigidbody.velocity = nv;

		if (shouldSlowsDown) {
			speed = Mathf.Lerp(speed, 0f, slowDownLerpFactor);
		}
		
	}


/////// public functions for setting whirlwindItem state //////
	public bool IsDoneSlowingDown {
		get {
			return speed < 0.1f;
		}
	}

	public void Initialize (Vector3 position, float height, float radius) {
		transform.position = position;
		this.height = height;
		this.radius = radius;
	}

	public void StirUp () {
		isStirup = true;
		isGoingUp = UnityEngine.Random.Range(0f,1f) > 0.5f;
		speed = Global.SpinSpeed * height / 5f;
		direction = 1f;
		shouldSlowsDown = false;
		slowDownLerpFactor = baseSlowDownLerpFactor;
	}
	
	public void SlowToStopFast (bool isFastStop) {
		shouldSlowsDown = true;
		if (isFastStop) {
			slowDownLerpFactor = 0.3f;
		}
	}

	public void EnableCollider (bool e) {
		collider.enabled = e;
	}

/////// inherited functions //////
	void FixedUpdate () {
		Orbit();
	}
}