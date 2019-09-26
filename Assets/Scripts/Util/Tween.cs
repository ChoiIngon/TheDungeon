using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Util
{
	public class UITween : MonoSingleton<UITween>
	{
		public static IEnumerator ColorTo(Image image, Color to, float time)
		{
			return UITween.Instance._ColorTo(image, to, time);
		}

		private IEnumerator _ColorTo(Image image, Color to, float time)
		{
			Color from = image.color;
			Color amount = Color.white;
			amount.r = to.r - image.color.r;
			amount.g = to.g - image.color.g;
			amount.b = to.b - image.color.b;
			amount.a = to.a - image.color.a;
			
			Color delta = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			while (Mathf.Abs(delta.r) < Mathf.Abs(amount.r) || Mathf.Abs(delta.g) < Mathf.Abs(amount.g) || Mathf.Abs(delta.b) < Mathf.Abs(amount.b) || Mathf.Abs(delta.a) < Mathf.Abs(amount.a))
			{
				delta.r += amount.r * (Time.deltaTime / time);
				delta.g += amount.g * (Time.deltaTime / time);
				delta.b += amount.b * (Time.deltaTime / time);
				delta.a += amount.a * (Time.deltaTime / time);
				image.color = new Color(from.r + delta.r, from.g + delta.g, from.b + delta.b, from.a + delta.a);
				yield return null;
			}
			image.color = to;
		}

		public static IEnumerator ColorTo(SpriteRenderer renderer, Color to, float time)
		{
			return UITween.Instance._ColorTo(renderer, to, time);
		}

		private IEnumerator _ColorTo(SpriteRenderer renderder, Color to, float time)
		{
			Color from = renderder.color;
			Color amount = Color.white;
			amount.r = to.r - renderder.color.r;
			amount.g = to.g - renderder.color.g;
			amount.b = to.b - renderder.color.b;
			amount.a = to.a - renderder.color.a;

			Color delta = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			while (Mathf.Abs(delta.r) < Mathf.Abs(amount.r) || Mathf.Abs(delta.g) < Mathf.Abs(amount.g) || Mathf.Abs(delta.b) < Mathf.Abs(amount.b) || Mathf.Abs(delta.a) < Mathf.Abs(amount.a))
			{
				delta.r += amount.r * (Time.deltaTime / time);
				delta.g += amount.g * (Time.deltaTime / time);
				delta.b += amount.b * (Time.deltaTime / time);
				delta.a += amount.a * (Time.deltaTime / time);
				renderder.color = new Color(from.r + delta.r, from.g + delta.g, from.b + delta.b, from.a + delta.a);
				yield return null;
			}
			renderder.color = to;
		}

	}
}