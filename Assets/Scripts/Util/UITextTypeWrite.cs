using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class UITextTypeWrite : MonoBehaviour {
    Text text;
    string copied;
    public float charPerSecond = 20.0f;
    void Awake()
    {
        text = GetComponent<Text>();
        copied = text.text;
        text.text = "";

        StartCoroutine(Play());
    }


    public IEnumerator Play()
    {
        foreach (char c in copied)
        {
            text.text += c;
            yield return new WaitForSeconds(1.0f / charPerSecond);
        }
    }
}
