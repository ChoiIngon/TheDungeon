<?php

define("ITEM_GRADE_LOW",          0);
define("ITEM_GRADE_NORMAL",       1);
define("ITEM_GRADE_HIGH",         2);
define("ITEM_GRADE_MAGIC",        3);
define("ITEM_GRADE_RARE",         4);
define("ITEM_GRADE_LEGENDARY",    5);
define("ITEM_GRADE_MAX",          6);

define("ITEM_PART_HELMET",        0);
define("ITEM_PART_HAND",          1);
define("ITEM_PART_ARMOR",         2);
define("ITEM_PART_RING",          3);
define("ITEM_PART_SHOES",         4);
define("ITEM_PART_MAX",           5);

function EquipmentItemStat($type, $desc, $base, $rand)
{
	return array("type" => $type, "description" => $desc, "base_value" => $base, "random_value"=>$rand);
}

$info = array();

$grade_weight = array(
	array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
	array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
	array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
	array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
	array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
	array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
);
$info["grade_weight"] = $grade_weight;

$stats = array();
$stats[] = EquipmentItemStat("MAX_HEALTH", 	"max health", 	10, 	3);
$stats[] = EquipmentItemStat("ATTACK", 		"attack", 		5, 		2);
$stats[] = EquipmentItemStat("DEFENSE",		"defense", 		7, 		3);
$stats[] = EquipmentItemStat("SPEED", 		"speed", 		2, 		0.72);
$stats[] = EquipmentItemStat("CRITICAL", 	"% critical", 	3.5, 	0.3);
//$stats[] = EquipmentItemStat("STEALTH", 	"stealth", 		1, 		0.2);
//$stats[] = EquipmentItemStat("VIABILITY", 	"viability", 	1, 		0.13);
$stats[] = EquipmentItemStat("COIN_BONUS", 	"% bonus coin", 5.0, 	0.5);
$stats[] = EquipmentItemStat("EXP_BONUS",	"% bonus exp", 	10.0, 	0.5);
$info["item_stats"] = $stats;

$items = array();
$items[] = array(
        "id" => "ITEM_RING_OF_GREEDY",
        "name" => "Ring of Greedy",
        "price" => 10,
        "icon" => "item_ring_001",
        "part" => ITEM_PART_RING,
        "description" => "You will gather more coins if you wear this ring",
        "main_stat" => EquipmentItemStat("COIN_BONUS", "% bonus coin", 3, 0.78)
);

$items[] = array(
        "id" => "ITEM_RING_OF_HEALTH",
        "name" => "Ring of Health",
        "price" => 10,
        "icon" => "item_ring_002",
        "part" => ITEM_PART_RING,
        "description" => "If you wear this ring, maximum health will be increased.",
        "main_stat" => EquipmentItemStat( "MAX_HEALTH", "health", 10, 4)
);

$items[] = array(
        "id" => "ITEM_DAGGER",
        "name" => "Dagger",
        "price" => 10,
        "icon" => "item_sword_001",
        "part" => ITEM_PART_HAND,
        "description" => "A simple iron dagger with a well worn wooden handle",
        "main_stat" => EquipmentItemStat( "ATTACK", "attack", 10, 3)
);

$items[] = array(
        "id" => "ITEM_SHORT_SWORD",
        "name" => "Short Sword",
        "price" => 10,
        "icon" => "item_sword_002",
        "part" => ITEM_PART_HAND,
        "description" => "It is indeed quit short, just a few inches longer than dagger",
        "main_stat" => EquipmentItemStat( "ATTACK", "attack", 10, 3)
);

$items[] = array(
        "id" => "ITEM_QUARTER_STAFF",
        "name" => "Quarter Staff",
        "price" => 10,
        "icon" => "item_sword_003",
        "part" => ITEM_PART_HAND,
        "description" => "A staff of hardwood, its ends ar shod with iron.",
        "main_stat" => EquipmentItemStat( "ATTACK", "attack", 10, 3)
);

$items[] = array(
        "id" => "ITEM_MACE",
        "name" => "Mace",
        "price" => 10,
        "icon" => "item_sword_001",
        "part" => ITEM_PART_HAND,
        "description" => "The iron head of this weapon inflicts substantial damage",
        "main_stat" => EquipmentItemStat( "ATTACK", "attack", 10, 3)
);

$items[] = array(
        "id" => "ITEM_CLOTH_ARMOR",
        "name" => "Cloth armor",
        "price" => 10,
        "icon" => "item_armor_001",
        "part" => ITEM_PART_ARMOR,
        "description" => "This lightweight armor offers basic protection.",
        "main_stat" => EquipmentItemStat( "DEFENSE", "defense", 1, 0.82)
);

$items[] = array(
        "id" => "ITEM_LEATHER_ARMOR",
        "name" => "Leather armor",
        "price" => 10,
        "icon" => "item_armor_002",
        "part" => ITEM_PART_ARMOR,
        "description" => "Armor made from tanned monster hide.",
        "main_stat" => EquipmentItemStat( "DEFENSE", "defense", 4, 1.41)
);

$items[] = array(
        "id" => "ITEM_MAIL_ARMOR",
        "name" => "Mail armor",
        "price" => 10,
        "icon" => "item_armor_002",
        "part" => ITEM_PART_ARMOR,
        "description" => "Interlocking metal links for a tough but flexible shui of armor.",
        "main_stat" => EquipmentItemStat( "DEFENSE", "defense", 6, 2.87)
);
$info["items"] = $items;
echo json_encode($info);
?>
