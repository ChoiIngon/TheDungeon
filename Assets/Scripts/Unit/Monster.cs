using UnityEngine;
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
			public float item_chance;
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

	public Meta meta;
	public Unit data;
	private Transform ui_root;
	private Text ui_name;
	public UIGaugeBar ui_health;

	public Animator animator;
	private Transform damage_effect_spot;
	public Effect_MonsterDamage[] damage_effects;
	public int damage_effect_index;
	public Transform death_effect_prefab;
	private Transform buff_effect_spot;
	private Transform[] buff_effects;

	private SpriteRenderer sprite;
	private Shader shaderOriginal;
	private Shader shaderWhite;

	private void Awake()
	{
		meta = null;
		data = null;
		ui_root = UIUtil.FindChild<Transform>(transform, "../../UI/Battle");
		ui_name = UIUtil.FindChild<Text>(ui_root, "MonsterName");
		ui_health = UIUtil.FindChild<UIGaugeBar>(ui_root, "MonsterHealth");
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
		buff_effects[(int)Buff.Type.Blind - 1] = UIUtil.FindChild<Transform>(buff_effect_spot, "Effect_MonsterBlindness");
		buff_effects[(int)Buff.Type.Fear - 1] = UIUtil.FindChild<Transform>(buff_effect_spot, "Effect_MonsterFear");
		buff_effects[(int)Buff.Type.Bleeding - 1] = UIUtil.FindChild<Transform>(buff_effect_spot, "Effect_MonsterBleeding");

		damage_effect_spot = UIUtil.FindChild<Transform>(transform, "../../UI/BattleEffect");
		damage_effects = new Effect_MonsterDamage[2];
		for (int i = 0; i < damage_effects.Length; i++)
		{
			damage_effects[i] = GameObject.Instantiate<Effect_MonsterDamage>(UIUtil.FindChild<Effect_MonsterDamage>(damage_effect_spot, "Effect_MonsterDamage"));
			damage_effects[i].transform.SetParent(damage_effect_spot);
		}
		death_effect_prefab = UIUtil.FindChild<Transform>(damage_effect_spot, "Effect_MonsterDeath/BloodDroplets");
	}

	private void Start()
	{
		shaderOriginal = sprite.material.shader;
		shaderWhite = Shader.Find("GUI/Text Shader");
		gameObject.SetActive(false);
		Util.EventSystem.Subscribe<Buff>(EventID.Buff_Start, OnBuffStart);
		Util.EventSystem.Subscribe<Buff>(EventID.Buff_End, OnBuffEnd);
	}

	private void OnEnable()
	{
		ui_root?.gameObject.SetActive(true);
	}

	private void OnDisable()
	{
		ui_root?.gameObject.SetActive(false);
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
		this.data = new Unit();
		this.data.Init();
		this.data.max_health = meta.health;
		this.data.cur_health = meta.health;
		this.data.attack = meta.attack;
		this.data.defense = meta.defense;
		this.data.speed = meta.speed;
		this.data.critical = 0.0f;

		gameObject.SetActive(true);

		ui_name.text = meta.name;
		ui_health.max = data.max_health;
		ui_health.current = data.cur_health;
		sprite.sprite = ResourceManager.Instance.Load<Sprite>(meta.sprite_path);
		sprite.material.shader = shaderOriginal;

		damage_effect_index = 0;
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

	public void Runaway()
	{
	}

	public IEnumerator OnDamage(Unit.AttackResult attackResult)
	{
		iTween.ShakePosition(gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
		Effect_MonsterDamage effect = damage_effects[damage_effect_index++];
		damage_effect_index = damage_effect_index % damage_effects.Length;
		effect.gameObject.SetActive(false);
		effect.damage = (int)attackResult.damage;
		effect.critical = attackResult.critical;
		effect.gameObject.SetActive(true);
		data.cur_health -= (int)attackResult.damage;
		ui_health.current = data.cur_health;
		if (true == attackResult.critical)
		{
			sprite.material.shader = shaderWhite;
			yield return new WaitForSeconds(0.03f);
			sprite.material.shader = shaderOriginal;
		}
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
