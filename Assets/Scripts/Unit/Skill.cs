using UnityEngine;
using System.Collections;

public abstract class Skill 
{
	public string skill_id;
	public virtual void OnAttack(Unit target) { }
	public virtual void OnDefense(Unit target) { }
}

public class Skill_Stun : Skill
{
	public override void OnAttack(Unit target)
	{
		if (30 >= Random.Range(0, 100) && 0 == target.GetBuffCount(Buff.Type.Stun))
		{
			Buff_Stun buff = new Buff_Stun(target);
			buff.turn = 7;
			target.AddBuff(buff);
		}
	}
}