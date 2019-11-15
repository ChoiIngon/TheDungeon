using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneStart : SceneMain
{
	private UIAchievement ui_achievement;
	private Button start_button;
	private Button setting_button;
	private Button reset_button;
	private Button achievement_button;
	private void Awake()
	{
		start_button = UIUtil.FindChild<Button>(transform, "UI/StartButton");
		setting_button = UIUtil.FindChild<Button>(transform, "UI/SettingButton");
		reset_button = UIUtil.FindChild<Button>(transform, "UI/ResetButton");
		achievement_button = UIUtil.FindChild<Button>(transform, "UI/AchievementButton");
		ui_achievement = UIUtil.FindChild<UIAchievement>(transform, "UI/UIAchievement");
	}

	public override IEnumerator Run()
	{
		if ("Start" == SceneManager.GetActiveScene().name)
		{
			AsyncOperation operation = SceneManager.LoadSceneAsync("Common", LoadSceneMode.Additive);
			while (false == operation.isDone)
			{
				yield return null;
			}
			yield return StartCoroutine(GameManager.Instance.Init());
		}
				
		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
		
		UIUtil.AddPointerUpListener(start_button.gameObject, () =>
		{
			StartCoroutine(OnStart());
		});
				
		UIUtil.AddPointerUpListener(setting_button.gameObject, () =>
		{
			GameManager.Instance.ui_setting.gameObject.SetActive(true);
		});
				
		UIUtil.AddPointerUpListener(reset_button.gameObject, () =>
		{
			Database.Disconnect(Database.Type.UserData);
			System.IO.File.Delete(Application.persistentDataPath + "/user_data.db");
			Database.Connect(Database.Type.UserData, Application.persistentDataPath + "/user_data.db");
			StartCoroutine(OnStart());
		});

		UIUtil.AddPointerUpListener(achievement_button.gameObject, () =>
		{
			ui_achievement.gameObject.SetActive(true);
		});

		yield return GameManager.Instance.CameraFade(Color.black, new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.0f);
	}

	private IEnumerator OnStart()
	{
		yield return GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.0f, true);
		start_button.gameObject.SetActive(false);
		setting_button.gameObject.SetActive(false);
		reset_button.gameObject.SetActive(false);
		achievement_button.gameObject.SetActive(false);
		/*
		string[] scripts = new string[] {
			"오래전 이 던전엔 자신의 부와 젊음을 위해 백성들을 악마의 제물로 바쳤다는 피의 여왕이 살았다고 하네. ",
			"시간이 지나 이젠 전설이 되었지만 한가지 확실한건 저 곳엔 여왕이 남긴 엄청난 보물이 있다는 거야.",
			"하지만 지금까지 저곳으로 들어서 무사히 돌아나온 사람은 없다는군."
		};
		yield return GameManager.Instance.ui_npc.Write("npc_farseer", scripts);
		*/
		yield return GameManager.Instance.AsyncLoadScene("Dungeon");
		yield return GameManager.Instance.AsyncUnloadScene("Start");
	}

}
