using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer {

	float prevActivationTime;
	float cooldownTime;

	public Timer(float c) {
		cooldownTime = c;
		prevActivationTime = -c;
	}

	public float PercentTimePassed { get { return 1f - PercentTimeLeft; } }
	public float TimeLeft { get { return cooldownTime - (Time.time - prevActivationTime); } }
	public bool IsOffCooldown { get { return Time.time - prevActivationTime > cooldownTime; } }
	public float PercentTimeLeft { get { return 1f - TimeLeft / cooldownTime; } }
	public float CooldownTime { get { return cooldownTime; } set { cooldownTime = value; } }
	public void Reset() { prevActivationTime = Time.time; }
}
