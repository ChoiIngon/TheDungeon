using UnityEngine;

public class Item
{
	public enum Type
	{
		Invalid,
		Equipment,
		Potion,
		Key,
		Scroll,
		Max
	}

	public enum Grade
	{
		Invalid,
		Low,
		Normal,
		High,
		Magic,
		Rare,
		Legendary,
		Max
	}

	[System.Serializable]
	public abstract class Meta
	{
		public string id;
		public string name;
		public Type type;
		public int price;
		public string sprite_path;
		public string description;
		public abstract Item CreateInstance ();
	}

	private Meta _meta;
	private static int ITEM_SEQ = 1;

	public virtual Meta meta
	{
		get {
			return _meta;
		}
	}

	public int item_seq = 0;
	public int slot_index = -1;
	public Grade grade = Item.Grade.Invalid;

	public Item(Meta meta)
	{
		_meta = meta;
		item_seq = ITEM_SEQ++;
	}

	public virtual string description { get { throw new System.NotImplementedException(); } }
}
	
