using System.Runtime.InteropServices;
using UnityEngine;

/*
 * information here is for placeholder, in case database is not connected to
 */
public class OfflinePlaceHolder
{
	static BookInfo bookInfo1 = new BookInfo();
	static BookInfo bookInfo2 = new BookInfo();
	static BookInfo bookInfo3 = new BookInfo();
	static BookInfo bookInfo4 = new BookInfo();
	static BookInfo bookInfo5 = new BookInfo();


	public static BookInfo[] bookInfos = 
		{ bookInfo1, bookInfo2, bookInfo3, bookInfo4, bookInfo5 };


	public static BookInfo RandomBookInfo () {
		return bookInfos[(int)UnityEngine.Random.Range(0f, (float)bookInfos.Length)];
	}
}
