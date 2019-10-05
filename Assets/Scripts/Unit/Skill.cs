using UnityEngine;
using System.Collections.Generic;

public abstract class Skill 
{
	public abstract class Meta
	{
		public string skill_id;
		public string description;
		public abstract Skill CreateInstance();
	}

	public Meta meta;
	public virtual void OnAttack(Unit target) { }
	public virtual void OnDefense(Unit target) { }
}

public class Skill_Stun : Skill
{
	public new class Meta : Skill.Meta
	{
		public override Skill CreateInstance()
		{
			Skill_Stun skill =  new Skill_Stun();
			skill.meta = this;
			return skill;
		}
	}
	public override void OnAttack(Unit target)
	{
		if (20 >= Random.Range(0, 100) && 0 == target.GetBuffCount(Buff.Type.Stun))
		{
			Buff_Stun buff = new Buff_Stun(target);
			buff.turn = 2;
			target.AddBuff(buff);
		}
	}
}

