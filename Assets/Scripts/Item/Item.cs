using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item {
	public enum Type {
		Equipment,
		Potion,
		Key,
		Scroll,
		Max
	}
	public enum Grade {
		Low,
		Normal,
		High,
		Magic,
		Rare,
		Legendary,
		Max
	}

	[System.Serializable]
	public abstract class Info
	{
		public string id;
		public string name;
		public string icon;
		public int price;
		public int grade;
		public string description;
		public abstract Item CreateInstance ();
	}
	public string 	id;
	public string 	name;
	public Sprite 	icon;
	public Type 	type;
	public int 		price;
	public Grade	grade;
	public string	description;

	public Item(Type type) {
		this.type = type;
	}

	public virtual List<string> Actions() {
		List<string> actions = new List<string> ();
		actions.Add ("DROP");
		return actions;
	}
}
	
