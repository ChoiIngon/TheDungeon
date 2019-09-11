using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonPlayer : MonoBehaviour
{
	public UIGaugeBar health;
	public UIGaugeBar exp;
	/*
	public IEnumerator AddExp(int amount)
	{
		int incExp = amount + (int)exp.current;
		exp.current = 0;
		while (exp.max <= incExp)
		{
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

	private void OnChangeExp()
	{
		int incExp = amount + (int)exp.current;
		exp.current = 0;
		while (exp.max <= incExp)
		{
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
	/*
    
    

    
*/    
    public void Attack(Unit defender)
    {
        int attack = (int)Mathf.Max(1, GameManager.Instance.player.attack + Random.Range(-GameManager.Instance.player.attack * 0.1f, GameManager.Instance.player.attack * 0.1f) - defender.defense);
        if (GameManager.Instance.player.critcal >= Random.Range(0.0f, 100.0f))
        {
            attack *= 3;
            // critical effect
        }

		/*
        for (int i = 0; i < 2; i++)
        {
            EquipItem weapon = Player.Instance.GetEquipment(EquipItem.Part.Hand, i);
            if (null != weapon && null != weapon.enchantment)
            {
                weapon.enchantment.Enchant(defender);
            }
        }
		*/
        defender.Damage(attack);
    }
	/*
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
	*/
}
