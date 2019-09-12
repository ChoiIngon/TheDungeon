using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : Item
{
	public enum PotionType
	{
		Invalid,
		Heal
	}

	public new class Meta : Item.Meta
	{
		public PotionType potion_type;

		public override Item CreateInstance()
		{
			return new PotionItem(this);
		}
	}

	public PotionItem(PotionItem.Meta meta) : base(meta)
	{
	}
}
		/*
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
		target.Health (target.GetStat().health);
	}
	public new class Info : PotionItem.Info
	{
		public override Item CreateInstance()
		{
			HealingPotionItem item = new HealingPotionItem ();
			item.id = id;
			item.name = name;
			item.icon = ResourceManager.Instance.Load<Sprite> (icon);
			item.price = price;
			item.grade = Item.Grade.Normal;
			item.description = description;
			return item;	
		}
	}
}

public class PoisonPotionItem : PotionItem {
	public override void Use(Unit target)
	{
		target.buffs.Add (new Buff_Venom (target));
	}
	public new class Info : PotionItem.Info
	{
		public override Item CreateInstance()
		{
			PoisonPotionItem item = new PoisonPotionItem ();
			item.id = id;
			item.name = name;
			item.icon = ResourceManager.Instance.Load<Sprite> (icon);
			item.price = price;
			item.grade = Item.Grade.Normal;
			item.description = description;
			return item;	
		}
	}
}
		*/