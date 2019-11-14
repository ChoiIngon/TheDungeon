using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
	public class UnitSkill
	{
		public int ref_count;
		public Skill skill_data;
	}
	public Dictionary<string, UnitSkill> skills;
	public List<Buff>[] buffs = new List<Buff>[(int)Buff.Type.Max];

	public class AttackResult
	{
		public string type = "";
		public bool critical = false;
		public float damage = 0.0f;
	}

	public float max_health;
	public float cur_health;

	public float attack;
	public float defense;
	public float speed;
	public float critical;

	public Stat stats;

	public System.Action<AttackResult> on_attack;
	public System.Action<AttackResult> on_defense;
	public Skill current_skill;

	public virtual void Init()
	{
		max_health = 0.0f;
		cur_health = 0.0f;

		attack = 0.0f;
		defense = 0.0f;
		speed = 0.0f;
		critical = 0.0f;

		stats = new Stat();
		skills = new Dictionary<string, UnitSkill>();
		for (int i = 0; i < buffs.Length; i++)
		{
			buffs[i] = new List<Buff>();
		}

		Skill_Attack.Meta defaultSkillMeta = new Skill_Attack.Meta();
		current_skill = defaultSkillMeta.CreateInstance();
		AddSkill(current_skill);
	}

	public void Attack(Unit target)
	{
		current_skill.OnAttack(target);
		current_skill.cooltime = current_skill.meta.cooltime;
	}

	public void OnDamage(Unit attacker, AttackResult attackResult)
	{
		foreach (var itr in skills)
		{
			Skill skill = itr.Value.skill_data;
			skill.OnDefense(attacker);
		}
		cur_health = Mathf.Max(cur_health - attackResult.damage, 0.0f);
		on_defense?.Invoke(attackResult);
	}

	public virtual void CalculateStat()
	{
		attack = stats.GetStat(StatType.Attack) + (stats.GetStat(StatType.Attack) * stats.GetStat(StatType.Attack_Rate) / 100);
		defense = stats.GetStat(StatType.Defense) + (stats.GetStat(StatType.Defense) * stats.GetStat(StatType.Defense_Rate) / 100);
		speed = stats.GetStat(StatType.Speed) + (stats.GetStat(StatType.Speed) * stats.GetStat(StatType.Speed_Rate) / 100);
		critical = stats.GetStat(StatType.Critical);
		max_health = stats.GetStat(StatType.Health) + (stats.GetStat(StatType.Health) * stats.GetStat(StatType.Health_Rate) / 100);

		attack = Stat.Truncate(attack, 0.01f);
		defense = Stat.Truncate(defense, 0.01f);
		speed = Stat.Truncate(speed, 0.01f);
		critical = Stat.Truncate(critical, 0.01f);
		max_health = Mathf.Round(max_health);
	}

	public void AddSkill(Skill skill)
	{
		UnitSkill unitSkill = null;
		if (false == skills.ContainsKey(skill.meta.skill_id))
		{
			unitSkill = new UnitSkill();
			unitSkill.skill_data = skill;
			unitSkill.ref_count = 0;
			skill.OnAttach(this);
			skills.Add(skill.meta.skill_id, unitSkill);
		}
		else
		{
			unitSkill = skills[skill.meta.skill_id];
		}

		unitSkill.ref_count += 1;

		Debug.Log("add skill(skill_id:" + skill.meta.skill_id + ", ref_count:" + unitSkill.ref_count + ")");
	}

	public void RemoveSkill(Skill skill)
	{
		if (false == skills.ContainsKey(skill.meta.skill_id))
		{
			Debug.LogError("can not find unit skill(skill_id:" + skill.meta.skill_id + ")");
			return;
		}
		UnitSkill unitSkill = skills[skill.meta.skill_id];
		Debug.Log("add skill(skill_id:" + skill.meta.skill_id + ", ref_count:" + unitSkill.ref_count + ")");
		unitSkill.ref_count -= 1;

		if (0 >= unitSkill.ref_count)
		{
			Debug.Log("remove skill");
			skill.OnDetach();
			skills.Remove(skill.meta.skill_id);
		}
	}
		
	public int GetBuffCount(Buff.Type type)
	{
		return buffs[(int)type - 1].Count;
	}

	public void AddBuff(Buff buff)
	{
		Debug.Log("add buff(buff_id:" + buff.buff_name + ")");
		buff.target = this;
		buffs[(int)buff.buff_type - 1].Add(buff);
		Util.EventSystem.Publish<Buff>(EventID.Buff_Start, buff);
	}

	public void RemoveBuff(Buff buff)
	{
		buffs[(int)buff.buff_type - 1].Remove(buff);
		Util.EventSystem.Publish<Buff>(EventID.Buff_End, buff);
		Debug.Log("remove buff(buff_id:" + buff.buff_name + ")");
	}

	public void OnBattleTurn()
	{
		for (int i = 0; i < buffs.Length; i++)
		{
			List<Buff> buffsByType = new List<Buff>(buffs[i]);
			foreach (Buff buff in buffsByType)
			{
				buff.OnEffect();
				Util.EventSystem.Publish<Buff>(EventID.Buff_Effect, buff);
				if (false == buff.active)
				{
					RemoveBuff(buff); 
				}
			}
		}
	}
}
