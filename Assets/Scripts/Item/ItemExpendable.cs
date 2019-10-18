using System.Collections.Generic;
using UnityEngine;

public class ExpendableItem : Item
{
	public enum ExpendableItemType
	{
		Invalid,
		Potion,
		Scroll,
	}

	public new class Meta : Item.Meta
	{
		public ExpendableItemType expendable_item_type;

		public override Item CreateInstance()
		{
			return new ExpendableItem(this);
		}
	}

	public ExpendableItem(ExpendableItem.Meta meta) : base(meta)
	{
	}

	public virtual void Use(Unit target)
	{
		throw new System.NotImplementedException();
	}
}

public class HealPotionItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new HealPotionItem(this);
		}
	}
	public HealPotionItem(HealPotionItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		target.cur_health = target.max_health;
		Util.EventSystem.Publish(EventID.Player_Stat_Change);
	}
}

public class StranthPotionItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new StranthPotionItem(this);
		}
	}
	public StranthPotionItem(StranthPotionItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		target.stats.AddStat(new Stat.Data() { type = StatType.Attack, value = target.stats.GetStat(StatType.Attack) * 0.01f });
		target.CalculateStat();
	}
}

public class SpeedPotionItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new SpeedPotionItem(this);
		}
	}
	public SpeedPotionItem(SpeedPotionItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		target.stats.AddStat(new Stat.Data() { type = StatType.Speed, value = target.stats.GetStat(StatType.Speed) * 0.01f });
		target.CalculateStat();
	}
}

public class FoodItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new FoodItem(this);
		}
	}

	public FoodItem(FoodItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		//target.cur_health = target.max_health;
		Util.EventSystem.Publish(EventID.Player_Stat_Change);
	}

}

public class MapRevealScrollItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new MapRevealScrollItem(this);
		}
	}

	public MapRevealScrollItem(MapRevealScrollItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		Util.EventSystem.Publish(EventID.Dungeon_Map_Reveal);
	}
}

public class MonsterRevealScrollItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new MonsterRevealScrollItem(this);
		}
	}

	public MonsterRevealScrollItem(MonsterRevealScrollItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		Util.EventSystem.Publish(EventID.Dungeon_Monster_Reveal);
	}
}

public class TreasureRevealScrollItem : ExpendableItem
{
	public new class Meta : ExpendableItem.Meta
	{
		public override Item CreateInstance()
		{
			return new TreasureRevealScrollItem(this);
		}
	}

	public TreasureRevealScrollItem(TreasureRevealScrollItem.Meta meta) : base(meta)
	{
	}

	public override void Use(Unit target)
	{
		Util.EventSystem.Publish(EventID.Dungeon_Treasure_Reveal);
	}
}

public class ExpendableItemManager
{
	public List<ExpendableItem.Meta> item_metas = new List<ExpendableItem.Meta>();
	public void Init()
	{
		{
			HealPotionItem.Meta meta = new HealPotionItem.Meta();
			meta.expendable_item_type = ExpendableItem.ExpendableItemType.Potion;
			meta.type = Item.Type.Expendable;
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
			meta.expendable_item_type = ExpendableItem.ExpendableItemType.Potion;
			meta.type = Item.Type.Expendable;
			meta.id = "ITEM_POTION_STRENGTH";
			meta.name = "Strength Potion";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "An elixir that will permenently increase strength.";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}

		{
			SpeedPotionItem.Meta meta = new SpeedPotionItem.Meta();
			meta.expendable_item_type = ExpendableItem.ExpendableItemType.Potion;
			meta.type = Item.Type.Expendable;
			meta.id = "ITEM_POTION_SPEED";
			meta.name = "Speed Potion";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "An elixir that will permenently increase speed.";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}

		{
			MapRevealScrollItem.Meta meta = new MapRevealScrollItem.Meta();
			meta.expendable_item_type = ExpendableItem.ExpendableItemType.Scroll;
			meta.type = Item.Type.Expendable;
			meta.id = "ITEM_SCROLL_REVEAL_MAP";
			meta.name = "scroll of map";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "reveal map.";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}

		{
			MonsterRevealScrollItem.Meta meta = new MonsterRevealScrollItem.Meta();
			meta.expendable_item_type = ExpendableItem.ExpendableItemType.Scroll;
			meta.type = Item.Type.Expendable;
			meta.id = "ITEM_SCROLL_REVEAL_MONSTER";
			meta.name = "scroll of monster";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "detect monster";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}

		{
			TreasureRevealScrollItem.Meta meta = new TreasureRevealScrollItem.Meta();
			meta.expendable_item_type = ExpendableItem.ExpendableItemType.Scroll;
			meta.type = Item.Type.Expendable;
			meta.id = "ITEM_SCROLL_REVEAL_TREASURE";
			meta.name = "scroll of treasure";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "detect treasure";
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}
	}

	public ExpendableItem CreateRandomItem()
	{
		if (0 == item_metas.Count)
		{
			throw new System.Exception("no potion item info");
		}
		return item_metas[Random.Range(0, item_metas.Count)].CreateInstance() as ExpendableItem;
	}
}