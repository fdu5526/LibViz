using UnityEngine;
using System.Collections;

public class Whirlwind : MonoBehaviour {

	WhirlwindObject[] wb;

	// Use this for initialization
	void Start () {
		GameObject[] gl = GameObject.FindGameObjectsWithTag("WhirlwindObject");
		wb = new WhirlwindObject[gl.Length];

		for (int i = 0; i < gl.Length; i++) {
			wb[i] = gl[i].GetComponent<WhirlwindObject>();
		}
	}


	void InteractWithWhirlwind () {
		if (Input.GetKeyDown("space")) {
			if (wb[0].currentState == WhirlwindObject.State.Dormant) {
				for (int i = 0; i < wb.Length; i++) {
					wb[i].FlyInto();
				}
			} else if (wb[0].currentState == WhirlwindObject.State.Orbit) {
				for (int i = 0; i < wb.Length; i++) {
					wb[i].FlyBack();
				}
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		InteractWithWhirlwind();
	}
}
