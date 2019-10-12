using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
	public float max_health;
	public float cur_health;

	public float attack;
	public float defense;
	public float speed;
	public float critical;

	public Stat stats;

	public class UnitSkill
	{
		public int ref_count;
		public Skill skill_data;
	}
	private Dictionary<string, UnitSkill> skills;
	public List<Buff>[] buffs = new List<Buff>[(int)Buff.Type.Max];

	public class AttackResult
	{
		public bool critical = false;
		public float damage = 0.0f;
	}

	public Unit()
	{
		Init();
	}


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
	}

	public AttackResult Attack(Unit target)
	{
		AttackResult result = new AttackResult();
		if (0 < GetBuffCount(Buff.Type.Stun))
		{
			result.damage = 0.0f;
			return result;
		}

		if (0 < GetBuffCount(Buff.Type.Fear))
		{
			result.damage = 0.0f;
			return result;
		}

		float attack = this.attack + Random.Range(-this.attack * 0.1f, this.attack * 0.1f);
		float defense = target.defense + Random.Range(-target.defense * 0.1f, target.defense * 0.1f);
		result.damage = Mathf.Min(attack, attack * (attack/defense));

		OnAttack(target);
		target.OnDefense(this);

		if (critical >= Random.Range(0.0f, 100.0f))
		{
			result.damage *= 3;
			result.critical = true;
		}
		return result;
	}

	public virtual void CalculateStat()
	{
		attack = stats.GetStat(StatType.Attack) + (stats.GetStat(StatType.Attack) * stats.GetStat(StatType.Attack_Rate) / 100);
		defense = stats.GetStat(StatType.Defense) + (stats.GetStat(StatType.Defense) * stats.GetStat(StatType.Defense_Rate) / 100);
		speed = stats.GetStat(StatType.Speed) + (stats.GetStat(StatType.Speed) * stats.GetStat(StatType.Speed_Rate) / 100);
		critical = stats.GetStat(StatType.Critical);
		max_health = stats.GetStat(StatType.Health) + (stats.GetStat(StatType.Health) * stats.GetStat(StatType.Health_Rate) / 100);
	}

	public void AddSkill(Skill skill)
	{
		UnitSkill unitSkill = null;
		if (false == skills.ContainsKey(skill.meta.skill_id))
		{
			unitSkill = new UnitSkill();
			unitSkill.skill_data = skill;
			unitSkill.ref_count = 0;
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
			skills.Remove(skill.meta.skill_id);
		}
	}

	public void OnAttack(Unit target)
	{
		foreach (var itr in skills)
		{
			Skill skill = itr.Value.skill_data;
			skill.OnAttack(target);
		}
	}

	public void OnDefense(Unit target)
	{
		foreach (var itr in skills)
		{
			Skill skill = itr.Value.skill_data;
			skill.OnDefense(target);
		}
	}

	public int GetBuffCount(Buff.Type type)
	{
		return buffs[(int)type - 1].Count;
	}

	public void AddBuff(Buff buff)
	{
		Debug.Log("add buff(buff_id:" + buff.buff_id + ")");
		buffs[(int)buff.buff_type - 1].Add(buff);
		Util.EventSystem.Publish<Buff>(EventID.Buff_Start, buff);
	}

	public void RemoveBuff(Buff buff)
	{
		buffs[(int)buff.buff_type - 1].Remove(buff);
		Util.EventSystem.Publish<Buff>(EventID.Buff_End, buff);
		Debug.Log("remove buff(buff_id:" + buff.buff_id + ")");
	}

	public void OnBattleTurn()
	{
		List<Buff> delete = new List<Buff>();
		for (int i = 0; i < buffs.Length; i++)
		{
			List<Buff> buffsByType = buffs[i];
			foreach (Buff buff in buffsByType)
			{
				buff.OnBuff();
				if (false == buff.active)
				{
					delete.Add(buff);
				}
			}
		}
		foreach (Buff buff in delete)
		{
			RemoveBuff(buff);
		}
	}
}
