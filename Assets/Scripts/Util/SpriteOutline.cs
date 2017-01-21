using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color color = Color.white;
    public int size = 0;
    private SpriteRenderer spriteRenderer;

	void OnEnable()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
    void Update()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", 1f);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", size);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}