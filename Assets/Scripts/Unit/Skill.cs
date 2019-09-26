using UnityEngine;
using System.Collections;

public abstract class Skill 
{
	public abstract void OnHit(Unit target);
}

public class Skill_Charm : Skill
{
	public override void OnHit(Unit target)
	{
		Buff_Stun stun = new Buff_Stun(target);
		stun.time = 3.0f;
	}
}