using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using OpenNI;

public class UserDetector : MonoBehaviour {

	[SerializeField] OpenNIUserTracker tracker;
	[SerializeField] UserDetectHandler handler;
	[SerializeField] float comeRelativeTreshod = 20f;
	[SerializeField] float stopAbstractTreshod = - 400f;

	public enum State
	{
		None,
		Pass,
		Come,
		Stop,
	}

	State m_state = State.None;
	State state {
		get {
			return m_state;
		}
		set {
			if (m_state != value) {
				positionList.Clear ();
				m_state = value;
			}
			if (value == State.None) {
				oriPosition = Vector3.zero;
			}
		}
	}

	void Awake()
	{
	}

	void Update()
	{
		if (state != State.None) {
			UpdateUserInfo ();		
		}

	}

	[SerializeField] int cachePositionNum = 5;
	List<Vector3> positionList = new List<Vector3>();


	Vector3 oriPosition;
	Vector3 currentPosition;
	int temID;

	void UpdateUserInfo()
	{
		currentPosition = tracker.GetUserCenterOfMass (temID);
	

		// save the current position in the list
		if ( positionList.Count >= cachePositionNum )
			positionList.RemoveAt (0);
		positionList.Add (currentPosition);


		// if the state is pass, then test the close
		if ( state == State.Pass )
		{
			if (positionCloser(  )) {
				positionList.Clear ();
				Come ();
			}
		}

		// if the state is come, then test the stop
		if (state == State.Come) {
			if ( positionUnmove () ) {
				positionList.Clear ();
				Stop ();
			}
		}
	}

	IEnumerator RecordOriPosition()
	{
		while ( oriPosition.Equals(Vector3.zero) ) {
			yield return null;
			oriPosition = tracker.GetUserCenterOfMass (temID);
		}
	}

	public bool positionCloser()
	{
		Vector3 move = currentPosition - oriPosition;

		Debug.Log ("Check closer" + move);
		if (move.z > comeRelativeTreshod || currentPosition.z > stopAbstractTreshod )
			return true;

		return false;
//		if (positionList.Count < cachePositionNum)
//			return false;
//		for (int i = 0; i < positionList.Count - 1 ; ++i) {
//			if (!(positionList [i].z < positionList [i + 1].z))
//				return false;
//		}
//		return true;
	}

	public bool positionUnmove()
	{
		Debug.Log ("CheckUnmove " + currentPosition);
		if (currentPosition.z > stopAbstractTreshod)
			return true;
		return false;
//		if (positionList.Count < cachePositionNum)
//			return false;
//		for (int i = 0; i < positionList.Count - 1 ; ++i) {
//			if ( ! ( (positionList [i] - positionList [i + 1]).magnitude < 0.1f  ) ) 
//				return false;
//		}
//		return true;
	}

	public void UserDetected( NewUserEventArgs e )
	{
		if (state == State.None) {
			temID = e.ID;
			StartCoroutine (RecordOriPosition ());
			Pass ();
		}

	}

	public void UserLost( UserLostEventArgs e )
	{
		if (e.ID == temID) {
			Leave ();
		}
	}



	public void Pass()
	{
		state = State.Pass;
		if ( handler != null )
			handler.UserPass ();
	}

	public void Come()
	{
		state = State.Come;
		if ( handler != null )
			handler.UserCome ();
	}

	public void Stop()
	{
		state = State.Stop;
		if ( handler != null )
			handler.UserStop ();
	}

	public void Leave()
	{
		state = State.None;
		if (handler != null)
			handler.UserLeave ();
	}
}
