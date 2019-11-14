using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISkillButton : MonoBehaviour
{
	public Image background;
	public Image skill_icon;
	public ImageOutline outline;
	public Text title;
	public Skill skill;
	
	private void Awake()
	{
		background = GetComponent<Image>();
		if (null == background)
		{
			throw new MissingComponentException("Button");
		}
		skill_icon = UIUtil.FindChild<Image>(transform, "Image");
		outline = UIUtil.FindChild<ImageOutline>(transform, "Image");
		title = UIUtil.FindChild<Text>(transform, "Text");
	}

	public void Init(Skill skill, System.Action action)
	{
		this.skill = skill;
		UIUtil.AddPointerUpListener(gameObject, action);
		title.text = skill.meta.skill_name;
		background.sprite = ResourceManager.Instance.Load<Sprite>(skill.meta.sprite_path);
		skill_icon.sprite = ResourceManager.Instance.Load<Sprite>(skill.meta.sprite_path);
		if (0.0f < skill.meta.cooltime)
		{
			skill_icon.fillAmount = ((float)skill.meta.cooltime - skill.cooltime) / skill.meta.cooltime;
		}
		else
		{
			skill_icon.fillAmount = 1.0f;
		}
		outline.active = false;
		if (1.0f <= skill_icon.fillAmount + 0.00001f)
		{
			outline.active = true;
		}
	}

	public void SetTitle(string title)
	{
		this.title.text = title;
	}

	public IEnumerator Refresh()
	{
		skill_icon.fillAmount = 0.0f;
		float fillAmount = 1.0f;

		if (0.0f >= skill.cooltime)
		{
			outline.active = true;
			skill_icon.fillAmount = fillAmount;
			yield break;
		}

		if (0.0f < skill.meta.cooltime)
		{
			fillAmount = ((float)skill.meta.cooltime - skill.cooltime) / skill.meta.cooltime;
		}

		while (skill_icon.fillAmount < fillAmount)
		{
			skill_icon.fillAmount += fillAmount * 0.2f;
			yield return new WaitForSeconds(0.1f);
		}
		skill_icon.fillAmount = fillAmount;
		outline.active = false;
		if (1.0f <= skill_icon.fillAmount + 0.00001f)
		{
			outline.active = true;
		}
	}	
}
