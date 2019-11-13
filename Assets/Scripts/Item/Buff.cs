using System.Collections;
using UnityEngine;

public class Buff
{
	public enum Type
	{
		Invalid,
		Stun = 1,
		Blind = 2,
		Fear = 3,
		Bleeding = 4,
		Max	= 4
	}

	public string buff_name;
	public Type buff_type;
	public Unit target;
	public int duration;
	public bool active;

	public Buff(Unit unit, string buffName, int duration, Type buffType)
	{
		target = unit;
		buff_name = buffName;
		buff_type = buffType;
		this.duration = duration;
		active = true;
	}

	public virtual void OnEffect()
	{
		duration--;
		if (0 >= duration)
		{
			active = false;
		}
	}
}

public class Buff_DOT : Buff
{
	public float damage;
	public Buff_DOT(Unit target, string buffName, int duration, float damage, Type buffType) : base(target, buffName, duration, buffType)
	{
		this.duration = duration;
		this.damage = damage;
	}

	public override void OnEffect()
	{
		if (false == active)
		{
			return;
		}

		Unit.AttackResult result = new Unit.AttackResult();
		result.damage = damage;
		target.OnDamage(null, result);

		base.OnEffect();
	}
}

public class Buff_Bleeding : Buff_DOT			   
{
    public Buff_Bleeding(Unit target, string buffName, int duration, float damage) : base(target, buffName, duration, damage, Type.Bleeding)
    {
	}
}
