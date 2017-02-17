<?php

define("ITEM_GRADE_LOW",          0);
define("ITEM_GRADE_NORMAL",       1);
define("ITEM_GRADE_HIGH",         2);
define("ITEM_GRADE_MAGIC",        3);
define("ITEM_GRADE_RARE",         4);
define("ITEM_GRADE_LEGENDARY",    5);
define("ITEM_GRADE_MAX",          6);

$config = array();

$level_infos = array();
$level_infos[] = array(
	"level" => 1,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		//"MONSTER_SKELETON_002"
		"MONSTER_REAPER_001",
		"MONSTER_BLOODFANG_001"
	),
	"items" => array(
		"chance" => 1,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, 		"weight" => 200),
			array("grade"=>ITEM_GRADE_NORMAL, 	"weight" => 120),
			array("grade"=>ITEM_GRADE_HIGH , 	"weight" => 80),
			array("grade"=>ITEM_GRADE_MAGIC, 	"weight" => 60),
			array("grade"=>ITEM_GRADE_RARE, 	"weight" => 20),
			array("grade"=>ITEM_GRADE_LEGENDARY,"weight" => 1)
		)
	)
);

$level_infos[] = array(
	"level" => 2,
	"monsters" => array(
		"MONSTER_SKELETON_002",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.8,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, 		"weight" => 150),
			array("grade"=>ITEM_GRADE_NORMAL, 	"weight" => 120),
			array("grade"=>ITEM_GRADE_HIGH , 	"weight" => 80),
			array("grade"=>ITEM_GRADE_MAGIC, 	"weight" => 60),
			array("grade"=>ITEM_GRADE_RARE, 	"weight" => 20),
			array("grade"=>ITEM_GRADE_LEGENDARY,"weight" => 1)
		)
	)
);

$level_infos[] = array(
	"level" => 3,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001",
		"MONSTER_SUCCUBUS_001"
	),
	"items" => array(
		"chance" => 0.6,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, 		"weight" => 150),
			array("grade"=>ITEM_GRADE_NORMAL, 	"weight" => 120),
			array("grade"=>ITEM_GRADE_HIGH , 	"weight" => 80),
			array("grade"=>ITEM_GRADE_MAGIC, 	"weight" => 60),
			array("grade"=>ITEM_GRADE_RARE, 	"weight" => 20),
			array("grade"=>ITEM_GRADE_LEGENDARY,"weight" => 1)
		)
	)
);
$level_infos[] = array(
	"level" => 4,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001",
		"MONSTER_SUCCUBUS_002"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
/*
$level_infos[] = array(
	"level" => 5,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 6,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 7,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 8,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 9,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 10,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 11,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 12,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 13,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_info[] = array(
	"level" => 14,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade_weight" => array(
			array("grade"=>ITEM_GRADE_LOW, "weight" => 1),
			array("grade"=>ITEM_GRADE_NORMAL, "weight" => 1),
			array("grade"=>ITEM_GRADE_HIGH , "weight"=> 1),
			array("grade"=>ITEM_GRADE_MAGIC, "weight" => 1000),
			array("grade"=>ITEM_GRADE_RARE, "weight" => 1000),
			array("grade"=>ITEM_GRADE_LEGENDARY, "weight" => 1000)
		)
	)
);
$level_infos[] = array(
	"level" => 15,
	"monsters" => array(
		"MONSTER_SKELETON_001",
		"MONSTER_DAEMON_001"
	),
	"items" => array(
		"chance" => 0.5,
		"grade" => array(
			ITEM_GRADE_LOW => 1000,
			ITEM_GRADE_NORMAL => 1000,
			ITEM_GRADE_HIGH => 1000,
			ITEM_GRADE_MAGIC => 1000,
			ITEM_GRADE_RARE => 1000,
			ITEM_GRADE_LEGENDARY => 1000
		)
	)
);
*/
$config["level_infos"] = $level_infos;
echo json_encode($config);
?>
