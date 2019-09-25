using UnityEngine;
using UnityEngine.UI;

public class ImageOutline : MonoBehaviour
{
    public Color color = Color.white;
    public bool active = false;
	[Range(1, 16)]
    public int size = 1;

    private Image image;

	void OnEnable()
	{
		image = GetComponent<Image>();
	}
    void Update()
    {
        Material mat = Instantiate(image.material);
		mat.name = name;
        mat.SetFloat("_Outline", active ? 1f : 0f);
        mat.SetColor("_OutlineColor", color);
        mat.SetFloat("_OutlineSize", size);
        image.material = mat;
	}
}