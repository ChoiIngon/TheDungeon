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
	Dictionary<string, string> texts = new Dictionary<string, string>()
	{
		{ "ACHIEVE/COMPLETE", "Complete {0}." },
		{ "DUNGEON/WELCOME", "Welcome to the level {0} of dungeon." },
		{ "DUNGEON/BATTLE/HIT", "{0} hit {1}." },
		{ "DUNGEON/BATTLE/DEFEATED", "{0} defeated {1}." },
		{ "DUNGEON/HAVE_ITEM", "{0} now have {1} item." },
		{ "ERROR/INVENTORY_FULL", "Inventroy is full." },
		{ "ERROR/UNLOCK_DOOR", "You need key." },
	};

	public void SetLanguage(LanguageCode code)
	{
		language_code = code;
	}

	public static string GetText(string key, params object[] args)
	{
		return Instance._GetText(key, args);
	}
	private string _GetText(string key, params object[] args)
	{
		if (false == texts.ContainsKey(key))
		{
			throw new System.Exception("invalid text key(text_key:" + key + ")");
		}

		return string.Format(texts[key], args);
	}

}