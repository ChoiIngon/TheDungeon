using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : Item
{
	public enum PotionType
	{
		Invalid,
		Heal,
		Strength
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

	public virtual void Drink(Unit target)
	{
		throw new System.NotImplementedException();
	}
}

public class HealPotionItem : PotionItem
{
	public new class Meta : PotionItem.Meta
	{
		public override Item CreateInstance()
		{
			return new HealPotionItem(this);
		}
	}
	public HealPotionItem(HealPotionItem.Meta meta) : base(meta)
	{
	}

	public override void Drink(Unit target)
	{
		target.cur_health = target.max_health;
		Util.EventSystem.Publish(EventID.Player_Change_Health);
	}

	public override string description { get { return meta.description; } }
}

public class StranthPotionItem : PotionItem {
	public new class Meta : PotionItem.Meta
	{
		public override Item CreateInstance()
		{
			return new StranthPotionItem(this);
		}
	}
	public StranthPotionItem(StranthPotionItem.Meta meta) : base(meta)
	{
	}

	public override void Drink(Unit target)
	{
		target.attack += 1;
	}

	public override string description { get { return meta.description; } }
}
