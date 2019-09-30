﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Monster : MonoBehaviour
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

	public Meta						meta;
	public Unit						data;
	private Transform				ui_root;
	private Text					ui_name;
	public UIGaugeBar				ui_health;
	private SpriteRenderer			sprite;
	public Animator					animator;
	private Transform				damage_effect_spot;
	public Effect_MonsterDamage		damage_effect_prefab;
	public Transform				death_effect_prefab;
	private Transform				buff_effect_spot;
	private Transform[]			buff_effects;

	private void Start()
	{
		meta = null;
		data = null;

		ui_root = UIUtil.FindChild<Transform>(transform, "../../UI/Monster");
		ui_name = UIUtil.FindChild<Text>(ui_root, "Name");
		ui_health = UIUtil.FindChild<UIGaugeBar>(ui_root, "Health");

		sprite = GetComponent<SpriteRenderer>();
		if (null == sprite)
		{
			throw new MissingComponentException("SpriteRenderer");
		}
		animator = GetComponent<Animator>();
		if (null == animator)
		{
			throw new MissingComponentException("Animator");
		}

		buff_effect_spot = UIUtil.FindChild<Transform>(transform, "Buff");
		buff_effects = new Transform[(int)Buff.Type.Max];
		buff_effects[(int)Buff.Type.Stun - 1] = UIUtil.FindChild<Transform>(buff_effect_spot, "Effect_MonsterStun");

		damage_effect_spot = UIUtil.FindChild<Transform>(transform, "../../UI/BattleEffect");
		damage_effect_prefab = UIUtil.FindChild<Effect_MonsterDamage>(damage_effect_spot, "Effect_MonsterDamage");
		death_effect_prefab = UIUtil.FindChild<Transform>(damage_effect_spot, "Effect_MonsterDeath/BloodDroplets");
		
		ui_root.gameObject.SetActive(false);

		Util.EventSystem.Subscribe<Buff>(EventID.Buff_Start, OnBuffStart);
		Util.EventSystem.Subscribe<Buff>(EventID.Buff_End, OnBuffEnd);
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Buff>(EventID.Buff_Start, OnBuffStart);
		Util.EventSystem.Unsubscribe<Buff>(EventID.Buff_End, OnBuffEnd);
	}

	public void Init(Meta meta)
	{
		foreach (Transform buff in buff_effects)
		{
			buff.gameObject.SetActive(false);
		}
		this.meta = meta;
		this.data = new Unit()
		{
			max_health = meta.health,
			cur_health = meta.health,
			attack = meta.attack,
			defense = meta.defense,
			speed = meta.speed,
			critical = 0.0f
		};

		gameObject.SetActive(true);
		ui_root.gameObject.SetActive(true);

		ui_name.text = meta.name;
		ui_health.max = data.max_health;
		ui_health.current = data.cur_health;
		sprite.sprite = ResourceManager.Instance.Load<Sprite>(meta.sprite_path);
	}

	public IEnumerator ColorTo(Color from, Color to, float time)
	{
		sprite.color = from;
		return Util.UITween.ColorTo(sprite, to, time);
	}

	public void Die()
	{
		Transform deathEffect = GameObject.Instantiate<Transform>(death_effect_prefab);
		deathEffect.gameObject.SetActive(true);
		Object.Destroy(deathEffect.gameObject, 1.0f);
		ui_root.gameObject.SetActive(false);
	}

	private void OnBuffStart(Buff buff)
	{
		buff_effects[(int)buff.buff_type - 1].gameObject.SetActive(true);
	}

	private void OnBuffEnd(Buff buff)
	{
		buff_effects[(int)buff.buff_type - 1].gameObject.SetActive(false);
	}
}
