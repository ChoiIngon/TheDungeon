using UnityEngine;
using System.Collections;

using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : Toggle
{
	#region Inspector
	// ReSharper disable InconsistentNaming
	Sprite normalSprite;
	// ReSharper restore InconsistentNaming
	#endregion

	protected override void Start()
	{
		base.Start();

		normalSprite = ((Image)targetGraphic).sprite;
		onValueChanged.AddListener(value =>
		{
			switch (transition)
			{
				case Transition.ColorTint: 
					image.color = isOn ? colors.pressedColor : colors.normalColor; 
					break;
				case Transition.SpriteSwap: 
					image.sprite = isOn ? spriteState.pressedSprite : normalSprite; 
					break;
				default: 
					throw new NotImplementedException();
			}
		});
	}
}