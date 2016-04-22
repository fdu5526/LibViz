using UnityEngine;
using System.Collections;

public class WhirlwindHandler : UserDetectHandler {

	public Whirlwind whirlwind;
	Coroutine stageCor = null;

	void Awake()
	{
		if ( whirlwind == null )
		{
			GameObject wwObj = GameObject.Find( "WhirlwindCenter" );
			if ( wwObj != null )
				whirlwind = wwObj.GetComponent<Whirlwind>();
		}
	}

	public override void UserPass ()
	{
		if ( whirlwind != null )
		{
			if ( stageCor != null )
				StopCoroutine (stageCor);
			stageCor = StartCoroutine( TryStirUp ());
		}
	}

	IEnumerator TryStirUp()
	{
		Debug.Log ("StirUp");
		if ( whirlwind == null )
			yield break;
		
		while ( ! whirlwind.CanStirUp ) {
			yield return null;
		}

		whirlwind.StirUpFromIdle();
	}

	public override void UserCome ()
	{
		Debug.Log ("SlowDown");
		if ( whirlwind != null )
		{
			if ( stageCor != null )
				StopCoroutine (stageCor);
			stageCor = StartCoroutine( TrySlowDown());
		}
	}

	IEnumerator TrySlowDown()
	{
		if ( whirlwind == null )
			yield break;

		while ( ! whirlwind.CanSlowDown ) {
			yield return null;
		}

		whirlwind.SlowToStopWhirlExam();
	}


	public override void UserStop ()
	{
		if ( whirlwind != null )
		{
			
		}
	}


	public override void UserLeave ()
	{
		if ( whirlwind != null )
		{
			if ( stageCor != null )
				StopCoroutine (stageCor);
			stageCor = StartCoroutine( TryEnd());
		}
	}


	IEnumerator TryEnd()
	{
		Debug.Log ("End");
		if ( whirlwind == null )
			yield break;

		while ( ! whirlwind.CanEnd ) {
			yield return null;
		}

		whirlwind.End();
	}
}
