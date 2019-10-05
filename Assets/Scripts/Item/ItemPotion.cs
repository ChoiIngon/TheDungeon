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

public class PotionItemManager
{
	private List<PotionItem.Meta> item_metas = new List<PotionItem.Meta>();
	public void Init()
	{
		{
			HealPotionItem.Meta meta = new HealPotionItem.Meta();
			meta.potion_type = PotionItem.PotionType.Heal;
			meta.type = Item.Type.Potion;
			meta.id = "ITEM_POTION_HEALING";
			meta.name = "Healing Potion";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_002";
			meta.description = "An elixir that will instantly return you to full health and cure poison.";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}

		{
			StranthPotionItem.Meta meta = new StranthPotionItem.Meta();
			meta.potion_type = PotionItem.PotionType.Strength;
			meta.type = Item.Type.Potion;
			meta.id = "ITEM_POTION_STRENGTH";
			meta.name = "Strength Potion";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "An elixir that will permenently increase strength.";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}
	}

	public Item CreateRandomItem()
	{
		if (0 == item_metas.Count)
		{
			throw new System.Exception("no potion item info");
		}
		return item_metas[Random.Range(0, item_metas.Count)].CreateInstance();
	}
}