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
			whirlwind.StirUp(Global.StirUpSpeed);
		}

	}

	public override void UserCome ()
	{
		if ( whirlwind != null )
		{
			whirlwind.SlowToStopWhirlExam();
		}
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
			whirlwind.End();
		}

	}
}
