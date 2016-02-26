using System.Runtime.InteropServices;
using UnityEngine;

/*
 * information here is for placeholder, in case database is not connected to
 */
public class OfflinePlaceHolder
{
	static BookInfo bookInfo1 = new BookInfo(
		"A boat beneath a sunny sky","Thomas, Donna",2012,"Santa Cruz, CA","Title from container; imprint from Colophon-Housed in a beige cloth covered slipcase with an illustrated paper label. Illustrations and calligraphy by Donna Thomas. Bound in one-quarter deer hide over Japanese paper boards. Lettering on beige cloth covered  insert","Title from container; imprint from Colophon-Housed in a beige cloth covered slipcase with an illustrated paper label. Illustrations and calligraphy by Donna Thomas. Bound in one-quarter deer hide over Japanese paper boards. Lettering on beige cloth covered  insert","Artists' books-Alice (Fictitious character : Carroll","Specimens-Poetry","-","-","-");
	static BookInfo bookInfo2 = new BookInfo(
		"Bon bon mots","Chen, Julie",1998,"Berkeley, Calif","The box, which is designed to resemble a candy box, contains 3 miniature books (2 of which are accordion folded), 1 folded octagonal object with text, and a small box with text containing 5 copper balls which are to be placed into 5 holes on box's bottom surface. All are designed to resemble pieces of candy and are nested in a cloth which fits over partitioned bottom of box. A leaf with box's contents is mounted on the inside lid-Title from box-Letterpress printed in an edition of 100 copies","The box, which is designed to resemble a candy box, contains 3 miniature books (2 of which are accordion folded), 1 folded octagonal object with text, and a small box with text containing 5 copper balls which are to be placed into 5 holes on box's bottom surface. All are designed to resemble pieces of candy and are nested in a cloth which fits over partitioned bottom of box. A leaf with box's contents is mounted on the inside lid-Title from box-Letterpress printed in an edition of 100 copies","Artists' books-Miniature books-Candy-Candy containers","Specimens-Specimens-Miscellanea-Miscellanea","-","-","-");
	static BookInfo bookInfo3 = new BookInfo(
		"Cathedral","Munson, Howard",2004,"San Francisco","Four double-page pop-ups, of purely imagined cathedrals, which can be viewed one at a time in double-paged spreads-Binding closure buttons with black stretch loops. Boards half bound in red cloth with paper illustrated architectural drawings-Signed by the author","Four double-page pop-ups, of purely imagined cathedrals, which can be viewed one at a time in double-paged spreads-Binding closure buttons with black stretch loops. Boards half bound in red cloth with paper illustrated architectural drawings-Signed by the author","Cathedrals-Pop-up books-Artists' books","Specimens-Specimens","-","-","-");
	static BookInfo bookInfo4 = new BookInfo(
		"Changeling","Riker, Maryann J",2006,"Phillipsburg, N.J","Mixed media artist's sculpture; one-of-a-kind. A cabinet-shaped structure with two illustrated panels (moon, tape measure, butterfly). These unfold and reveal an intimate interior.  Collage on exterior and interior walls; interior contains mirrors on each panel, plus one shelf with mounted seashell. A third mirror and second seashell stand inside the slotted cabinet","Mixed media artist's sculpture; one-of-a-kind. A cabinet-shaped structure with two illustrated panels (moon, tape measure, butterfly). These unfold and reveal an intimate interior.  Collage on exterior and interior walls; interior contains mirrors on each panel, plus one shelf with mounted seashell. A third mirror and second seashell stand inside the slotted cabinet","Artists' books-Metamorphosis in art","Specimens","-","-","-");
	static BookInfo bookInfo5 = new BookInfo(
		"Charles Bukowski papers","Bukowski, Charles",0,"-","-","-","American literature-American literature-Authors, American","-","Archival resources","20th century","California");


	public static BookInfo[] bookInfos = 
		{ bookInfo1, bookInfo2, bookInfo3, bookInfo4, bookInfo5 };


	public static BookInfo RandomBookInfo () {
		//return bookInfos[1];
		return bookInfos[(int)UnityEngine.Random.Range(0f, (float)bookInfos.Length)];
	}
}
