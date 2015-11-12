using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	public bool isCounterClockwise;
	public float speed;


	Transform center;
	float sqrDistanceFromCenter;

	int verticalCounter;
	bool isGoingUp;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().angularVelocity = new Vector3(RandomAngularVelocityRange,
																														RandomAngularVelocityRange, 
																														RandomAngularVelocityRange);
		center = GameObject.Find("Center").GetComponent<Transform>();


		Vector3 v = center.position - GetComponent<Transform>().position;
		sqrDistanceFromCenter = new Vector2(v.x, v.z).sqrMagnitude;


		isGoingUp = UnityEngine.Random.Range(0f, 1f) > 0.5f;
	}

	// for setting initial angular velocity
	float RandomAngularVelocityRange { get { return 5f * UnityEngine.Random.Range(-1f, 1f); } }

	
	// Update is called once per frame
	void FixedUpdate () {
		Rigidbody rigidbody = GetComponent<Rigidbody>();


		// go up and down
		if (verticalCounter > 100) {
			verticalCounter = 0;
			isGoingUp = !isGoingUp;
		}
		verticalCounter++;
		
		// d is directional vector to player, d2 is the 2D vector
		Vector3 p = GetComponent<Transform>().position;
		Vector3 d = center.position - p;
		Vector2 d2 = new Vector2(d.x, d.z);


		// small adjustments to prevent objects from rotating into the center
		if (sqrDistanceFromCenter - d2.sqrMagnitude > 0.05f) {
			Vector2 d2n = d2.normalized;
			GetComponent<Transform>().position = p - 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		}
		
		// rotation based on rotation matrix
		Vector2 v;
		if (isCounterClockwise) { 
			v = new Vector2(d2.x * 0.0698f + d2.y * 0.998f, d2.x * -0.998f + d2.y * 0.0698f);
		} else {
			v = new Vector2(d2.x * 0.0698f + d2.y * -0.998f, d2.x * 0.998f + d2.y * 0.0698f);
		}
		v.Normalize();
		rigidbody.velocity = new Vector3(v.x, isGoingUp ? 0.06f : -0.06f, v.y) * speed;
	}
}
