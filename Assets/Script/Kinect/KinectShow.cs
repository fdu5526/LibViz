using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KinectShow : UserDetectHandler {

	[SerializeField] Image img;
	[SerializeField] Text text;

	override public void UserPass()
	{
		Color col = Color.green;
		col.a = 0.5f;
		img.color = col;

		text.text = "USER PASS";
		text.color = Color.green;
	}

	override public void UserStop()
	{

		Color col = Color.yellow;
		col.a = 0.5f;
		img.color = col;

		text.text = "USER STOP";
		text.color = Color.yellow;
	}

	override public void UserCome()
	{

		Color col = Color.magenta;
		col.a = 0.5f;
		img.color = col;

		text.text = "USER COME";
		text.color = Color.magenta;
	}

	override public void UserLeave()
	{
		Color col = Color.red;
		col.a = 0.5f;
		img.color = col;

		text.text = "USER LEAVE";
		text.color = Color.red;
	}
}
