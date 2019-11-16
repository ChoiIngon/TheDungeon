using UnityEngine;
using System.Collections;

public class EventID 
{
    public const string CoinAmountChanged = "CoinAmountChanged";
	public const string Inventory_Add = "Inventory_Add";
	public const string Inventory_Remove = "Inventory_Remove";
	public const string Inventory_Open = "Inventory_Open";
	public const string Inventory_Close = "Inventory_Close";

    public const string Item_Equip = "Item_Equip";
    public const string Item_Unequip = "Item_Unequip";

	public const string Dungeon_Move = "Dungeon_Move";
	public const string Dungeon_Battle_Start = "Dungeon_Battle_Start";
	public const string Dungeon_Battle_Finish = "Dungeon_Battle_Finish";
	public const string Dungeon_Exit_Unlock = "Dungeon_Exit_Unlock";
	public const string Dungeon_Map_Reveal = "Dungeon_Map_Reveal";
	public const string Dungeon_Monster_Reveal = "Dungeon_Monster_Reveal";
	public const string Dungeon_Treasure_Reveal = "Dungeon_Treasure_Reveal";
	public const string MiniMap_Show = "MiniMap_Show";
	public const string MiniMap_Hide = "MiniMap_Hide";

	public const string TextBox_Open = "TextBox_Open";
	public const string TextBox_Close = "TextBox_Close";

	public const string Shop_Open = "Shop_Open";
	public const string Shop_Close = "Shop_Close";

	public const string Player_Add_Exp = "Player_Add_Exp";
	public const string Player_Stat_Change = "Player_Stat_Change";

	public const string Buff_Start = "Buff_Start";
	public const string Buff_End = "Buff_End";
	public const string Buff_Effect = "Buff_Effect";

	public const string Quest_Start = "Quest_Start";
	
	public const string Quest_Update = "Quest_Update";
	public const string Quest_Complete = "Quest_Complete";

	
}
