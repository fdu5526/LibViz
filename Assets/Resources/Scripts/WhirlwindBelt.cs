using UnityEngine;
using System.Collections;

public class WhirlwindBelt : MonoBehaviour {
	[Range(1, 5)]
	public int level;

	float radius;
	float height;
	float speed;

	public WhirlwindObject[] wwObjs;

	// Use this for initialization
	void Start () {
		height = (float)level * 1.5f;
		radius = height / 9f * 8f;
		speed = 5f;

		for (int i = 0; i < wwObjs.Length; i++) {
			wwObjs[i].speed = speed;
			wwObjs[i].height = height;
			wwObjs[i].radius = radius;
		}
	}

	void OnMouseDrag () {
		// change speed
		if (wwObjs[0].currentState == WhirlwindObject.State.Orbit) {
			for (int i = 0; i < wwObjs.Length; i++) {
				wwObjs[i].speed = speed * 5f;
			}
		}
	}

	
	// Update is called once per frame
	void FixedUpdate () {
	
	}
}
