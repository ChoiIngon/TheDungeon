using UnityEngine;
using System.Collections.Generic;

public abstract class Skill 
{
	public abstract class Meta
	{
		public string skill_id;
		public string skill_name;
		public string description;
		public string sprite_path;
		public int cooltime;
		public abstract Skill CreateInstance();
	}

	public Meta meta;
	public float cooltime;
	public virtual void OnAttack(Unit target) { }
	public virtual void OnDefense(Unit target) { }
}

public class Skill_Stun : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "Stun";
			skill_id = "SKILL_STUN";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 10;
			description = "5턴 동안 대상을 기절 시킴";
		}
		public override Skill CreateInstance()
		{
			Skill_Stun skill =  new Skill_Stun();
			skill.meta = this;
			return skill;
		}
	}
	public override void OnAttack(Unit target)
	{
		if (0 == target.GetBuffCount(Buff.Type.Stun))
		{
			Buff_Stun buff = new Buff_Stun(target);
			buff.turn = 5;
			target.AddBuff(buff);
		}
	}
}

public class Skill_Fear : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "Fear";
			skill_id = "SKILL_FEAR";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 3;
			description = "20% 확율로 적을 공포에 질리게 한다. 공포에 질린적은 도망간다";
		}
		public override Skill CreateInstance()
		{
			Skill_Fear skill = new Skill_Fear();
			skill.meta = this;
			return skill;
		}
	}
	public override void OnAttack(Unit target)
	{
		if (0 == target.GetBuffCount(Buff.Type.Fear))
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
		public Meta()
		{
			skill_name = "Bledding";
			skill_id = "SKILL_BLEEDING";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 2;
			description = "20% 확율로 적에게 출혈 상처를 입힌다";
		}
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
		public Meta()
		{
			skill_name = "Blind";
			skill_id = "SKILL_BLINDNESS";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 4;
			description = "높은 확율로 공격이 빗나감";
		}
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
		skill_metas.Add(new Skill_Stun.Meta());
		skill_metas.Add(new Skill_Fear.Meta());
		skill_metas.Add(new Skill_Bleeding.Meta());
		skill_metas.Add(new Skill_Blindness.Meta());
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