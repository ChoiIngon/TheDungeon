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
    public int turn;
    public Buff_Blaze(Unit target) : base(target)
    {
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {
        target.OnDamage(0);
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}

public class Buff_Venom : Buff
{
    public int turn;
    public Buff_Venom(Unit target) : base(target)
    {
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {
        target.OnDamage(0);
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}

public class Buff_GrimReaper : Buff
{
    public Buff_GrimReaper(Unit target) : base(target)
    {
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {
        target.OnDamage(target.maxHealth);
        target.buffs.Remove(this);
    }
}

public class Buff_Stun : Buff
{
    public int turn;
    public float speed;
    public Buff_Stun(Unit target) : base(target)
    {
        this.speed = target.speed;
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {

        target.speed = 0.0f;
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}

public class Buff_Chill : Buff
{
    public int turn;
    public float speed;
    public Buff_Chill(Unit target) : base(target)
    {
        this.speed = target.speed;
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {
        target.speed = 0.0f;
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}

public class Buff_Unstable : Buff
{
    public int turn;
    public float speed;
    public Buff_Unstable(Unit target) : base(target)
    {
        this.speed = target.speed;
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {

        target.speed = 0.0f;
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}

public class Buff_Terror : Buff
{
    public int turn;
    public float speed;
    public Buff_Terror(Unit target) : base(target)
    {
        this.speed = target.speed;
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {

        target.speed = 0.0f;
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}

public class Buff_Luck : Buff
{
    public int turn;
    public float speed;
    public Buff_Luck(Unit target) : base(target)
    {
        this.speed = target.speed;
        target.buffs.Add(this);
    }

    public override void OnBuff()
    {

        target.speed = 0.0f;
        if (0 >= turn)
        {
            target.buffs.Remove(this);
        }
    }
}