﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneDungeon : SceneMain
{
	public static UILog log;
	
	private int dungeon_level;
	
	public UIMiniMap mini_map;
	public Dungeon dungeon;	
	public DungeonBattle battle;

	public Button inventory_button;
	public Text text_inventory;
	private Transform ui_player_transform;
	public UIGaugeBar player_health;
	public UIGaugeBar player_exp;
	
	public UIShop shop;
	public UIInventory ui_inventory;

	private UICoin ui_coin;
	private Vector3 ui_coin_position;
	private Transform coin_spot;
    private Coin coin_prefab;
	private DungeonMove dungeon_move;
	
	public override IEnumerator Run()
	{
		if ("Dungeon" == SceneManager.GetActiveScene().name)
		{
			AsyncOperation operation = SceneManager.LoadSceneAsync("Common", LoadSceneMode.Additive);
			while (false == operation.isDone)
			{
				// loading progress
				yield return null;
			}
			yield return StartCoroutine(GameManager.Instance.Init());
		}

		log = UIUtil.FindChild<UILog>(transform, "UI/UILog");
		log.gameObject.SetActive(true);

		dungeon = UIUtil.FindChild<Dungeon>(transform, "Dungeon");
		dungeon.gameObject.SetActive(true);
		dungeon_move = UIUtil.FindChild<DungeonMove>(transform, "Dungeon");

		battle = UIUtil.FindChild<DungeonBattle>(transform, "Battle");
		battle.gameObject.SetActive(false);
		
		ui_inventory = UIUtil.FindChild<UIInventory>(transform, "UI/UIInventory");
		ui_inventory.gameObject.SetActive(false);
		ui_inventory.Init();

		inventory_button = UIUtil.FindChild<Button>(transform, "UI/Dungeon/SideButtons/InventoryButton");
		inventory_button.onClick.AddListener(() =>
		{
			ui_inventory.gameObject.SetActive(true);
			Util.EventSystem.Publish(EventID.Dungeon_Monster_Reveal);
		});
		
		text_inventory = UIUtil.FindChild<Text>(transform, "UI/Dungeon/SideButtons/InventoryButton/Text");
		
		mini_map = UIUtil.FindChild<UIMiniMap>(transform,		"UI/Dungeon/MiniMap");
		mini_map.gameObject.SetActive(true);
		
		ui_player_transform = UIUtil.FindChild<Transform>(transform, "UI/Player");
		ui_player_transform.gameObject.SetActive(true);
		player_health = UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Health");
		player_exp =	UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Exp");

		ui_coin = UIUtil.FindChild<UICoin>(transform, "UI/Dungeon/UICoin");
		ui_coin.gameObject.SetActive(true);
		{
			Image image = UIUtil.FindChild<Image>(ui_coin.transform, "Image");
			ui_coin_position = Camera.main.ScreenToWorldPoint(
				new Vector3(image.rectTransform.position.x, image.rectTransform.position.y, Camera.main.transform.position.z * -1.0f)
			);
		}


		coin_spot = UIUtil.FindChild<Transform>(transform, "CoinSpot");
		coin_spot.gameObject.SetActive(true);
		coin_prefab = UIUtil.FindChild<Coin>(transform, "Prefabs/Coin");

		shop = UIUtil.FindChild<UIShop>(transform, "UI/UIShop");

		Util.EventSystem.Subscribe<int>(EventID.Dungeon_Move, OnMove);
		Util.EventSystem.Subscribe(EventID.Dungeon_Exit_Unlock, () => { StartCoroutine(OnExitUnlock()); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Map_Reveal, () => { mini_map.RevealMap(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Monster_Reveal, () => { mini_map.RevealMonster(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Treasure_Reveal, () => { mini_map.RevealTreasure(); });
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Subscribe(EventID.Player_Stat_Change, OnPlayerStatChange);
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Start, OnQuestStart);
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Complete, OnQuestComplete);
		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);

		InitScene();
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<int>(EventID.Dungeon_Move, OnMove);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Exit_Unlock);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Map_Reveal);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Monster_Reveal);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Treasure_Reveal);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Unsubscribe(EventID.Player_Stat_Change, OnPlayerStatChange);
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Start, OnQuestStart);
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Complete, OnQuestComplete);
	}

	private void InitScene()
	{
		GameManager.Instance.player.Init();

		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
		player_exp.max = GameManager.Instance.player.meta.GetMaxExp(GameManager.Instance.player.level);
		player_exp.current = GameManager.Instance.player.exp;
		text_inventory.text = GameManager.Instance.player.inventory.count.ToString() + "/" + Inventory.MAX_SLOT_COUNT;
		
		InitDungeon();

		Quest quest = QuestManager.Instance.GetAvailableQuest();
		quest.Start();
	}

	private void InitDungeon()
	{
		StartCoroutine(GameManager.Instance.CameraFade(Color.black, new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.5f));
		
		dungeon.Init(++dungeon_level);
		mini_map.Init(dungeon);

		dungeon.InitRooms();
		mini_map.CurrentPosition(dungeon.data.current_room.id);

		ProgressManager.Instance.Update(ProgressType.DungeonLevel, "", dungeon.level);
		StartCoroutine(GameManager.Instance.ui_textbox.Write(GameText.GetText("DUNGEON/WELCOME", dungeon.level), false));
	}

	private IEnumerator Move(int direction)
	{
		mini_map.CurrentPosition(dungeon.data.current_room.id);
		
		yield return OnExitUnlock();
		yield return OnMonser();
		if (null != dungeon.data.current_room.monster)
		{
			if (DungeonBattle.BattleResult.Win == battle.battle_result)
			{
				yield return Win(dungeon.data.current_room.monster);
			}
			else if (DungeonBattle.BattleResult.Lose == battle.battle_result)
			{
				yield return Lose();
			}
			else if (DungeonBattle.BattleResult.Runaway == battle.battle_result)
			{
				int runawayDirection = (direction + 2) % 4;
				if (0 <= runawayDirection || Dungeon.WIDTH * Dungeon.HEIGHT > runawayDirection)
				{
					yield return dungeon_move.Move(runawayDirection);
				}
			}
		}
		OnTreasureBox();
		yield return OnShop();		
		
		if ("" != dungeon.data.current_room.npc_sprite_path)
		{
			ProgressManager.Instance.Update(ProgressType.MeetNpc, dungeon.data.current_room.npc_sprite_path, 1);
		}

		mini_map.CurrentPosition(dungeon.data.current_room.id);
	}

	private IEnumerator OnMonser()
	{
		if (null == dungeon.data.current_room.monster)
		{
			yield break;
		}

		AudioManager.Instance.Stop(AudioManager.DUNGEON_BGM);
		AudioManager.Instance.Play(AudioManager.BATTLE_BGM, true);
		battle.gameObject.SetActive(true);
		yield return battle.BattleStart(dungeon.data.current_room.monster);
		battle.gameObject.SetActive(false);
		AudioManager.Instance.Stop(AudioManager.BATTLE_BGM);
		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
	}
	private IEnumerator Win(Monster.Meta meta)
	{
		dungeon_move.touch_input.block_count++;
		dungeon.data.current_room.monster = null;
		GameManager.Instance.player.enemy_slain_count++;

		int prevPlayerLevel = GameManager.Instance.player.level;
		Stat stat = GameManager.Instance.player.stats;
		int rewardCoin = meta.reward.coin + (int)Random.Range(-meta.reward.coin * 0.1f, meta.reward.coin * 0.1f);
		rewardCoin += (int)(rewardCoin * 0.1f * dungeon.level);
		int bonusCoin = (int)(rewardCoin * stat.GetStat(StatType.CoinBonus) / 100.0f);
		int rewardExp = meta.reward.exp + (int)Random.Range(-meta.reward.exp * 0.1f, meta.reward.exp * 0.1f);
		rewardExp += (int)(rewardExp * 0.1f * dungeon.level);
		int bonusExp = (int)(rewardExp * stat.GetStat(StatType.ExpBonus) / 100.0f);

		GameManager.Instance.player.ChangeCoin(rewardCoin + bonusCoin, false);
		GameManager.Instance.player.AddExp(rewardExp + bonusExp);

		GameManager.Instance.player.collect_coin_count += rewardCoin + bonusCoin;
		GameManager.Instance.player.total_exp += rewardExp + bonusExp;

		if (meta.reward_item_chance > Random.Range(0.0f, 100.0f))
		{
			if ("" == meta.reward.item_id)
			{
				dungeon.data.current_room.item = EquipItemManager.Instance.GetRandomMeta();
			}
			else
			{
				dungeon.data.current_room.item = ItemManager.Instance.FindMeta<Item.Meta>(meta.reward.item_id);
			}
		}

		Database.Execute(Database.Type.UserData, "UPDATE user_data SET player_coin=" + GameManager.Instance.player.coin);
		ProgressManager.Instance.Update(ProgressType.EnemiesSlain, meta.id, 1);
	
		CreateCoins(rewardCoin + bonusCoin);

		for (int i = prevPlayerLevel; i < GameManager.Instance.player.level; i++)
		{
			yield return player_exp.SetCurrent(player_exp.max, 0.3f);
			player_exp.max = GameManager.Instance.player.meta.GetMaxExp(i);
			yield return player_exp.SetCurrent(0.0f, 0.0f);
		}
		
		player_exp.max = GameManager.Instance.player.meta.GetMaxExp(GameManager.Instance.player.level);
		player_exp.current = GameManager.Instance.player.exp;
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;

		string battleResult = "";
		battleResult += GameText.GetText("DUNGEON/BATTLE/DEFEATED", "You", meta.name) + "\n";
		battleResult += "Coins : +" + rewardCoin + (0 < bonusCoin ? "(" + bonusCoin + " bonus)" : "") + "\n";
		battleResult += "Exp : +" + rewardExp + (0 < bonusExp ? "(" + bonusExp + " bonus)" : "") + "\n";
		if (prevPlayerLevel < GameManager.Instance.player.level)
		{
			battleResult += "Level : " + prevPlayerLevel + " -> " + GameManager.Instance.player.level + "\n";
		}

		yield return GameManager.Instance.ui_textbox.Write(battleResult, false);

		mini_map.CurrentPosition(dungeon.data.current_room.id);
		dungeon_move.touch_input.block_count--;
	}
	private IEnumerator Lose()
	{
		dungeon_move.touch_input.block_count++;
		ProgressManager.Instance.Update(ProgressType.DieCount, "", 1);

		Rect prev = GameManager.Instance.ui_textbox.Resize(1000 /*Screen.currentResolution.height * GameManager.Instance.canvas.scaleFactor * 0.8f*/);
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(
			"You died.\n\n" +
			"collect coin : " + GameManager.Instance.player.collect_coin_count + "\n" +
			"collect item : " + GameManager.Instance.player.collect_item_count + "\n" +
			"open box : " + GameManager.Instance.player.open_box_count + "\n" +
			"move : " + GameManager.Instance.player.move_count + "\n" +
			"level : " + GameManager.Instance.player.level + "(exp:" + GameManager.Instance.player.total_exp + ")\n"
		));
		GameManager.Instance.ui_textbox.Resize(prev.height);
		yield return GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.5f);
		yield return GameManager.Instance.advertisement.Show(Advertisement.PlacementType.Video);
		dungeon_move.touch_input.block_count--;
		yield return GameManager.Instance.AsyncLoadScene("Start");
		yield return GameManager.Instance.AsyncUnloadScene("Dungeon");
	}

	private void OnTreasureBox()
	{
		if (null == dungeon.data.current_room.monster && null != dungeon.data.current_room.item)
		{
			dungeon.current_room.treasure_box.Show(dungeon.data.current_room);
		}
	}
	private IEnumerator GoDown()
	{
		Vector3 position = Camera.main.transform.position;
		StartCoroutine(GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.5f));
		yield return StartCoroutine(MoveTo(Camera.main.gameObject, iTween.Hash(
			"x", dungeon.current_room.stair.transform.position.x,
			"y", dungeon.current_room.stair.transform.position.y,
			"z", dungeon.current_room.stair.transform.position.z - 0.5f,
			"time", 1.0f
		), true));
		InitDungeon();
		Camera.main.transform.position = position;
	}

	private IEnumerator OnExitUnlock()
	{
		if (Room.Type.Exit == dungeon.data.current_room.type)
		{
			dungeon_move.touch_input.block_count++;
			bool yes = false;
			GameManager.Instance.ui_textbox.on_submit += () => {
				yes = true;
				GameManager.Instance.ui_textbox.Close();
			};
			yield return GameManager.Instance.ui_textbox.Write("Do you want to go down the stair?", false);
			if (true == yes)
			{
				yield return StartCoroutine(GoDown());
			}
			dungeon_move.touch_input.block_count--;
		}
	}

	private IEnumerator OnShop()
	{
		if (Room.Type.Shop == dungeon.data.current_room.type)
		{
			dungeon.data.current_room.npc_sprite_path = "Npc/npc_graverobber";
			dungeon.current_room.Init(dungeon.data.current_room);
			bool yes = false;
			GameManager.Instance.ui_textbox.on_submit += () => {
				yes = true;
				GameManager.Instance.ui_textbox.Close();
			};
			yield return GameManager.Instance.ui_npc.Write("Npc/npc_graverobber_portrait", new string[] { "[전설의 상인]\n오! 이곳에서 사람은 오랜만이구만! 물건좀 보겠소?" });
			if (true == yes)
			{
				yield return shop.Open();
			}
			yield return GameManager.Instance.ui_npc.Write("Npc/npc_graverobber_portrait", new string[] { "[전설의 상인]\n그럼 좋은 여행하게나..킬킬킬!" });
			
			dungeon.data.current_room.npc_sprite_path = "";
			dungeon.data.current_room.type = Room.Type.Normal;
			dungeon.current_room.npc.gameObject.SetActive(false);
		}
	}
	private void CreateCoins(int amount)
	{
		int total = amount;
		int multiply = 1;
		float scale = 1.0f;

		while (0 < total)
		{
			int countCount = Random.Range(5, 10);
			for (int i = 0; i < countCount; i++)
			{
				Coin coin = GameObject.Instantiate<Coin>(coin_prefab);
				coin.amount = Mathf.Min(total, multiply);
				coin.destroy_position = ui_coin_position;
				coin.transform.SetParent(coin_spot, false);
				coin.transform.localScale = new Vector3(scale, scale, 1.0f);
				coin.transform.localPosition = Vector3.zero;
				coin.gameObject.SetActive(true);
				iTween.MoveBy(coin.gameObject, new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0.0f, 0.5f), 0.0f), 0.5f);

				total -= coin.amount;
				if (0 >= total)
				{
					return;
				}
			}
			multiply *= 10;
			scale += 0.1f;
		}
	}

	private void OnMove(int direction)
	{
		StartCoroutine(Move(direction));
	}

	private void OnItemAdd(Item item)
	{
		string text = GameManager.Instance.player.inventory.count.ToString() + "/" + Inventory.MAX_SLOT_COUNT.ToString();

		if (GameManager.Instance.player.inventory.count == Inventory.MAX_SLOT_COUNT)
		{
			text_inventory.text = "<color=red>" + text + "</color>";
		}
		else
		{
			text_inventory.text = text;
		}
	}

	private void OnItemRemove(Item item)
	{
		text_inventory.text = GameManager.Instance.player.inventory.count.ToString() + "/" + Inventory.MAX_SLOT_COUNT.ToString();
	}

	private void OnPlayerStatChange()
	{
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
	}

	public void OnQuestStart(Quest quest)
	{
		dungeon.data.current_room.npc_sprite_path = quest.sprite_path;
		dungeon.current_room.Init(dungeon.data.current_room);
		mini_map.CurrentPosition(dungeon.data.current_room.id);
	}

	public void OnQuestComplete(Quest quest)
	{
		dungeon.data.current_room.npc_sprite_path = "";
		dungeon.current_room.Init(dungeon.data.current_room);
	}
}
