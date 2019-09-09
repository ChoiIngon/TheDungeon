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
		Debug.Log("create item(item_id:" + meta.id + ", item_seq:" + item_seq + ")");
	}

	public virtual void Equip(Unit unit)
	{
		throw new System.InvalidOperationException("can not equip item(item_id:" + meta.id + ", item_seq:" + item_seq + ")");
	}

	public virtual void Unequip()
	{
		throw new System.InvalidOperationException("can not unequip item(item_id:" + meta.id + ", item_seq:" + item_seq + ")");
	}

	public virtual void Delete(int count)
	{
		throw new System.InvalidOperationException("can not delete item(item_id:" + meta.id + ", item_seq:" + item_seq + ")");
	}

	public virtual void Sell(int count)
	{
		throw new System.InvalidOperationException("can not sell item(item_id:" + meta.id + ", item_seq:" + item_seq + ")");
	}

	public virtual void Use(Unit unit)
	{
		throw new System.InvalidOperationException("can not use item(item_id:" + meta.id + ", item_seq:" + item_seq + ")");
	}
    public virtual string description { get { throw new System.NotImplementedException(); } }
}
	
