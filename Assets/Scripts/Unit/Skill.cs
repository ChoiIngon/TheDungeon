using UnityEngine;
using System.Collections;

public abstract class Skill 
{
	public abstract void OnHit(Unit target);
}

public class Skill_Stun : Skill
{
	public override void OnHit(Unit target)
	{
		if (30 >= Random.Range(0, 100))
		{
			Buff_Stun stun = new Buff_Stun(target);
			stun.time = 3.0f;
		}
	}
}