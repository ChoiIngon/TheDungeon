using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Util.MonoSingleton<AudioManager>
{
	public const string DUNGEON_BGM = "dungeon_bgm";
	public const string DUNGEON_WALK = "dungeon_walk";
	public const string BATTLE_BGM = "battle_bgm";
	public const string TEXTBOX_TYPE = "textbox_type";
	public const string DOOR_OPEN = "door_open";
	public const string BOX_OPEN = "box_open";

	private Dictionary<string, AudioSource> audio_sources = new Dictionary<string, AudioSource>();

	public void Init()
	{
		Object[] audioClips = Resources.LoadAll("Audio", typeof(AudioClip));
		foreach (var audioClip in audioClips)
		{
			GameObject audio = new GameObject();

			audio.transform.SetParent(transform, false);
			audio.name = audioClip.name;

			AudioSource audioSource = audio.AddComponent<AudioSource>();
			audioSource.clip = (AudioClip)audioClip;
			audioSource.loop = false;
			audioSource.playOnAwake = false;
			audio_sources[audio.name] = audioSource;
		}
		DontDestroyOnLoad(AudioManager.Instance.gameObject);
	}

	public void Play(string name, bool loop = false)
	{
		if (false == audio_sources.ContainsKey(name))
		{
			throw new System.Exception("invalid audiosource name(" + name + ")");
		}

		AudioSource audioSource = audio_sources[name];
		audioSource.loop = loop;
		audioSource.Play();
	}

	public void Stop(string name)
	{
		if (false == audio_sources.ContainsKey(name))
		{
			throw new System.Exception("invalid audiosource name(" + name + ")");
		}

		AudioSource audioSource = audio_sources[name];
		audioSource.Stop();
	}

	public void Mute(bool flag)
	{
		foreach (var itr in audio_sources)
		{
			AudioSource audioSource = itr.Value;
			audioSource.mute = flag;
		}
	}

	public void Volumn(float volumn)
	{
		foreach (var itr in audio_sources)
		{
			AudioSource audioSource = itr.Value;
			audioSource.volume = volumn;
		}
	}
}
