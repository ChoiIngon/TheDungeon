using System.Collections;
using UnityEngine;

public abstract class Buff {
    public string name;
    public Unit target;
    public Buff(Unit target)
    {
        this.target = target;
    }

    public abstract void OnBuff();
}

public class Buff_Blaze : Buff
{
    public int duration;
    public Buff_Blaze(Unit target) : base(target)
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
    public Buff_Venom(Unit target) : base(target)
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
    public Buff_GrimReaper(Unit target) : base(target)
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
    public float time;
    public Buff_Stun(Unit target) : base(target)
    {
        target.buffs.Add(this);
		Util.EventSystem.Publish<Buff_Stun>(EventID.Buff_Stun_Start, this);
	}

    public override void OnBuff()
    {
		time -= Time.deltaTime;
		if (0.0f >= time)
		{
			target.buffs.Remove(this);
			Util.EventSystem.Publish<Buff_Stun>(EventID.Buff_Stun_Finish, this);
		}
    }
}

public class Buff_Chill : Buff
{
    public int turn;
    public float speed;
    public Buff_Chill(Unit target) : base(target)
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
    public Buff_Unstable(Unit target) : base(target)
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
    public Buff_Terror(Unit target) : base(target)
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
    public Buff_Luck(Unit target) : base(target)
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