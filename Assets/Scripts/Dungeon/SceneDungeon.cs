using UnityEngine;
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

	public Button button_inventory;
	public Text text_inventory;
	private Transform ui_player_transform;
	public UIGaugeBar player_health;
	public UIGaugeBar player_exp;
	
	public UIShop shop;
	public UIInventory ui_inventory;

	private Text ui_dungeon_level;
	private Transform coin_spot;
    private Coin coin_prefab;
	private DungeonMoveButtons move_buttons;

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

		dungeon = UIUtil.FindChild<Dungeon>(transform, "Dungeon");
		battle = UIUtil.FindChild<DungeonBattle>(transform, "Battle");
		battle.gameObject.SetActive(true);
				
		move_buttons = UIUtil.FindChild<DungeonMoveButtons>(transform, "UI/MoveButtons");
		ui_inventory = UIUtil.FindChild<UIInventory>(transform, "UI/UIInventory");
		ui_inventory.Init();
		ui_inventory.gameObject.SetActive(false);

		button_inventory = UIUtil.FindChild<Button>(transform, "UI/Dungeon/ButtonInventory");
		UIUtil.AddPointerUpListener(button_inventory.gameObject, () =>
		{
			ui_inventory.gameObject.SetActive(true);
		});
		text_inventory = UIUtil.FindChild<Text>(transform, "UI/Dungeon/ButtonInventory/Text");
		
		mini_map = UIUtil.FindChild<UIMiniMap>(transform,		"UI/Dungeon/MiniMap");
		ui_dungeon_level = UIUtil.FindChild<Text>(transform,	"UI/Dungeon/Level");

		ui_player_transform = UIUtil.FindChild<Transform>(transform, "UI/Player");
		player_health = UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Health");
		player_exp =	UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Exp");

		UICoin uiCoin = UIUtil.FindChild<UICoin>(transform, "UI/UICoin");
		uiCoin.gameObject.SetActive(true);
		
		coin_spot = UIUtil.FindChild<Transform>(transform, "CoinSpot");
		coin_spot.gameObject.SetActive(true);
		coin_prefab = UIUtil.FindChild<Coin>(transform, "Prefabs/Coin");

		shop = UIUtil.FindChild<UIShop>(transform, "UI/UIShop");

		Util.EventSystem.Subscribe<int>(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Subscribe(EventID.Dungeon_Exit_Unlock, () => { StartCoroutine(OnExitUnlock()); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Map_Reveal, () => { mini_map.RevealMap(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Monster_Reveal, () => { mini_map.RevealMonster(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Treasure_Reveal, () => { mini_map.RevealTreasure(); });
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Subscribe(EventID.Player_Stat_Change, OnPlayerStatChange);
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Start, OnQuestStart);
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Complete, OnQuestComplete);
		Util.EventSystem.Subscribe(EventID.MiniMap_Show, OnShowMiniMap);
		Util.EventSystem.Subscribe<float>(EventID.MiniMap_Hide, OnHideMiniMap);
		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);

		InitScene();			
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<int>(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Exit_Unlock);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Map_Reveal);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Monster_Reveal);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Treasure_Reveal);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Unsubscribe(EventID.Player_Stat_Change, OnPlayerStatChange);
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Start, OnQuestStart);
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Complete, OnQuestComplete);
		Util.EventSystem.Unsubscribe(EventID.MiniMap_Show, OnShowMiniMap);
		Util.EventSystem.Unsubscribe<float>(EventID.MiniMap_Hide, OnHideMiniMap);
	}

	private void InitScene()
	{
		GameManager.Instance.player.Init();

		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
		player_exp.max = GameManager.Instance.player.GetMaxExp();
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
		move_buttons.Init(dungeon.data.current_room);
		mini_map.Init(dungeon);
		mini_map.CurrentPosition(dungeon.data.current_room.id);

		ui_dungeon_level.text = "<size=" + (ui_dungeon_level.fontSize * 0.8f) + ">B</size> " + dungeon_level.ToString();

		ProgressManager.Instance.Update(ProgressType.DungeonLevel, "", dungeon.level);
		StartCoroutine(GameManager.Instance.ui_textbox.Write(GameText.GetText("DUNGEON/WELCOME", dungeon.level)));
	}

	private IEnumerator Move(int direction)
	{
		while (0 < coin_spot.childCount)
		{
			yield return null;
		}

		yield return dungeon.Move(direction);
		move_buttons.Init(dungeon.data.current_room);
		mini_map.CurrentPosition(dungeon.data.current_room.id);

		if (Room.Type.Exit == dungeon.data.current_room.type || Room.Type.Lock == dungeon.data.current_room.type)
		{
			StartCoroutine(mini_map.Hide(0.5f, 0.3f));
		}
		else
		{
			StartCoroutine(mini_map.Show(0.5f));
		}

		yield return OnExitUnlock();
		yield return OnMonser();
		yield return OnTreasureBox();
		yield return OnShop();		
		
		if ("" != dungeon.data.current_room.npc_sprite_path)
		{
			ProgressManager.Instance.Update(ProgressType.MeetNpc, dungeon.data.current_room.npc_sprite_path, 1);
		}

		move_buttons.Init(dungeon.data.current_room);
		mini_map.CurrentPosition(dungeon.data.current_room.id);
		Util.EventSystem.Publish(EventID.Dungeon_Move_Finish);
	}

	private IEnumerator OnMonser()
	{
		if (null == dungeon.data.current_room.monster)
		{
			yield break;
		}

		AudioManager.Instance.Stop(AudioManager.DUNGEON_BGM);
		AudioManager.Instance.Play(AudioManager.BATTLE_BGM, true);

		button_inventory.gameObject.SetActive(false);
		yield return battle.BattleStart(dungeon.data.current_room.monster);
		button_inventory.gameObject.SetActive(true);

		AudioManager.Instance.Stop(AudioManager.BATTLE_BGM);
		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
		if (DungeonBattle.BattleResult.Win == battle.battle_result)
		{
			yield return Win(dungeon.data.current_room.monster);
		}
		else if (DungeonBattle.BattleResult.Lose == battle.battle_result)
		{
			yield return Lose();
		}

		/*
		else if (DungeonBattle.BattleResult.Runaway == battle.battle_result)
		{
			int runawayDirection = (direction + 2) % 4;
			if (0 <= runawayDirection || Dungeon.WIDTH * Dungeon.HEIGHT > runawayDirection)
			{
				Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, runawayDirection);
			}
		}
		*/
	}
	private IEnumerator Win(Monster.Meta meta)
	{
		GameManager.Instance.player.enemy_slain_count++;

		int prevPlayerLevel = GameManager.Instance.player.level;
		Stat stat = GameManager.Instance.player.stats;
		int rewardCoin = meta.reward.coin + (int)Random.Range(-meta.reward.coin * 0.1f, meta.reward.coin * 0.1f);
		int bonusCoin = (int)(rewardCoin * stat.GetStat(StatType.CoinBonus) / 100.0f);
		int rewardExp = meta.reward.exp + (int)Random.Range(-meta.reward.exp * 0.1f, meta.reward.exp * 0.1f);
		int bonusExp = (int)(rewardExp * stat.GetStat(StatType.ExpBonus) / 100.0f);

		GameManager.Instance.player.ChangeCoin(rewardCoin + bonusCoin, false);
		GameManager.Instance.player.AddExp(rewardExp + bonusExp);

		GameManager.Instance.player.collect_coin_count += rewardCoin + bonusCoin;
		GameManager.Instance.player.total_exp += rewardExp + bonusExp;

		Database.Execute(Database.Type.UserData, "UPDATE user_data SET player_coin=" + GameManager.Instance.player.coin);
		ProgressManager.Instance.Update(ProgressType.EnemiesSlain, meta.id, 1);
	
		CreateCoins(rewardCoin + bonusCoin);

		for (int i = prevPlayerLevel; i < GameManager.Instance.player.level; i++)
		{
			yield return player_exp.SetCurrent(player_exp.max, 0.3f);
			player_exp.max = GameManager.Instance.player.GetMaxExp(i);
			yield return player_exp.SetCurrent(0.0f, 0.0f);
		}
		
		player_exp.max = GameManager.Instance.player.GetMaxExp();
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

		yield return GameManager.Instance.ui_textbox.Write(battleResult);
		dungeon.data.current_room.monster = null;
		mini_map.CurrentPosition(dungeon.data.current_room.id);
	}
	private IEnumerator Lose()
	{
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
		yield return GameManager.Instance.advertisement.ShowAds();
		yield return GameManager.Instance.AsyncLoadScene("Start");
		yield return GameManager.Instance.AsyncUnloadScene("Dungeon");
	}

	private IEnumerator OnTreasureBox()
	{
		if (null == dungeon.data.current_room.monster && null != dungeon.data.current_room.item)
		{
			dungeon.current_room.treasure_box.Show(dungeon.data.current_room);
		}
		yield break;
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
			move_buttons.touch_input.AddBlockCount();
			bool yes = false;
			GameManager.Instance.ui_textbox.on_submit += () => {
				yes = true;
				GameManager.Instance.ui_textbox.Close();
			};
			yield return GameManager.Instance.ui_textbox.Write("Do you want to go down the stair?");
			if (true == yes)
			{
				yield return StartCoroutine(GoDown());
			}
			move_buttons.touch_input.ReleaseBlockCount();
		}
	}

	private IEnumerator OnShop()
	{
		if (Room.Type.Shop == dungeon.data.current_room.type)
		{
			StartCoroutine(mini_map.Hide(0.5f));

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
			StartCoroutine(mini_map.Show(0.5f));
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

	private void OnMoveStart(int direction)
	{
		StartCoroutine(mini_map.Show(0.5f));
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

	public void OnShowMiniMap()
	{
		StartCoroutine(mini_map.Show(0.5f));
	}
	public void OnHideMiniMap(float alpha)
	{
		StartCoroutine(mini_map.Hide(0.5f, alpha));
	}
}
