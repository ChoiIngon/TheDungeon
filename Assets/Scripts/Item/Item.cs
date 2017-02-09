using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemStatTrigger
{
	public abstract bool IsAvailable (Player.Stat stat);
}

public class ItemStatTrigger_LowHealth : ItemStatTrigger
{
	float percent;
	public ItemStatTrigger_LowHealth(float percent)
	{
		this.percent = percent;
	}
	public override bool IsAvailable(Player.Stat stat)
	{
		if (stat.curHealth <= stat.maxHealth * percent) {
			return true;
		}
		return false;
	}
}

public abstract class Item {
	public enum Type {
		Equipment,
		Potion,
		Key
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

	public string 	id;
	public string 	name;
	public Sprite 	icon;
	public Type 	type;
	public Grade 	grade;	
	public int 		price;
	public int		level;
	public string	description;
	public Item(Type type) {
		this.type = type;
	}
	public abstract Item CreateInstance();
	public virtual List<string> Actions() {
		List<string> actions = new List<string> ();
		actions.Add ("DROP");
		return actions;
	}

	public void Pickup()
	{
		Player.Instance.inventory.Put (this);
	}
}
	
