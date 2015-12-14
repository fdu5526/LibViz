using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	WhirlwindObject[] wb;
	GameObject frontLight;

	// Use this for initialization
	void Start () {
		frontLight = GameObject.Find("Structure/FrontLight");
		GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindObject");
		wb = new WhirlwindObject[gl.Length];

		for (int i = 0; i < gl.Length; i++) {
			wb[i] = gl[i].GetComponent<WhirlwindObject>();
		}
	}


	void InteractWithWhirlwind () {
		if (Input.GetKeyDown("a") &&
				wb[0].currentState == WhirlwindObject.State.Dormant) {
			frontLight.GetComponent<Light>().enabled = false;
			for (int i = 0; i < wb.Length; i++) {
				wb[i].FlyToOrbit();
			}
		} else if (Input.GetKeyDown("s") &&
							 wb[0].currentState == WhirlwindObject.State.Orbit) {
			frontLight.GetComponent<Light>().enabled = false;
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
