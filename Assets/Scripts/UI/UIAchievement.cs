using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievement : MonoBehaviour
{
	public GameObject slot_prefab;
	private Transform content;
	private Scrollbar scrollbar;
	
	private void Awake()
	{
		content = UIUtil.FindChild<Transform>(transform, "Slots/Viewport/Content");
		scrollbar = UIUtil.FindChild<Scrollbar>(transform, "Slots/Scrollbar Vertical");
		Button close = UIUtil.FindChild<Button>(transform, "Close");
		UIUtil.AddPointerUpListener(close.gameObject, () => { gameObject.SetActive(false); });
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		while (0 < content.childCount)
		{
			Transform child = content.GetChild(0);
			child.SetParent(null);
			GameObject.Destroy(child.gameObject);
		}

		foreach (var itr in AchieveManager.Instance.achieves)
		{
			GameObject obj = GameObject.Instantiate<GameObject>(slot_prefab);
			UISlot slot = obj.AddComponent<UISlot>();

			slot.Init(itr.Value);
			slot.transform.SetParent(content, false);
		}
		scrollbar.value = 1.0f;
		/*
		for (int i = 0; i < 10; i++)
		{
			GameObject obj = GameObject.Instantiate<GameObject>(slot_prefab);
			UISlot slot = obj.AddComponent<UISlot>();
			slot.Init(null);
			slot.transform.SetParent(content, false);
		}
		*/
	}

	public class UISlot : MonoBehaviour
	{
		private Text description;
		private Image background;
		public Image achieve_mark;

		
		private void Awake()
		{
			achieve_mark = UIUtil.FindChild<Image>(transform, "achieve_mark");
			description = UIUtil.FindChild<Text>(transform, "description");
			background = UIUtil.FindChild<Image>(transform, "slot_bg");
		}

		public void Init(Achieve achieve)
		{
			if (0 == achieve.complete_step)
			{
				description.text = "<color=#6b6b6b>???</color>";
				background.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
			}
			else
			{
				description.text = "";
				description.text += "<color=#cea652>" + achieve.name + "</color>\n";

				description.text += "<color=#6b6b6b>" + achieve.meta.description + "</color>";
				description.text += " <color=#4eb105>" + achieve.reward_stat.ToString() + "</color>\n";
				achieve_mark.sprite = ResourceManager.Instance.Load<Sprite>(achieve.meta.sprite_path);
			}
			//if (1 == achieve.step && achieve.count < achieve.goal)
			//{
			//	description.text += "<color=#6b6b6b>???</color>";
			//}
			//description.text += "<color=#CEA652>" + achieve.name + "</color>";
		}
	}
}
