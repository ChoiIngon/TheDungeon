using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonPlayer : Unit {
    public UIGaugeBar health;
    public UIGaugeBar exp;
    public UIInventory inventory;
    public override void Init()
    {
        base.Init();
		level = 1;
        exp.max = level;
        exp.current = 0;
        health.current = health.max = Player.Instance.GetStat().health;
        inventory.Init();
    }

    public EquipmentItem EquipItem(EquipmentItem item, int index)
    {
        EquipmentItem prev = Player.Instance.EquipItem(item, index);
        health.max = Player.Instance.GetStat ().health;
        inventory.playerInfo.Init ();
        return prev;
    }
    public EquipmentItem UnequipItem(EquipmentItem.Part category, int index)
    {
        EquipmentItem item = Player.Instance.UnequipItem(category, index);
        health.max = Player.Instance.GetStat ().health;
        inventory.playerInfo.Init ();
        return item;
    }

    public IEnumerator AddExp(int amount)
    {
		int incExp = amount + (int)exp.current;
		exp.current = 0;
		while (exp.max <= incExp) {
			yield return StartCoroutine(exp.DeferredValue(exp.max, 0.25f));
			incExp -= (int)exp.max;
			level += 1;
			exp.max += level;
			exp.current = 0;
			health.current = health.max;
			// levelup effect
		}
		yield return StartCoroutine(exp.DeferredValue(incExp, 0.1f));
    }
    public override void Attack(Unit defender)
    {
        Unit.Stat stat = Player.Instance.GetStat();
        int attack = (int)Mathf.Max(1, stat.attack + Random.Range(-stat.attack * 0.1f, stat.attack * 0.1f) - defender.stats.defense);
        if (stat.critcal / 100.0f >= Random.Range(0.0f, 1.0f))
        {
            attack *= 3;
            // critical effect
        }

        for (int i = 0; i < 2; i++)
        {
            EquipmentItem weapon = Player.Instance.GetEquipment(EquipmentItem.Part.Hand, i);
            if (null != weapon && null != weapon.enchantment)
            {
                weapon.enchantment.Enchant(defender);
            }
        }
        defender.Damage(attack);
    }
    public override void Damage(int damage)
    {
        // proc damage
		StartCoroutine(health.DeferredValue(health.current-damage, 0.2f));
		// damage prefab
    }
    public override void Health(int health)
    {
        StartCoroutine(this.health.DeferredValue((float)health, 0.2f));
    }
       
    public override Stat GetStat()
    {
        return Player.Instance.GetStat();
    }
}
