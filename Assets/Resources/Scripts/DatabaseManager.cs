using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour {

	[SerializeField] SQLConnector connector {get { return SQLConnector.Instance;}}

	string[] columnTitles = {"Author", "Date", "Location", "Author Other", "Subject Headings"};
	string[] columns = {"name", "date", "pub_place", "other_author_personal", "subject"};
	int maxItemsPerBelt = 30;
	bool connectionSuccess;
	/*
		2 ways of searching
			drill down - by putting into desk or typing in search
			open up - whirl the whirlwind, repopulate with ||
	*/
	/*
		5 searches
			Author
			Normalized date
			Location
			Subject headings (SearchBySubject())
			Author other
	*/

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

			// search through the columns
			for (int i = 0; i < columns.Length; i++) {
				// make a query
				if (columns[i].Equals("subject")) {
					b = connector.SearchBySubject(inputInfo.GetSubjects());
				} else {
					b = connector.Search(inputInfo.GetData(columns[i]), columns[i]);					
				}
				
 				// remove ones that are the same as the search term itself
 				for (int j = 0; j < b.Count; j++) {
 					if (b[j].FileName.Equals(inputInfo.FileName)) {
 						b.Remove(b[j]);
 						break;
 					}
 				}

 				// limits the items count
				b = b.GetRange(0, Mathf.Min(b.Count, maxItemsPerBelt));
				wwbi = new WhirlwindBeltInfo(b, columnTitles[i]);
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

			List<BookInfo> b;
			WhirlwindBeltInfo wwbi;

			// search through the columns
			for (int i = 0; i < columns.Length; i++) {

				// aggregate the terms of which we search
				List<string> searchTerms = new List<string>();
				for (int j = 0; j < inputInfos.Count; j++) {
					if (columns[i].Equals("subject")) {
						searchTerms.AddRange(inputInfos[j].GetSubjects());
					} else {
						searchTerms.Add(inputInfos[j].GetData(columns[i]));
					}
				}

				// make a query
				if (columns[i].Equals("subject")) {
					b = connector.SearchBySubject(searchTerms);
				} else {
					b = connector.Search(searchTerms, columns[i]);					
				}
				
 				// remove ones that are the same as the search terms
 				for (int j = 0; j < b.Count; j++) {
 					for (int k = 0; k < inputInfos.Count; k++) {
 						if (b[j].FileName.Equals(inputInfos[k].FileName)) {
	 						b.Remove(b[j]);
	 						j--;
	 						break;
	 					}
 					}
 				}

 				// limits the items count
				b = b.GetRange(0, Mathf.Min(b.Count, maxItemsPerBelt));
				wwbi = new WhirlwindBeltInfo(b, columnTitles[i]);
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
			List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();

			// query everything, do not include the ones that we have no images for
			List<BookInfo> results = connector.Search("", columns[0]);
			for (int i = 0; i < results.Count; i++) {
				if (!WhirlwindItem.HasItemSprite(results[i].FileName)) {
					results.Remove(results[i]);
				}
			}

			List<BookInfo> b;
			WhirlwindBeltInfo wwbi;

			int amountPerBelt;
			int prevBeltEndIndex = 0;
			// take chunks out (based on belt sizes) of all the entries so nothing repeats
			for (int i = 0; i < columns.Length; i++) {
				amountPerBelt = 4 + i * 3;
 				b = results.GetRange(prevBeltEndIndex, amountPerBelt);
				wwbi = new WhirlwindBeltInfo(b, columnTitles[i]);
				retVal.Add(wwbi);
				prevBeltEndIndex += amountPerBelt + 1;
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



	// this is only called when database is not connected; creates placeholder data for testing only
	List<WhirlwindBeltInfo> OfflinePlaceHolderSearch (int numBelts) {
		string[] filenames = WhirlwindItem.GetAllItemFileNames();
		List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();

		Debug.Assert(filenames.Length > 0);

		string[] columns = {"title", "title_subtitle", "name", "pub_date", "pub_place", "note", "scope_content", "history"};

		for (int i = 0; i < numBelts; i++) {
			int amount = (int)UnityEngine.Random.Range(15f, 30f);
			List<BookInfo> b = new List<BookInfo>();
			for (int j = 0; j < amount; j++) {
				BookInfo bi = new BookInfo();
				string fn = "placeholder";
				while (fn.Equals("placeholder")) {
					fn = filenames[UnityEngine.Random.Range(0, filenames.Length)];
				}

				bi.AddData("file_name", fn);
				for (int c = 0; c < columns.Length; c++) {
					bi.AddData(columns[c], "PLACEHOLDER");
				}
				b.Add(bi);
			}
			retVal.Add(new WhirlwindBeltInfo(b, "PLACEHOLDER " + i));
		}

		return retVal;
	}
}