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

public class Skill_Fear : Skill
{
	public new class Meta : Skill.Meta
	{
		public override Skill CreateInstance()
		{
			Skill_Fear skill = new Skill_Fear();
			skill.meta = this;
			return skill;
		}
	}
	public override void OnAttack(Unit target)
	{
		if (100 >= Random.Range(0, 100) && 0 == target.GetBuffCount(Buff.Type.Fear))
		{
			Buff_Fear buff = new Buff_Fear(target);
			buff.turn = 2;
			target.AddBuff(buff);
		}
	}
}

public class Skill_Bleeding : Skill
{
	public new class Meta : Skill.Meta
	{
		public override Skill CreateInstance()
		{
			Skill_Bleeding skill = new Skill_Bleeding();
			skill.meta = this;
			return skill;
		}
	}
	public override void OnAttack(Unit target)
	{
		if (100 >= Random.Range(0, 100))
		{
			Buff_Bleeding buff = new Buff_Bleeding(target);
			buff.turn = 2;
			target.AddBuff(buff);
		}
	}
}

public class Skill_Blindness : Skill
{
	public new class Meta : Skill.Meta
	{
		public override Skill CreateInstance()
		{
			Skill_Blindness skill = new Skill_Blindness();
			skill.meta = this;
			return skill;
		}
	}
	public override void OnAttack(Unit target)
	{
		if (100 >= Random.Range(0, 100))
		{
			Buff_Blindness buff = new Buff_Blindness(target);
			buff.turn = 2;
			target.AddBuff(buff);
		}
	}
}
public class SkillManager : Util.Singleton<SkillManager>
{
	private List<Skill.Meta> skill_metas = new List<Skill.Meta>();

	public void Init()
	{
		skill_metas.Add(new Skill_Stun.Meta() { skill_id = "SKILL_STUN", description = "20%의 확률로 5턴 동안 대상을 기절 시킴" });
		skill_metas.Add(new Skill_Fear.Meta() { skill_id = "SKILL_FEAR", description = "20% 확율로 적을 공포에 질리게 한다. 공포에 질린적은 도망간다" });
		skill_metas.Add(new Skill_Bleeding.Meta() { skill_id = "SKILL_BLEEDING", description = "20% 확율로 적에게 출혈 상처를 입힌다" });
		skill_metas.Add(new Skill_Fear.Meta() { skill_id = "SKILL_BLEEDING", description = "20% 확율로 적에게 출혈 상처를 입힌다" });
		skill_metas.Add(new Skill_Blindness.Meta() { skill_id = "SKILL_BLINDNESS", description = "높은 확율로 공격이 빗나감" });
	}

	public Skill CreateRandomInstance()
	{
		if (0 == skill_metas.Count)
		{
			return null;
		}

		return skill_metas[Random.Range(0, skill_metas.Count)].CreateInstance();
	}
}