using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PotionItem : Item {
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("DRINK");
		actions.Add ("THROW");
		return actions;
	}
	public virtual void Use(Unit target)
	{
	}
}
