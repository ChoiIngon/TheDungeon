using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ImageOutline : MonoBehaviour
{
    public Color color = Color.white;
    public bool outline = false;
    //public int size = 0;

    private Image image;

	void OnEnable()
	{
		image = GetComponent<Image>();
	}
    void Update()
    {
        Material mat = Instantiate(image.material);
		mat.name = name;
        mat.SetFloat("_Outline", outline ? 1f : 0f);
        mat.SetColor("_OutlineColor", color);
        //mat.SetFloat("_OutlineSize", size);
        image.material = mat;
    }
}