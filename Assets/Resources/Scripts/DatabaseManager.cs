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
				
 				// re ones that are the same as the search term itself
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
				
 				// re ones that are the same as the search terms
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
		List<WhirlwindBeltInfo> retVal = new List<WhirlwindBeltInfo>();

		string[] filenames = {"fml_0206_boxP6", "specol_0117_box14_3", "specol_0123_box2_6", "specol_0315_box199_1", "specol_6044_box7_envelope4", "PR4612_EM37_c2013", "specol_0117_box60", "specol_0123_box2_7", "specol_0315_box199_2", "specol_6047_box1_folder9", "PR462_EA45_1960z", "specol_0123_box1_folder2", "specol_0123_box2_8", "specol_0315_box55_1", "specol_6407_box1_folder1", "specol_0110_box92_1", "specol_0123_box2_1", "specol_0155_Box22", "specol_0315_box55_2", "specol_N7433_4_R543_C43_2006", "specol_0110_box92_2", "specol_0123_box2_2", "Specol_0293_box9", "specol_0315_box55_3", "specol_NC982_5_L5_1562", "specol_0110_box92_3", "specol_0123_box2_3", "specol_0304_box1_2", "specol_0315_box55_4", "specol_PS3566_A92_H23_1996", "specol_0117_box14_1", "specol_0123_box2_4", "specol_0304_box1_3", "specol_0315_box55_5", "specol_0117_box14_2", "specol_0123_box2_5", "specol_0315_box199", "specol_6002_1", "specol_0037_box1_folder23", "specol_0388_box62_2", "specol_N7433_4_G65_M36_1995", "specol_N7433_4_T78J45_2006", "specol_0110_box41_folder3", "specol_6067_box1_folder23", "specol_N7433_4_H34S6_2005", "specol_N7433_4_W6_1996", "specol_0110_box41_folder5", "specol_6067_box1_folder24", "specol_N7433_4_H396_R45_1995", "specol_N7433_4_Y36_N3_2001", "specol_0304_box1_1", "specol_7011_box1", "specol_N7433_4_M558_A84_2002", "specol_N7433_4M86S4_1996", "specol_0315_box55", "specol_GV1235I73_2003", "specol_N7433_4_M558_C67_2010", "specol_N7433_7_L36_A45_2005", "specol_0388_box4_folder10", "specol_GV1525_L36_1851", "specol_N7433_4_M558C67_2010", "specol_PR4611_B63_2012", "specol_0388_box4_folder11", "specol_N7433_3T537_C53_2011", "specol_N7433_4_M86_C38_2005", "specol_PR4854_J88A6_1997", "specol_0388_box4_folder12", "specol_N7433_4_A34_S8_2010", "specol_N7433_4_M86_S2_2005", "specol_PS3554_O884_L3_1994", "specol_0388_box57_folder3_ov10", "specol_N7433_4_B38_I57_1993", "specol_N7433_4_N44_M66_2011", "specol_PS3561_A8612_D44_1998", "specol_0388_box57_folder3_ov3_1", "specol_N7433_4_C44_L54_1996", "specol_N7433_4_O23_D66_1990", "specol_T785_L1_H37_1939", "specol_0388_box57_folder3_ov4", "specol_N7433_4_C44_W67_1999", "specol_N7433_4_O74_S77_2007", "specol_Z105_5_1450_C378", "specol_0388_box57_folder3_ov5", "specol_N7433_4_C55_B66_1998", "specol_N7433_4_R55H67_2004", "specol_Z105_5_1460_C378", "specol_0388_box57_folder3_ov7", "specol_N7433_4_D4285S65_2007", "specol_N7433_4_R83_E53_2006", "specol_0388_box57_folder3_ov8", "specol_N7433_4_F47_L58_2005", "specol_N7433_4_S35_O53_2000", "specol_0388_box62_1", "specol_N7433_4_G648_O26_1992", "specol_N7433_4_T47G55_2011"};

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