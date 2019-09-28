using System.Collections;
using UnityEngine;

public class Buff
{
	public enum Type
	{
		Invalid,
		Stun = 1,
		Max	= 1
	}

	public string buff_id;
	public Type buff_type;
	public Unit target;
	public bool active;

	public Buff(Unit target, Type type)
	{
		this.target = target;
		this.buff_type = type;
		this.active = true;
	}

	public virtual void OnBuff()
	{
	}
}

public class Buff_Blaze : Buff
{
    public int duration;
    public Buff_Blaze(Unit target) : base(target, Type.Invalid)
    {
		duration = 8;
    }

    public override void OnBuff()
    {
		//duration -= 1;
        //target.Damage(0);
		//if (0 >= duration)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_Venom : Buff
{
    public int turn;
    public Buff_Venom(Unit target) : base(target, Type.Invalid)
    {
        //target.buffs.Add(this);
    }

    public override void OnBuff()
    {
        //target.Damage(0);
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_GrimReaper : Buff
{
    public Buff_GrimReaper(Unit target) : base(target, Type.Invalid)
    {
        //target.buffs.Add(this);
    }

    public override void OnBuff()
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
		Util.EventSystem.Publish<Buff_Stun>(EventID.Buff_Start, this);
	}

    public override void OnBuff()
    {
		turn--;
		Debug.Log("on buff(remain_turn:" + turn + ")");
		if (0 >= turn)
		{
			active = false;
			Util.EventSystem.Publish<Buff_Stun>(EventID.Buff_End, this);
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

    public override void OnBuff()
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

    public override void OnBuff()
    {

		//target.stats.speed = 0.0f;
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}

public class Buff_Terror : Buff
{
    public int turn;
    public float speed;
    public Buff_Terror(Unit target) : base(target, Type.Invalid)
    {
		//this.speed = target.stats.speed;
        //target.buffs.Add(this);
    }

    public override void OnBuff()
    {
		//target.stats.speed = 0.0f;
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
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

    public override void OnBuff()
    {
		//target.stats.speed = 0.0f;
        //if (0 >= turn)
        //{
        //    target.buffs.Remove(this);
        //}
    }
}