﻿using UnityEngine;
using System.Collections;

public class WhirlwindObject : MonoBehaviour {

	// assigned
	[Range(5f, 18.0f)]
	public float speed;

	[Range(2.5f, 9.5f)]
	public float height;

	// state machine
	public enum State { Dormant, FlyToOrbit, Orbit, FlyToGrid, Grid, FlyToDormant };
	public State currentState;


	// generated
	float radius;
	Vector3 dormantPosition;
	public Vector3 gridPosition;

	Transform center;
	GameObject trail;

	// for going up and down
	int verticalCounter;
	int currentVerticalCounterMax;
	bool isGoingUp;



	// Use this for initialization
	void Start () {

		currentState = State.Dormant;
		radius = height / 9f * 5f;

		dormantPosition = GetComponent<Transform>().position;
		center = GameObject.Find("WhirlwindCenter").GetComponent<Transform>();
		trail = GetComponent<Transform>().Find("Trail").gameObject;
		trail.GetComponent<ParticleSystem>().Stop();

		isGoingUp = UnityEngine.Random.Range(0f, 1f) > 0.5f;
	}




	// for setting initial angular velocity
	float RandomAngularVelocityRange { 
		get { 
			float f = 5f * UnityEngine.Random.Range(0.3f, 1f);
			return UnityEngine.Random.Range(0f, 1f) > 0.5f ? f : -f;
		} 
	}
	int NewVerticalCounterMax { get { return (int)UnityEngine.Random.Range(50f, 150f); } }



	public void FlyToOrbit () {
		currentState = State.FlyToOrbit;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Collider>().enabled = false;
		currentVerticalCounterMax = NewVerticalCounterMax;
		trail.GetComponent<ParticleSystem>().Play();
		GetComponent<Rigidbody>().angularVelocity = new Vector3(RandomAngularVelocityRange,
																														RandomAngularVelocityRange, 
																														RandomAngularVelocityRange);
	}



	void Orbit () {
		float xc;
		float yc;
		float dy = 0f;
		Vector2 v, d2, d2n;
		Vector3 p, d;


		if (!GetComponent<Collider>().enabled) {
			GetComponent<Collider>().enabled = true;
		}
		
		// vertical velocity
		if (currentState == State.FlyToOrbit) {
			if (GetComponent<Transform>().position.y < height) {
				dy = speed / 45f;
			}
		} else {
			// go up and down
			if (verticalCounter > currentVerticalCounterMax) {
				verticalCounter = 0;
				isGoingUp = !isGoingUp;	
			}
			verticalCounter++;
			dy = isGoingUp ? 0.1f : -0.1f;
		}

		// d is directional vector to player, d2 is the 2D vector
		p = GetComponent<Transform>().position;
		d = center.position - p;
		d2 = new Vector2(d.x, d.z);

		// small adjustments to prevent objects from escaping orbit
		d2n = d2.normalized;
		float rd = radius - d2.magnitude;
		if (rd > 0.05f) {
			GetComponent<Transform>().position = p - 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		} else if (rd < -0.05f) {
			GetComponent<Transform>().position = p + 0.05f * new Vector3(d2n.x, 0f, d2n.y);
		}

		// rotation based on rotation matrix		
		if (currentState == State.FlyToOrbit) {
			xc = 0.17f;
			yc = 0.985f;
		} else {
			xc = 0.0698f;
			yc = 0.998f;
		}

		v = new Vector2(d2.x * xc + d2.y * yc, d2.x * -yc + d2.y * xc);
		v.Normalize();
		GetComponent<Rigidbody>().velocity = new Vector3(v.x, dy, v.y) * speed * 0.3f;
	}



	public void FlyToDormant () {
		currentState = State.FlyToDormant;
		GetComponent<Rigidbody>().useGravity = true;
		trail.GetComponent<ParticleSystem>().Stop();
		GetComponent<Rigidbody>().velocity = speed * 0.3f * (dormantPosition - GetComponent<Transform>().position).normalized;
	}

/*
	void OnMouseOver () {
		if (Input.GetMouseButtonDown(0) && currentState != State.Grid) {
			GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindObject");
			for (int i = 0; i < gl.Length; i++) {
				gl[i].GetComponent<WhirlwindObject>().currentState = State.Grid;
				gl[i].GetComponent<Rigidbody>().useGravity = false;

				int x = i % 4;
				int y = i / 4;
				float hDistance = 2.7f;
				float vDistance = 2.5f - 0.1f * (float)y;
				GameObject.Find("Structure/Spotlight").GetComponent<Transform>().position = new Vector3(0f, 31f, -12.87756f);

				gl[i].GetComponent<Transform>().position = new Vector3((float)x * hDistance - 4f, 
																															 1.7f - 0.1f *(float)y + (float)y * vDistance,
																															 -5f);
			}
		}
		
	}*/


	public void FlyToGrid () {
		currentState = State.FlyToGrid;
		GetComponent<Rigidbody>().useGravity = false;
	}

	
	void FixedUpdate () {
			
		Vector3 p = GetComponent<Transform>().position;

		// state machine transitions
		switch (currentState) {
			case State.Dormant:
				break;
			case State.FlyToOrbit:
				if (Mathf.Abs(p.y - height) < 1f) { // FlyToOrbit => Orbit
					currentState = State.Orbit;
				} else {
					Orbit();
				}
				break;
			case State.Orbit:
				Orbit();
				break;

			case State.FlyToGrid:
				Vector3 d = gridPosition - p;
				if (d.sqrMagnitude < 0.01f)  { // FlyToGrid => Grid
					currentState = State.Grid;
				} else {
					GetComponent<Rigidbody>().velocity = speed * 0.3f * d.normalized;
				}
				break;
			case State.Grid:
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				GetComponent<Transform>().eulerAngles = Vector3.zero;
				trail.GetComponent<ParticleSystem>().Stop();
				break;
			case State.FlyToDormant:
				if (p.y < 2f) { // FlyToDormant => Dormant
					currentState = State.Dormant;
				} else {
					GetComponent<Rigidbody>().velocity = speed * (dormantPosition - GetComponent<Transform>().position).normalized;
				}
				break;
		}
	}
}
