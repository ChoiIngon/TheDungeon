using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITicker : MonoBehaviour {
    public Text contents;
    public Image image;
    public float time;
    private static UITicker _instance;
    public static UITicker Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = (UITicker)GameObject.FindObjectOfType(typeof(UITicker));
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "UITicker";
                    _instance = container.AddComponent<UITicker>();
                }
                _instance.Init();
            }
            return _instance;
        }
    }

    void Start()
    {
        //gameObject.SetActive(false);
    }
    void Init()
    {
        //gameObject.SetActive(false);
    }
    
    public IEnumerator Write(string text)
    {
        gameObject.SetActive(true);
        contents.text = text;
        float alpha = 0.0f;
        while (1.0f > alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            contents.color = new Color(contents.color.r, contents.color.g, contents.color.b, alpha);
            alpha += Time.deltaTime /(time * 0.3f);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        contents.color = new Color(contents.color.r, contents.color.g, contents.color.b, 1.0f);

        yield return new WaitForSeconds(time * 0.3f);
        while (0.0f < alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            contents.color = new Color(contents.color.r, contents.color.g, contents.color.b, alpha);
            alpha -= Time.deltaTime / (time * 0.3f);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
