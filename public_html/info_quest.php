<?php

$quests = array();
$quests[] = array(
	"id" => "FirstBlood",
	"name" => "First Blood",
	"triggers" => array(
		array("type" => "LessCompleteQuestCount", "value" => "0")
	),
	"progresses" => array(
		array("type" => "KillMonster", "key" => "", "goal" => 1),
		array("type" => "CurrentLocation", "key" => "Village", "goal" => 1)
	),
	"start_dialouge" => array(
		"speaker" => "VillageChief",
		"dialouge" => array(
			"Hello adventurer!! This is sample quest for you",
			"Kill any monster in the dungeon and come back to me"
		)
	),
	"complete_dialouge" => array(
		"speaker" => "VillageChief",
		"dialouge" => array(
			"You proved you deserve this reward"
		)
	),
	"reward" => array(
		"coin" => 100,
		"item" => "HealingPotion"
	)
);

$config = array();
$config["quests"] = $quests;
echo json_encode($config);
?>
