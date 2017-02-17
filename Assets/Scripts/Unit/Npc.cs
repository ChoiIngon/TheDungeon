using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour {
	public UITextBox textBox;
	public Image panel;
	public Image image;
	public Sprite sprite {
		set {
			image.sprite = value;
		}
	}

	// Use this for initialization
	void Start () {
		gameObject.SetActive (false);
	}

	public IEnumerator Talk(string text)
	{
		gameObject.SetActive (true);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * SceneMain.widthScale, "easeType", "easeInOutExpo"));
		string[] texts = new string[] {
			"Many heroes of all kinds ventured into the Dungeon before you.\n",
			"Some of them have returned with treasures and magical artifacts, most have never been heard of ever since.\n",
			"But none has succeeded in retrieving the Amulet of Yendor, which is told to be hidden in the depths of Pixel Dungeon.\n",
			"You consider yourself ready for the challenge, but most importantly you feel that fortune smiles upon you.\n",
			"It's time to start your own adventure!\r\nThe Dungeon lies right beneath the City, its upper levels are actually constitute the City's sewer system.\n",
			"Being nominally a part of the City, these levels are not that dangerous. No one will call them a safe place, but at least you won't need to deal with evil magic here.\n",
			"Many years ago an underground prison for the most dangerous criminals was built here. At that moment it seemed a very clever idea, this place, indeed, was very hard to escape.\n",
			"But soon dark miasma started to permeate from below, driving prisoners and guards insane.\n",
			"In the end the prison was abandoned, though some convicts were left locked here.\n",
			"The caves, which stretch down under the abandoned prison, are sparcely populated. They lie too deep to be exploited by the City and they are too poor in minerals to interest the dwarves.\n",
			"In the past there was a trade outpost somewhere here on the route between these two states, but it perished after the decline of Dwarven Metropolis.\n",
			"Only omnipresent gnolls and subterranean animals dwell here.\n",
			"Dwarven Metropolis was once the greatest of dwarven city-states. In its heyday the mechanized army of dwarves has successfully repelled the invasion of the old god and his demon army.\n",
			"But it is said,that the returning warriors have brought seeds of corruption with them and that victory was the beginning of the end for the underground kingdom.\r\n",
			"In the past these levels were the outskirts of Metropolis. After the costly victory in the war with the old god dwarves were too weakened to clear them of remaining demons. Gradually demons have consolidated their grip on this place and now it's called Demon Halls.\r\n",
			"Very few adventurers ever descended this far..."
		};

		yield return StartCoroutine(textBox.Write(texts));
		iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
		yield return StartCoroutine (textBox.Hide (0.5f));
	}
}
