using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ImageOutline : MonoBehaviour
{
    public Color color = Color.white;
    public int size = 0;

    private Image image;

	void OnEnable()
	{
		image = GetComponent<Image>();
	}
    void Update()
    {
        
        Material mat = Instantiate(image.material);
        mat.SetFloat("_Outline", 1f);
        mat.SetColor("_OutlineColor", color);
        mat.SetFloat("_OutlineSize", size);
        image.material = mat;
    }
}