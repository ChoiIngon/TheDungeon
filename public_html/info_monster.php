<?php
$config = array();

$monsters = array();
$monsters[] = array(
	"id" => "MONSTER_BLOODFANG_001",
	"name" => "Blood fang",
	"level" => 1,
	"health" => 400,
	"attack" => 200,
	"defense" => 0,
	"speed" => 100,
	"sprite_path" => "MS_Monster_Bloodfang",
	"reward" => array(
		"coin" => 100,
		"exp" => 100 
	)
);
$monsters[] = array(
	"id" => "MONSTER_REAPER_001",
	"name" => "Reaper specter",
	"level" => 1,
	"health" => 400,
	"attack" => 200,
	"defense" => 0,
	"speed" => 100,
	"sprite_path" => "MS_Monster_Reaper_Specter",
	"reward" => array(
		"coin" => 100,
		"exp" => 100 
	)
);
$monsters[] = array(
	"id" => "MONSTER_SKELETON_001",
	"name" => "Skeleton 1",
	"level" => 1,
	"health" => 400,
	"attack" => 200,
	"defense" => 0,
	"speed" => 100,
	"sprite_path" => "monster_skeleton",
	"reward" => array(
		"coin" => 100,
		"exp" => 100 
	)
);
$monsters[] = array(
	"id" => "MONSTER_SKELETON_002",
	"name" => "Skeleton 2",
	"level" => 2,
	"health" => 850,
	"attack" => 220,
	"defense" => 0,
	"speed" => 100,
	"sprite_path" => "monster_skeleton",
	"reward" => array(
		"coin" => 120,
		"exp" => 130 
	)
);
$monsters[] = array(
	"id" => "MONSTER_DAEMON_001",
	"name" => "Daemon",
	"level" => 1,
	"health" => 900,
	"attack" => 200,
	"defense" => 10,
	"speed" => 100,
	"sprite_path" => "monster_daemon",
	"reward" => array(
		"coin" => 150,
		"exp" => 150 
	)
);
$monsters[] = array(
	"id" => "MONSTER_SUCCUBUS_001",
	"name" => "Succubus",
	"level" => 1,
	"health" => 800,
	"attack" => 180,
	"defense" => 5,
	"speed" => 160,
	"sprite_path" => "monster_succubus",
	"reward" => array(
		"coin" => 180,
		"exp" => 160 
	)
);
$monsters[] = array(
	"id" => "MONSTER_SUCCUBUS_002",
	"name" => "Succubus",
	"level" => 1,
	"health" => 1000,
	"attack" => 200,
	"defense" => 5,
	"speed" => 200,
	"sprite_path" => "monster_succubus",
	"reward" => array(
		"coin" => 220,
		"exp" => 170 
	)
);

$config["monsters"] = $monsters;
echo json_encode($config);
?>
