using UnityEngine;
using System;
using System.Collections;

public class WhirlWindItemData {
	public string[] fields;
	public Sprite sprite;
	public Guid guid;

	public WhirlWindItemData (string[] fields, Sprite sprite, Guid guid) {
		this.fields = fields;
		this.sprite = sprite;
		this.guid = guid;
	}
}