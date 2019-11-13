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

	public Unit owner;
	public float cooltime;

	public virtual void OnAttach(Unit owner)
	{
		this.owner = owner;
	}
	public virtual void OnDetach() {}
	public virtual void OnAttack(Unit target) { }
	public virtual void OnDefense(Unit target) { }
}

public class Skill_Attack : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "Attack";
			skill_id = "SKILL_ATTACK";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "";
		}
		public override Skill CreateInstance()
		{
			Skill_Attack skill = new Skill_Attack();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnAttack(Unit target)
	{
		Unit.AttackResult result = new Unit.AttackResult();
		if (0 < owner.GetBuffCount(Buff.Type.Stun))
		{
			return;
		}

		if (0 < owner.GetBuffCount(Buff.Type.Fear))
		{
			return;
		}

		const float RANDOM_RANGE = 0.1f;
		float attack = owner.attack + Random.Range(-owner.attack * RANDOM_RANGE, owner.attack * RANDOM_RANGE);
		float defense = target.defense + Random.Range(-target.defense * RANDOM_RANGE, target.defense * RANDOM_RANGE);
		result.damage = attack * 100 / (100 + defense);

		if (owner.critical >= Random.Range(0.0f, 100.0f))
		{
			result.damage *= 3;
			result.critical = true;
		}

		target.OnDamage(owner, result);
		owner.on_attack?.Invoke(result);
		target.on_defense?.Invoke(result);
	}
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
			Buff buff = new Buff(target, "Stun", 5, Buff.Type.Stun);
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
			cooltime = 10;
			description = "2턴 동안 적을 공포에 질리게 한다. 공포에 질린적은 무서워한다";
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
			Buff buff = new Buff(target, "Fear", 2, Buff.Type.Fear);
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
			cooltime = 10;
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
		Buff buff = new Buff_DOT(target, "Bleeding", 5, 10, Buff.Type.Bleeding);
		target.AddBuff(buff);
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
			cooltime = 10;
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
		Buff buff = new Buff(target, "Blindness", 2, Buff.Type.Blind);
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