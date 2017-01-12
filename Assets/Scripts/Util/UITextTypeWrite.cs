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
    ScrollRect scrollRect;
    void Awake()
    {
        text = GetComponent<Text>();
        copied = text.text;
        text.text = "";

        audio = GetComponent<AudioSource>();
        scrollRect = GetComponentInParent<ScrollRect>();
        RectTransform rt = scrollRect.GetComponent<RectTransform>();
        text.fontSize = (int)(rt.rect.height / 5.0f - text.lineSpacing);
        StartCoroutine(Play());
    }


    public IEnumerator Play()
    {

        foreach (char c in copied)
        {
            audio.Play();
            text.text += c;
            if(null != scrollRect)
            {
                scrollRect.verticalNormalizedPosition = 0.0f;
            }
            
            yield return new WaitForSeconds(1.0f / charPerSecond);
        }
        
    }
}
