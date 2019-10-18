using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
	private Slider bgm;
	private Slider sfx;
	private Button close;

	private void Awake()
	{
		bgm = UIUtil.FindChild<Slider>(transform, "BGM/Slider");
		sfx = UIUtil.FindChild<Slider>(transform, "SFX/Slider");
		close = UIUtil.FindChild<Button>(transform, "BottomBar/Close");
	}

	public void Init()
	{
		CreateAchieveTableIfNotExists();

		Util.Database.DataReader reader = Database.Execute(Database.Type.UserData,
			"SELECT bgm_volume, sfx_volume FROM user_setting"
		);

		int rowCount = 0;
		while (true == reader.Read())
		{
			rowCount++;
			bgm.value = reader.GetFloat("bgm_volume");
			AudioManager.Instance.Volumn("BGM", bgm.value);
			sfx.value = reader.GetFloat("sfx_volume");
			AudioManager.Instance.Volumn("SFX", sfx.value);
		}

		if (0 == rowCount)
		{
			Database.Execute(Database.Type.UserData, "INSERT INTO user_setting(bgm_volume, sfx_volume) VALUES (1, 1)");
			bgm.value = 1.0f;
			AudioManager.Instance.Volumn("BGM", bgm.value);
			sfx.value = 1.0f;
			AudioManager.Instance.Volumn("SFX", sfx.value);
		}

		close.onClick.AddListener(() =>
		{
			gameObject.SetActive(false);
		});

		bgm.onValueChanged.AddListener((float value) =>
		{
			AudioManager.Instance.Volumn("BGM", value);
			Database.Execute(Database.Type.UserData, "UPDATE user_setting SET bgm_volume=" + value);
		});

		sfx.onValueChanged.AddListener((float value) =>
		{
			AudioManager.Instance.Volumn("SFX", value);
			Database.Execute(Database.Type.UserData, "UPDATE user_setting SET sfx_volume=" + value);
		});
	}

	private void CreateAchieveTableIfNotExists()
	{
		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_setting (" +
				"bgm_volume REAL NOT NULL DEFAULT 1," +
				"sfx_volume REAL NOT NULL DEFAULT 1" +
			")"
		);
	}
}
