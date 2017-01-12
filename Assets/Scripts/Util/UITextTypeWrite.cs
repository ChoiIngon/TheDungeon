using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Text))]
public class UITextTypeWrite : MonoBehaviour {
    Text text;
    string copied;
    AudioSource audio;
    public float charPerSecond = 20.0f;
    void Awake()
    {
        text = GetComponent<Text>();
        copied = text.text;
        text.text = "";

        audio = GetComponent<AudioSource>();
        StartCoroutine(Play());
    }


    public IEnumerator Play()
    {
        foreach (char c in copied)
        {
            text.text += c;
            audio.Play();
            yield return new WaitForSeconds(1.0f / charPerSecond);
            audio.Stop();
        }
    }
}
