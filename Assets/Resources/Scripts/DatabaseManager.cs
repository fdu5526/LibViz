using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour {

	[SerializeField] SQLConnector connector {get { return SQLConnector.Instance;}}

	int numOfFields = Enum.GetNames(typeof(Field)).Length;
	bool connectionSuccess;


	public bool ConnectionSuccess { get { return connectionSuccess; } }

	// call login first
	public void Login() {
		string res = connector.TryConnectSQL();
		connectionSuccess = res == "";
		if (connectionSuccess) {
			Debug.Log("Successful connection with database, using actual data");
		} else {
			Debug.Log("Could not connect to database.Running in debug offline placeholder mode.");
		}
	}

	// search by single bookinfo, this is for enlarge view
	public List<WhirlwindBeltInfo> Search (BookInfo inputInfo, int numBelts) {
		Debug.Assert(inputInfo != null);

		if (connectionSuccess) {
			List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();
			// search by all fields, get all the results
			for (int i = 0; i < numOfFields; i++) {
				Field f = (Field)i;
				
				// search each field
				List<BookInfo> b = connector.Search(inputInfo.GetField(f), f);
				WhirlwindBeltInfo wwbi = new WhirlwindBeltInfo(b, Global.Field2String(f));
				retVal.Add(wwbi);
			}

			// sort the search results by popularity
			retVal.Sort(delegate(WhirlwindBeltInfo b1, WhirlwindBeltInfo b2) { return b2.InfosCount.CompareTo(b1.InfosCount); });

			// return the top N results (pad if necessary)
			retVal = retVal.GetRange(0, numBelts);
			return retVal;
		} else {
			return OfflinePlaceHolderSearch(numBelts);
		}
	}

	// search via the search bar and "explore" button
	public List<WhirlwindBeltInfo> Search (List<BookInfo> inputInfos, int numBelts) {
		Debug.Assert(inputInfos != null && inputInfos.Count > 0);

		if (connectionSuccess) {
			List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();
			// search by all fields, get all the results
			for (int i = 0; i < numOfFields; i++) {
				Field f = (Field)i;
				
				// search each field
				List<string> fields = new List<string>();
				for (int j = 0; j < inputInfos.Count; j++) {
					fields.Add(inputInfos[j].GetField(f));
				}

				List<BookInfo> b = connector.Search(fields, f);
				WhirlwindBeltInfo wwbi = new WhirlwindBeltInfo(b, Global.Field2String(f));
				retVal.Add(wwbi);
			}

			// sort the search results by popularity
			retVal.Sort(delegate(WhirlwindBeltInfo b1, WhirlwindBeltInfo b2) { return b2.InfosCount.CompareTo(b1.InfosCount); });

			// return the top N results (pad if necessary)
			retVal = retVal.GetRange(0, numBelts);
			return retVal;
		} else {
			return OfflinePlaceHolderSearch(numBelts);
		}
		
	}

	// the default bookInfos that greet a user at the beginning
	public List<WhirlwindBeltInfo> GetDefaultBookInfos (int numBelts) {
		if (connectionSuccess) {
			//TODO actual curated default list here
			return OfflinePlaceHolderSearch(numBelts);
		} else {
			return OfflinePlaceHolderSearch(numBelts);
		}
	}



	// this is only called when database is not connected; creates placeholder data for testing only
	List<WhirlwindBeltInfo> OfflinePlaceHolderSearch (int numBelts) {
		List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();
		for (int i = 0; i < numBelts; i++) {
			int amount = (int)UnityEngine.Random.Range(15f, 30f);
			List<BookInfo> b = new List<BookInfo>();
			for (int j = 0; j < amount; j++) {
				b.Add(OfflinePlaceHolder.RandomBookInfo());
			}
			retVal.Add(new WhirlwindBeltInfo(b, "PLACEHOLDER " + i));
		}

		return retVal;
	}
}