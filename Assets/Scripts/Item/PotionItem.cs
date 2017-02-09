using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PotionItem : Item {
	public PotionItem() : base(Item.Type.Potion) {
	}
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("DRINK");
		actions.Add ("THROW");
		return actions;
	}
	public abstract void Use (Unit target);
}

public class HealingPotionItem : PotionItem {
	public override void Use(Unit target)
	{
		target.Health (target.stats.maxHealth);
	}
	public override Item CreateInstance()
	{
		HealingPotionItem item = new HealingPotionItem ();
		item.id = id;
		item.name = name;
		item.icon = icon;
		item.price = price;
		item.grade = grade;
		return item;	
	}
}

public class PoisonPotionItem : PotionItem {
	public override void Use(Unit target)
	{
		target.buffs.Add (new Buff_Venom (target));
	}
	public override Item CreateInstance()
	{
		PoisonPotionItem item = new PoisonPotionItem ();
		item.id = id;
		item.name = name;
		item.icon = icon;
		item.price = price;
		item.grade = grade;
		return item;	
	}
}