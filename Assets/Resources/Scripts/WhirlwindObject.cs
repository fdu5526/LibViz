using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	public bool isCounterClockwise;
	public float speed;
	Transform center;

	int verticalCounter;
	bool isGoingUp;

	// Use this for initialization
	void Start () {
		//GetComponent<Rigidbody>().velocity = new Vector3(10f, 0f, 0f);
		center = GameObject.Find("Center").GetComponent<Transform>();

		isGoingUp = UnityEngine.Random.Range(0f, 1f) > 0.5f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Rigidbody rigidbody = GetComponent<Rigidbody>();


		if (verticalCounter > 100) {
			verticalCounter = 0;
			isGoingUp = !isGoingUp;
		}


		verticalCounter++;
		
		// d is directional vector to player
		Vector3 p = GetComponent<Transform>().position;
		Vector3 d = center.position - p;
		Vector2 d2 = new Vector2(d.x, d.z);

		// TODO make this better
		if (isCounterClockwise) { 
			d = new Vector2(d2.x * 0.0698f + d2.y * 0.998f, d2.x * -0.998f + d2.y * 0.0698f);
		} else {
			d = new Vector2(d2.x * 0.0698f + d2.y * -0.998f, d2.x * 0.998f + d2.y * 0.0698f);
		}
		d.Normalize();

		rigidbody.velocity = new Vector3(d.x, isGoingUp ? 0.06f : -0.06f, d.y) * speed;
	}
}
