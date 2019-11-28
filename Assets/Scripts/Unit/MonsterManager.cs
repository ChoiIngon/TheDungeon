using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Util.Singleton<MonsterManager>
{
	private Dictionary<string, Monster.Meta> metas;
	public void Init()
	{
		metas = new Dictionary<string, Monster.Meta>();
		try
		{
			GoogleSheetReader sheetReader = new GoogleSheetReader(GameManager.GOOGLESHEET_ID, GameManager.GOOGLESHEET_API_KEY);
			sheetReader.Load("meta_monster");

			foreach (GoogleSheetReader.Row row in sheetReader)
			{
				Monster.Meta meta = new Monster.Meta();
				meta.id = row.GetString("monster_id");
				meta.name = row.GetString("monster_name");
				meta.health = row.GetFloat("health");
				meta.attack = row.GetFloat("attack");
				meta.defense = row.GetFloat("defense");
				meta.speed = row.GetFloat("speed");
				meta.sprite_path = row.GetString("sprite_path");
				meta.reward.coin = row.GetInt32("reward_coin");
				meta.reward.exp = row.GetInt32("reward_exp");
				metas.Add(meta.id, meta);
			}
		}
		catch (System.Exception e)
		{
			GameManager.Instance.ui_textbox.on_close = () =>
			{
				Application.Quit();
			};
			GameManager.Instance.ui_textbox.AsyncWrite("error: " + e.Message + "\n" + e.ToString(), false);
		}
	}

	public Monster.Meta FindMeta(string id)
	{
		if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can not find monster meta(monster_id:" + id + ")");
		}
		return metas[id];
	}
}