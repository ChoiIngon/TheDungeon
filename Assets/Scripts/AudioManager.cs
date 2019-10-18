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
		{
			Object[] audioClips = Resources.LoadAll("Audio/BGM", typeof(AudioClip));
			foreach (var audioClip in audioClips)
			{
				audio_sources[audioClip.name] = CreateAudioSource("BGM", (AudioClip)audioClip);
			}
		}
		{
			Object[] audioClips = Resources.LoadAll("Audio/SFX", typeof(AudioClip));
			foreach (var audioClip in audioClips)
			{
				audio_sources[audioClip.name] = CreateAudioSource("SFX", (AudioClip)audioClip);
			}
		}

		DontDestroyOnLoad(AudioManager.Instance.gameObject);
	}

	private AudioSource CreateAudioSource(string tag, AudioClip audioClip)
	{
		GameObject audio = new GameObject();
		audio.transform.SetParent(transform, false);
		audio.name = tag;

		AudioSource audioSource = audio.AddComponent<AudioSource>();
		audioSource.clip = audioClip;
		audioSource.loop = false;
		audioSource.playOnAwake = false;
		return audioSource;
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

	public void Volumn(string tag, float volumn)
	{
		foreach (var itr in audio_sources)
		{
			AudioSource audioSource = itr.Value;
			if (tag == audioSource.gameObject.name)
			{
				audioSource.volume = volumn;
			}
		}
	}


}
