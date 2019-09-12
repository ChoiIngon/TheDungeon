using UnityEngine;
using System.Collections;

public class Monster : Unit
{
	public class Meta
	{
		public class Reward
		{
			public int coin;
			public int exp;
		}
		public string id;
		public string name;
		public int level;
		public float health;
		public float defense;
		public float attack;
		public float speed;
		public string sprite_path;
		public Reward reward;

		public Meta()
		{
			reward = new Reward();
		}
	}
}
