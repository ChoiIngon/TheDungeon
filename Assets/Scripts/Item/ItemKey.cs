using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Item {
	
	public KeyItem() : base(null) {
	}
    /*
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("OPEN");
		actions.Add ("THROW");
		return actions;
	}
	public new class Info : Item.Info
	{
		public override Item CreateInstance()
		{
			KeyItem item = new KeyItem ();
			item.id = id;
			item.name = name;
			item.icon = ResourceManager.Instance.Load<Sprite> (icon);
			item.price = 0;
			item.grade = Item.Grade.Normal;
			item.description = description;
			return item;	
		}
	}
	*/
}

