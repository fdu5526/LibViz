using UnityEngine;
using System.Collections;

public class WhirlwindHandler : UserDetectHandler {

	public Whirlwind whirlwind;

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
			StartCoroutine( TryStirUp ());
		}
	}

	IEnumerator TryStirUp()
	{
		if ( whirlwind == null )
			yield break;
		
		while ( ! whirlwind.CanStirUp ) {
			yield return null;
		}

		whirlwind.StirUpFromIdle();
	}

	public override void UserCome ()
	{
		if ( whirlwind != null )
		{
			StartCoroutine( TrySlowDown());
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
			StartCoroutine( TryEnd());
		}
	}


	IEnumerator TryEnd()
	{
		if ( whirlwind == null )
			yield break;

		while ( ! whirlwind.CanEnd ) {
			yield return null;
		}

		whirlwind.End();
	}
}
