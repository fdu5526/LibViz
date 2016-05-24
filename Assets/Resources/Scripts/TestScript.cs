using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestScript : MonoBehaviour {

	public Whirlwind whirlwind;
	Timer endTimer;
	Timer waitTimer;
	BillBoardRenderer billboard;
	Canvas fullscreenSelectionUI; 

	// Use this for initialization
	void Awake () {
		UnityEngine.Random.seed = 0;
		Application.runInBackground = true;
		endTimer = new Timer(120f);
		waitTimer = new Timer(8f);
		billboard = (BillBoardRenderer)Object.FindObjectOfType(typeof(BillBoardRenderer));
		fullscreenSelectionUI = GameObject.Find("FullscreenSelectionUI").GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!waitTimer.IsOffCooldown) {
			return;
		}

		waitTimer.Reset();
		if (whirlwind.CanStirUp) {
			whirlwind.StirUpFromIdle();
		} else if (whirlwind.CanSlowDown) {
			whirlwind.SlowToStopWhirlExam();
			endTimer.Reset();
		} else if (whirlwind.CanEnd) {

			if (!whirlwind.PIsEnlargedOrFullscreen) {
				WhirlwindBelt b = whirlwind.RandomWhirlwindBelt;
				if (!b.IsBeltEmpty) {
					whirlwind.EnterEnlargeSelection(b.RandomWhirlwindItem);
				}
			} else {

				if (!fullscreenSelectionUI.enabled) {
					whirlwind.EnterFullScreen();
					billboard.PlayPause();
				} else {
					whirlwind.ExitFullScreen();
					float r = UnityEngine.Random.value;
					if (r < 0.8f) {
						WhirlwindBelt b = whirlwind.RandomWhirlwindBelt;
						if (!b.IsBeltEmpty) {
							whirlwind.EnterEnlargeSelection(b.RandomWhirlwindItem);
						} else {
							whirlwind.ExitEnlargeSelection(false);
						}
					}
				}
			}

			
			
			

			if (endTimer.IsOffCooldown) {
				whirlwind.End();
				print(Time.timeSinceLevelLoad);
			}
			
		}
	}
}
