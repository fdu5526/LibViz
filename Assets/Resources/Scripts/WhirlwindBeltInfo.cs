using System.Collections;
using System.Collections.Generic;

public class WhirlwindBeltInfo {
	public List<BookInfo> Infos;
	public string Label;

	public WhirlwindBeltInfo (List<BookInfo> b, string l) {
		this.Infos = b;
		this.Label = l;
	}

	public int InfosCount { get { return Infos.Count; } }
}