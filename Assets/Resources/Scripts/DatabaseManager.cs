using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour {

	[SerializeField] SQLConnector connector {get { return SQLConnector.Instance;}}

	string[] columns = {"name", "date", "pub_place", "other_author_personal"};
	int maxItemsPerBelt = 30;
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

			List<BookInfo> b;
			WhirlwindBeltInfo wwbi;

			// search through the 4 columns
			for (int i = 0; i < columns.Length; i++) {
 				b = connector.Search(inputInfo.GetData(columns[i]), columns[i]);
				b = b.GetRange(0, Mathf.Min(b.Count, maxItemsPerBelt));
				wwbi = new WhirlwindBeltInfo(b, columns[i]);
				retVal.Add(wwbi);
			}

			// search by subject
			b = connector.SearchBySubject(inputInfo.GetSubjects());
			b = b.GetRange(0, maxItemsPerBelt);
			wwbi = new WhirlwindBeltInfo(b, "subjects");
			retVal.Add(wwbi);

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
			//TODO

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
			List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();

			List<BookInfo> b;
			WhirlwindBeltInfo wwbi;
			string column;

			for (int i = 0; i < columns.Length; i++) {
 				b = connector.Search("", columns[i]);
				b = b.GetRange(0, Mathf.Min(b.Count, maxItemsPerBelt));
				wwbi = new WhirlwindBeltInfo(b, columns[i]);
				retVal.Add(wwbi);
			}

			b = connector.SearchBySubject("");
			b = b.GetRange(0, maxItemsPerBelt);
			wwbi = new WhirlwindBeltInfo(b, "subjects");
			retVal.Add(wwbi);

			// sort the search results by popularity
			retVal.Sort(delegate(WhirlwindBeltInfo b1, WhirlwindBeltInfo b2) { return b2.InfosCount.CompareTo(b1.InfosCount); });

			// return the top N results (pad if necessary)
			retVal = retVal.GetRange(0, numBelts);
			return retVal;
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