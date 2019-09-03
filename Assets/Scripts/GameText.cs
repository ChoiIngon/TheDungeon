using System.Collections.Generic;

class GameText : Util.Singleton<GameText>
{
	public enum LanguageCode
	{
		English,
		Korean,
		Max
	}

	public LanguageCode language_code = LanguageCode.English;
	Dictionary<string, List<string>> textByLanguageCode = new Dictionary<string, List<string>>() {
		{ "WAVE_START", new List<string> { "Wave Start", "웨이브 스타트" } },
		{ "WAVE", new List<string> { "WAVE", "웨이브" } },
		{ "UPGRADE_CITADEL", new List<string> { "Upgrade Citadel", "요새 업그레이드" } },
		{ "ERROR_HERO_UNIT_DEPLOY", new List<string> { "deploy hero unit before starting wave", "마법사를 먼저 성에 배치 해주세요" } },
	};

	public void SetLanguage(LanguageCode code)
	{
		language_code = code;
	}

	public string GetText(string textKey)
	{
		if (false == textByLanguageCode.ContainsKey(textKey))
		{
			throw new System.Exception("invalid text key(text_key:" + textKey + ")");
		}

		return textByLanguageCode[textKey][(int)language_code];
	}

}