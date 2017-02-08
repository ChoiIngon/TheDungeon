using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEnchantmemt
{
	public ItemEnchantmemt() {
	}
	public abstract void Enchant (Unit target);
}

public class ItemEnchantment_Blaze : ItemEnchantmemt
{
	public ItemEnchantment_Blaze()
	{
	}
	public override void Enchant(Unit target)
	{
		//float chance = (item.level + 1.0f) / 2.0f * (item.level + 3.0f);
		//if (chance >= Random.Range (0, 1)) {
			target.buffs.Add (new Buff_Blaze (target));
		//}
	}
}