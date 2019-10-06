using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item
{
	public new class Meta : Item.Meta
	{
		public override Item CreateInstance()
		{
			return new KeyItem(this);
		}
	}

	public KeyItem(Meta meta) : base(meta)
	{
	}
}

