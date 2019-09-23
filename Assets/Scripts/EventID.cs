using UnityEngine;
using System.Collections;

public class EventID 
{
    public const string CoinAmountChanged = "CoinAmountChanged";
	public const string Inventory_Add = "Inventory_Add";
	public const string Inventory_Remove = "Inventory_Remove";
    public const string Inventory_Slot_Select = "Inventory_Slot_Select";
	public const string Inventory_Slot_Release = "Inventory_Slot_Release";
	public const string Inventory_Open = "Inventory_Open";
	public const string Inventory_Close = "Inventory_Close";

    public const string Item_Equip = "Item_Equip";
    public const string Item_Unequip = "Item_Unequip";

	public const string Dungeon_Move_Start = "Dungeon_Move_Start";
	public const string Dungeon_Move_Finish = "Dungeon_Move_Finish";
	public const string Dungeon_Battle_Start = "Dungeon_Battle_Start";
	public const string Dungeon_Battle_Finish = "Dungeon_Battle_Finish";

	public const string Dialog_Open = "Dialog_Open";
	public const string Dialog_Close = "Dialog_Close";
	public const string TextBox_Open = "TextBox_Open";
	public const string TextBox_Close = "TextBox_Close";
	public const string Player_Add_Exp = "Player_Add_Exp";
	public const string Player_Change_Health = "Player_Change_Health";
}
