using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	[System.Serializable]
	public struct Stat
	{
		public int curHealth;
		public int maxHealth;
		public float attack;
		public float defense;
		public float speed;
		public float critcal;
		public float coinBonus;
		public float expBonus;
		static public Stat operator + (Stat rhs, Stat lhs)
		{
			rhs.curHealth += lhs.curHealth;
			rhs.maxHealth += lhs.maxHealth;
			rhs.attack += lhs.attack;
			rhs.defense += lhs.defense;
			rhs.speed += lhs.speed;
			rhs.critcal += lhs.critcal;
			rhs.coinBonus += lhs.coinBonus;
			rhs.expBonus += lhs.expBonus;
			return rhs;
		}
	}

	public int level;
    public List<Buff> buffs;
	public Stat stats;
    // Use this for initialization
    public virtual void Init () {
		stats = new Stat ();
        buffs = new List<Buff>();
	}
	public virtual void Attack(Unit defender)
	{
		defender.Damage (0);
	}
	public virtual void Damage (int damage)
	{
	}
	public virtual void Health(int health)
	{
	}
}
