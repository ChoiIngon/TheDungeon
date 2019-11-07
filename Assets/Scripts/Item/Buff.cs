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

	public string buff_id;
	public Type buff_type;
	public string buff_name;
	public Unit target;
	public bool active;

	public Buff(Unit target, Type type)
	{
		this.target = target;
		this.buff_type = type;
		this.active = true;
	}

	public virtual void OnEffect()
	{
	}
}

public class Buff_Blaze : Buff
{
    public int duration;
    public Buff_Blaze(Unit target) : base(target, Type.Invalid)
    {
		buff_name = "Blaze";
		duration = 8;
    }

    public override void OnEffect()
    {
		//duration -= 1;
        //target.Damage(0);
		//if (0 >= duration)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_Bleeding : Buff
{
	public int damage;
    public int turn;
    public Buff_Bleeding(Unit target) : base(target, Type.Bleeding)
    {
		buff_name = "Bleeding";
	}

    public override void OnEffect()
    {
		turn--;
		if (0 >= turn)
		{
			active = false;
		}
	}
}

public class Buff_GrimReaper : Buff
{
    public Buff_GrimReaper(Unit target) : base(target, Type.Invalid)
    {
        //target.buffs.Add(this);
    }

    public override void OnEffect()
    {
		//target.Damage(target.stats.health);
        //target.buffs.Remove(this);
    }
}

public class Buff_Stun : Buff
{
    public int turn;
    public Buff_Stun(Unit target) : base(target, Type.Stun)
    {
		buff_name = "Stun";
	}

    public override void OnEffect()
    {
		turn--;
		if (0 >= turn)
		{
			active = false;
		}
    }
}

public class Buff_Chill : Buff
{
    public int turn;
    public float speed;
    public Buff_Chill(Unit target) : base(target, Type.Invalid)
    {
		//this.speed = target.stats.speed;
        //target.buffs.Add(this);
    }

    public override void OnEffect()
    {
		//target.stats.speed = 0.0f;
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_Unstable : Buff
{
    public int turn;
    public float speed;
    public Buff_Unstable(Unit target) : base(target, Type.Invalid)
    {
		//this.speed = target.stats.speed;
        //target.buffs.Add(this);
    }

    public override void OnEffect()
    {

		//target.stats.speed = 0.0f;
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_Fear : Buff
{
	public int turn;
	public Buff_Fear(Unit target) : base(target, Type.Fear)
	{
		buff_name = "Fear";
	}

	public override void OnEffect()
	{
		turn--;
		if (0 >= turn)
		{
			active = false;
		}
	}
}

public class Buff_Luck : Buff
{
    public int turn;
    public float speed;
    public Buff_Luck(Unit target) : base(target, Type.Invalid)
    {
		//this.speed = target.stats.speed;
        //target.buffs.Add(this);
    }

    public override void OnEffect()
    {
		//target.stats.speed = 0.0f;
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_Blindness : Buff
{
	public int turn;
	public Buff_Blindness(Unit target) : base(target, Type.Blind)
	{
		buff_name = "Blindness";
	}

	public override void OnEffect()
	{
		turn--;
		if (0 >= turn)
		{
			active = false;
		}
	}
}
