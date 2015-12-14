using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	public enum State { Dormant, FlyToOrbit, Orbit, FlyToGrid, Grid, FlyToDormant };
	public State currentState;
	
	WhirlwindObject[] wb;
	GameObject frontLight;

	// Use this for initialization
	void Start () {
		frontLight = GameObject.Find("Structure/FrontLight");
		GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindObject");
		wb = new WhirlwindObject[gl.Length];

		for (int i = 0; i < gl.Length; i++) {
			wb[i] = gl[i].GetComponent<WhirlwindObject>();
			int x = i % 4;
			int y = i / 4;
			float hDistance = 2.7f;
			float vDistance = 2.5f - 0.1f * (float)y;

			wb[i].gridPosition = new Vector3((float)x * hDistance - 4f, 
																	 	1.7f - 0.1f *(float)y + (float)y * vDistance,
																	 	-5f);
		}
	}


	void OnMouseDown () {
		if (wb[0].currentState != WhirlwindObject.State.FlyToGrid ||
			 	wb[0].currentState != WhirlwindObject.State.Grid) {
			frontLight.GetComponent<Light>().enabled = true;
			GetComponent<Collider>().enabled = false;
			for (int i = 0; i < wb.Length; i++) {
				wb[i].FlyToGrid();
			}
		}
	}


	void InteractWithWhirlwind () {
		if (Input.GetKeyDown("a") &&
				wb[0].currentState == WhirlwindObject.State.Dormant) {
			frontLight.GetComponent<Light>().enabled = false;
			GetComponent<Collider>().enabled = true;
			for (int i = 0; i < wb.Length; i++) {
				wb[i].FlyToOrbit();
			}
		} else if (Input.GetKeyDown("s") &&
							 (wb[0].currentState == WhirlwindObject.State.Orbit || 
	 							wb[0].currentState == WhirlwindObject.State.Grid)) {
			frontLight.GetComponent<Light>().enabled = false;
			GetComponent<Collider>().enabled = true;
			for (int i = 0; i < wb.Length; i++) {
				wb[i].FlyToDormant();
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		InteractWithWhirlwind();
	}
}
