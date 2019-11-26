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
		Poison = 5,
		Runaway = 6,
		Stats = 7,
		Max	= 7
	}

	public string buff_name;
	public Type buff_type;
	public Unit target;
	public int duration;
	public bool active;

	public Buff(string buffName, int duration, Type buffType)
	{
		buff_name = buffName;
		buff_type = buffType;
		this.duration = duration;
		active = true;
	}

	public virtual void OnAttach(Unit target)
	{
		this.target = target;
	}
	public virtual void OnDetach() { }

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
	public Buff_DOT(string buffName, int duration, float damage, Type buffType) : base(buffName, duration, buffType)
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
		result.type = buff_name;
		result.damage = damage;
		target.OnDamage(null, result);

		base.OnEffect();
	}
}

public class Buff_Bleeding : Buff_DOT			   
{
    public Buff_Bleeding(int duration, float damage) : base("Bleeding", duration, damage, Type.Bleeding)
    {
	}
}

public class Buff_Stats : Buff
{
	private Stat stats;
	public Buff_Stats(string buffName, int duration, Stat stats, Type buffType) : base(buffName, duration, buffType)
	{
		this.stats = stats;
	}

	public override void OnAttach(Unit target)
	{
		base.OnAttach(target);
		target.stats += stats;
	}

	public override void OnDetach()
	{
		target.stats -= stats;
	}
}

public class Buff_Fear : Buff_Stats
{
	public Buff_Fear(int duration, Stat stats) : base("Fear", duration, stats, Type.Fear)
	{
	}
}

