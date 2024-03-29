﻿using UnityEngine;
using System.Collections.Generic;

public abstract class Skill 
{
	public enum TriggerType
	{
		Invalid,
		Active,
		Passive
	}
	public abstract class Meta
	{
		public TriggerType trigger_type;
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
	public virtual void OnDefense(Unit attacker, Unit.AttackResult attack) { }
}

public class Skill_Attack : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Attack";
			skill_id = "Skill_Attack";
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

		const float RANDOM_RANGE = 0.1f;
		float attack = owner.attack + Random.Range(-owner.attack * RANDOM_RANGE, owner.attack * RANDOM_RANGE);
		float defense = target.defense + Random.Range(-target.defense * RANDOM_RANGE, target.defense * RANDOM_RANGE);
		result.damage = attack * 100 / (100 + defense);
		result.damage = (int)result.damage;

		if (owner.critical >= Random.Range(0.0f, 100.0f))
		{
			result.damage *= owner.stats.GetStat(StatType.Critical_Damage) / 100;
			result.critical = true;
		}

		result.damage += result.damage * owner.stats.GetStat(StatType.Damage_Rate) / 100;
		owner.on_attack?.Invoke(result);
		target.OnDamage(owner, result);
	}
}

public class Skill_DoubleAttack : Skill_Attack
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Double Attack";
			skill_id = "Skill_DoubleAttack";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 20;
			description = "연속 2회 공격";
		}
		public override Skill CreateInstance()
		{
			Skill_DoubleAttack skill = new Skill_DoubleAttack();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnAttack(Unit target)
	{
		base.OnAttack(target);
		base.OnAttack(target);
	}
}

public class Skill_Penetrate : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Penetrate";
			skill_id = "Skill_Penetrate";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 25;
			description = "방어력을 무시한 관통 공격";
		}
		public override Skill CreateInstance()
		{
			Skill_Penetrate skill = new Skill_Penetrate();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnAttack(Unit target)
	{
		Unit.AttackResult result = new Unit.AttackResult();
		const float RANDOM_RANGE = 0.1f;
		float attack = owner.attack + Random.Range(-owner.attack * RANDOM_RANGE, owner.attack * RANDOM_RANGE);
		result.damage = attack;
		if (owner.critical >= Random.Range(0.0f, 100.0f))
		{
			result.damage *= owner.stats.GetStat(StatType.Critical_Damage) / 100;
			result.critical = true;
		}

		result.damage += result.damage * owner.stats.GetStat(StatType.Damage_Rate) / 100;
		result.damage = (int)result.damage;
		owner.on_attack?.Invoke(result);
		target.OnDamage(owner, result);
	}
}

public class Skill_Smash : Skill_Attack
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Smash";
			skill_id = "Skill_Smash";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 20;
			description = "강력한 공격으로 50%의 확율로 8턴 동안 적을 기절 시킨다";
		}
		public override Skill CreateInstance()
		{
			Skill_Smash skill = new Skill_Smash();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnAttack(Unit target)
	{
		base.OnAttack(target);
		if (50.0f >= Random.Range(0.0f, 100.0f) && 0 == target.GetBuffCount(Buff.Type.Stun))
		{
			Buff buff = new Buff("Stun", 8, Buff.Type.Stun);
			target.AddBuff(buff);
		}
	}
}

public class Skill_Stun : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Stun";
			skill_id = "Skill_Stun";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 25;
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
			Buff buff = new Buff("Stun", 5, Buff.Type.Stun);
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
			trigger_type = TriggerType.Active;
			skill_name = "Fear";
			skill_id = "Skill_Fear";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 20;
			description = "7턴 동안 적을 공포에 질리게 한다. 공포에 질린적은 공격력과 속도가 감소한다";
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
			Stat stats = new Stat();
			stats.SetStat(new Stat.Data() { type = StatType.Attack_Rate, value = -0.5f });
			stats.SetStat(new Stat.Data() { type = StatType.Speed_Rate, value = -0.5f });
			Buff buff = new Buff_Fear(7, stats);
			target.AddBuff(buff);
		}
	}
}

public class Skill_Bleeding : Skill_Attack
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;	
			skill_name = "Bleeding";
			skill_id = "Skill_Bleeding";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 30;
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
		base.OnAttack(target);
		const float RANDOM_RANGE = 0.1f;
		float attack = owner.attack + Random.Range(-owner.attack * RANDOM_RANGE, owner.attack * RANDOM_RANGE);
		attack *= 0.1f;
		attack = (int)attack;
		Buff buff = new Buff_Bleeding(5, attack);
		target.AddBuff(buff);
	}
}

public class Skill_Poison : Skill_Attack
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Poison";
			skill_id = "Skill_Poison";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 30;
			description = "적을 중독 시켜 지속적인 데미지를 입힌다.";
		}
		public override Skill CreateInstance()
		{
			Skill_Poison skill = new Skill_Poison();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnAttack(Unit target)
	{
		base.OnAttack(target);
		const float RANDOM_RANGE = 0.1f;
		float attack = owner.attack + Random.Range(-owner.attack * RANDOM_RANGE, owner.attack * RANDOM_RANGE);
		attack *= 0.1f;
		attack = (int)attack;
		Buff buff = new Buff_DOT(meta.skill_name, 5, attack, Buff.Type.Poison);
		target.AddBuff(buff);
	}
}

public class Skill_Blindness : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Blind";
			skill_id = "Skill_Blindness";
			sprite_path = "Item/item_equip_sword_004";
			cooltime = 15;
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
		Buff buff = new Buff("Blindness", 2, Buff.Type.Blind);
	}
}

public class Skill_Runaway : Skill
{
	public const string SKILL_ID = "Skill_Runaway";
	public new class Meta : Skill.Meta
	{
		public int max_count;
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Runaway";
			skill_id = SKILL_ID;
			sprite_path = "Skill/skill_icon_run";
			cooltime = 2;
			max_count = 3;
			description = "적으로 부터 도망 친다";
		}
		public override Skill CreateInstance()
		{
			Skill_Runaway skill = new Skill_Runaway();
			skill.meta = this;
			return skill;
		}
	}

	public int remain_count = 0;
	public override void OnAttach(Unit owner)
	{
		remain_count = (meta as Meta).max_count;
		base.OnAttach(owner);
	}

	public override void OnAttack(Unit target)
	{
		if (0 >= remain_count)
		{
			return;
		}
		remain_count -= 1;
		float successChance = (owner.speed / target.speed) * 0.25f;
		if (successChance > Random.Range(0.0f, 1.0f))
		{
			Buff buff = new Buff("Runaway", 1, Buff.Type.Runaway);
			owner.AddBuff(buff);
		}
	}
}

public class Skill_DamageRefection : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Passive;
			skill_name = "Refection";
			skill_id = "Skill_DamageRefection";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "10%의 확률로 받은 데미지의 절반을 반사한다";
		}
		public override Skill CreateInstance()
		{
			Skill_DamageRefection skill = new Skill_DamageRefection();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnDefense(Unit attacker, Unit.AttackResult attack)
	{
		if (10.0f > Random.Range(0.0f, 100.0f))
		{
			Unit.AttackResult clone = new Unit.AttackResult();
			clone.type = meta.skill_name;
			clone.damage = attack.damage * 0.5f;
			clone.critical = attack.critical;
			owner.on_attack?.Invoke(clone);
			attacker.OnDamage(owner, clone);
		}
	}
}

public class Skill_Rage : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Passive;
			skill_name = "Rage";
			skill_id = "Skill_Rage";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "공격을 받을때 일정 확률로 크리티컬 확률 증가";
		}
		public override Skill CreateInstance()
		{
			Skill_Rage skill = new Skill_Rage();
			skill.meta = this;
			return skill;
		}
	}

	float critical_chance = 0.0f;
	public override void OnAttach(Unit owner)
	{
		critical_chance = 0.0f;
		base.OnAttach(owner);
	}
	public override void OnDefense(Unit attacker, Unit.AttackResult attack)
	{
		if (30.0f > Random.Range(0.0f, 100.0f))
		{
			owner.stats.SubtractStat(new Stat.Data() { type = StatType.Critical_Chance, value = critical_chance });
			critical_chance += 1.0f;
			owner.stats.AddStat(new Stat.Data() { type = StatType.Critical_Chance, value = critical_chance });
		}
	}
}

public class Skill_CriticalAttack : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			trigger_type = TriggerType.Active;
			skill_name = "Critical Attack";
			skill_id = "Skill_CriticalAttack";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 24;
			description = "크리티컬 확률 100%";
		}
		public override Skill CreateInstance()
		{
			Skill_CriticalAttack skill = new Skill_CriticalAttack();
			skill.meta = this;
			return skill;
		}
	}

	public override void OnAttach(Unit owner)
	{
		base.OnAttach(owner);
	}
	public override void OnAttack(Unit target)
	{
		Unit.AttackResult result = new Unit.AttackResult();

		const float RANDOM_RANGE = 0.1f;
		float attack = owner.attack + Random.Range(-owner.attack * RANDOM_RANGE, owner.attack * RANDOM_RANGE);
		float defense = target.defense + Random.Range(-target.defense * RANDOM_RANGE, target.defense * RANDOM_RANGE);
		result.damage = attack * 100 / (100 + defense);
		result.damage = (int)result.damage;
		result.damage *= owner.stats.GetStat(StatType.Critical_Damage) / 100;
		result.critical = true;

		result.damage += result.damage * owner.stats.GetStat(StatType.Damage_Rate) / 100;
		owner.on_attack?.Invoke(result);
		target.OnDamage(owner, result);
	}
}

public class Skill_CounterAttack : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "Critical Attack";
			skill_id = "Skill_CounterAttack";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "공격을 받을때 일정 확률로 크리티컬 확률 증가";
		}
		public override Skill CreateInstance()
		{
			Skill_CounterAttack skill = new Skill_CounterAttack();
			skill.meta = this;
			return skill;
		}
	}

	float critical_chance = 0.0f;
	public override void OnAttach(Unit owner)
	{
		critical_chance = 0.0f;
		base.OnAttach(owner);
	}
	public override void OnDefense(Unit attacker, Unit.AttackResult attack)
	{
		critical_chance += owner.stats.GetStat(StatType.Critical_Chance);
	}
}

public class Skill_Parry : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "Critical Attack";
			skill_id = "Skill_Parry";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "공격을 받을때 일정 확률로 크리티컬 확률 증가";
		}
		public override Skill CreateInstance()
		{
			Skill_Parry skill = new Skill_Parry();
			skill.meta = this;
			return skill;
		}
	}

	float critical_chance = 0.0f;
	public override void OnAttach(Unit owner)
	{
		critical_chance = 0.0f;
		base.OnAttach(owner);
	}
	public override void OnDefense(Unit attacker, Unit.AttackResult attack)
	{
		critical_chance += owner.stats.GetStat(StatType.Critical_Chance);
	}
}

public class Skill_PartsCrush : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "PartsCrush";
			skill_id = "Skill_PartsCrush";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "공격을 받을때 일정 확률로 크리티컬 확률 증가";
		}
		public override Skill CreateInstance()
		{
			Skill_PartsCrush skill = new Skill_PartsCrush();
			skill.meta = this;
			return skill;
		}
	}

	float critical_chance = 0.0f;
	public override void OnAttach(Unit owner)
	{
		critical_chance = 0.0f;
		base.OnAttach(owner);
	}
	public override void OnDefense(Unit attacker, Unit.AttackResult attack)
	{
		critical_chance += owner.stats.GetStat(StatType.Critical_Chance);
	}
}

public class Skill_Fury : Skill
{
	public new class Meta : Skill.Meta
	{
		public Meta()
		{
			skill_name = "Fury";
			skill_id = "Skill_Fury";
			sprite_path = "Skill/skill_icon_stun";
			cooltime = 0;
			description = "공격을 받을때 일정 확률로 크리티컬 확률 증가";
		}
		public override Skill CreateInstance()
		{
			Skill_Fury skill = new Skill_Fury();
			skill.meta = this;
			return skill;
		}
	}

	float critical_chance = 0.0f;
	public override void OnAttach(Unit owner)
	{
		critical_chance = 0.0f;
		base.OnAttach(owner);
	}
	public override void OnDefense(Unit attacker, Unit.AttackResult attack)
	{
		critical_chance += owner.stats.GetStat(StatType.Critical_Chance);
	}
}

public class SkillManager : Util.Singleton<SkillManager>
{
	private Dictionary<string, Skill.Meta> metas = new Dictionary<string, Skill.Meta>();
	public void Init()
	{
		RegisterSkill<Skill_Runaway.Meta>();
		RegisterSkill<Skill_Attack.Meta>();
		RegisterSkill<Skill_DoubleAttack.Meta>();
		RegisterSkill<Skill_Bleeding.Meta>();
		RegisterSkill<Skill_Blindness.Meta>();
		RegisterSkill<Skill_Smash.Meta>();
		RegisterSkill<Skill_Stun.Meta>();
		RegisterSkill<Skill_Penetrate.Meta>();
		RegisterSkill<Skill_DamageRefection.Meta>();
		RegisterSkill<Skill_CriticalAttack.Meta>();
		RegisterSkill<Skill_Poison.Meta>();
		RegisterSkill<Skill_CounterAttack.Meta>();
		RegisterSkill<Skill_Parry.Meta>();
		RegisterSkill<Skill_PartsCrush.Meta>();
		RegisterSkill<Skill_Fury.Meta>();
	}

	public Skill.Meta FindMeta<T>(string id) where T : Skill.Meta
	{
	  if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can't find skill meta(id:" + id + ")");
		}
		return metas[id] as T;
	}

	private Skill.Meta RegisterSkill<T>() where T : Skill.Meta, new()
	{
		Skill.Meta meta = new T();
		metas[meta.skill_id] = meta;
		return meta;		
	}
}