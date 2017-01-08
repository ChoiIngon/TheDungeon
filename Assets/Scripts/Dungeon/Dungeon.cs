using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Dungeon : MonoBehaviour {
	/*
	private static Dungeon _instance;  
	public static Dungeon Instance  
	{  
		get  
		{  
			if (!_instance)  
			{  
				_instance = (Dungeon)GameObject.FindObjectOfType(typeof(Dungeon));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "Dungeon";  
					_instance = container.AddComponent<Dungeon>();  
				}  
			}  

			return _instance;  
		}  
	}  
	public int roomCount;
	public int width;
	public int height;
	public Tile tilePrefab;
	public Tile[] tiles;
	public List<Room> rooms;
	public Player player;
	public Sprite[] sprites;
	void Start()
	{
		tiles = new Tile[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				Tile tile = GameObject.Instantiate<Tile> (tilePrefab);
				tile.type = Tile.Type.Floor;
				tile.transform.SetParent (transform);
				tile.transform.localPosition = new Vector3 (Tile.Size * (float)x, Tile.Size * (float)y, 0.0f);
				tile.index = y * width + x;
				tiles [tile.index] = tile;
			}
		}

		TheDungeon.TouchPad touch = GetComponent<TheDungeon.TouchPad> ();
		touch.size = new Vector2 (Tile.Size * (float)width, Tile.Size * (float)height);
		//touch.offset = new Vector2 (Tile.Size * (float)width * 0.5f - Tile.Size * 0.5f, Tile.Size * (float)height * 0.5f - Tile.Size * 0.5f);
		touch.onTouchDown += ((Vector3 position) => {
			Vector3 index = (position - transform.position)/Tile.Size + new Vector3(Tile.Size * 0.5f, Tile.Size * 0.5f, 0.0f);
			int x = (int)index.x;
			int y = (int)index.y;
			Tile tile = tiles[y * width + x];
			player.transform.position = tile.transform.position;
			Camera.main.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, Camera.main.transform.position.z);
		});

		transform.position = new Vector3 (Tile.Size * (float)width * -0.5f, Tile.Size * (float)height * -0.5f, 0.0f);

		touch.onTouchDrag += ((Vector3 delta) => {
			//transform.position += delta;
			//player.transform.position += delta;
		});
		touch.onTouchUp += () => {
		};

		rooms = new List<Room> ();
		for (int i = 0; i < roomCount; i++) {
			Rect rect = new Rect (width/2 - Room.MAX_SIZE/2, height/2 - Room.MAX_SIZE/2, Random.Range (Room.MIN_SIZE, Room.MAX_SIZE), Random.Range (Room.MIN_SIZE, Room.MAX_SIZE));
			rect.center = new Vector2 (rect.center.x + Random.Range (-2, 2), rect.center.y + Random.Range (-2, 2));
			Room room = new Room (rect);
			room.id = i + 1;
			foreach (Room prev in rooms) {
				if (true == prev.rect.Overlaps (room.rect)) {
					if (prev.rect.center.x < room.rect.center.x) {
						float move = prev.rect.xMax - room.rect.xMin;
						room.rect.xMin += move;
						room.rect.xMax += move;
					} else {
						float move = room.rect.xMin - prev.rect.xMax;
						room.rect.xMin += move;
						room.rect.xMax += move;
					}
				}
			}
			rooms.Add (room);
		}
		foreach (Room room in rooms) {
			room.Dig ();
		}
	}
	/*
	private int[] tiles_;
	private int groupID;
	private int roomCount;
	private List<Room> rooms;
	private JSONNode root;

	public Dungeon(JSONNode root){
		this.root = root;
		this.groupID = 1;
		Room.MIN_SIZE = root ["room"] ["min"].AsInt;
		Room.MAX_SIZE = root ["room"] ["max"].AsInt;
		int w = root ["size"] ["width"].AsInt;
		int h = root ["size"] ["height"].AsInt;
		this.width = (0 == w % 2 ? w + 1 : w);
		this.height = (0 == h % 2 ? h + 1 : h);
		this.roomCount = root ["room"] ["count"].AsInt;
		this.rooms = new List<Room> ();
		this.tiles_ = new int[this.width * this.height];
		for (int i = 0; i < tiles_.Length; i++)
		{
			tiles_[i] = 0;
		}
		this.tiles = new Tile[this.width * this.height];
	}

	public void Init(Rect size, int roomCount
	public override void Generate()
	{
		GenerateTiles ();
		GenerateMonsters ();
		GenerateGateways ();
		GenerateNPC ();
	}

	private void GenerateNPC()
	{
		JSONNode json = root ["npc"];
		for (int i=0; i<json.Count; i++) {
			Npc npc = new Npc();
			npc.id = json[i]["id"];
			npc.SetPosition(rooms[json[i]["room"].AsInt].GetRandomPosition());
		}
	}

	private void GenerateTiles()
	{
		for (int i = 0; i< this.tiles.Length; i++) {
			Tile tile = new Tile();
			tile.SetPosition(new Position(i % this.width, i / this.width));
			tiles[i] = tile;
		}

		for(int i=0; i<1000 && rooms.Count <= this.roomCount; i++)
		{
			int x = Random.Range(1, this.width - Room.MAX_SIZE - 1);
			int w = Random.Range(Room.MIN_SIZE, Room.MAX_SIZE);
			int y = Random.Range(1, this.height - Room.MAX_SIZE - 1);
			int h = Random.Range(Room.MIN_SIZE, Room.MAX_SIZE);
			Room room = new Room (this, x, x + w, y, y + h);
			room.Digging ();
		}

		for (int y=1; y<height-1; y++) {
			for (int x=1; x<width-1; x++) {
				if(1 == x%2 && 1 == y%2 && 0 == GetTileGroupID(x, y)) {
					Maze maze = new Maze (this);
					maze.Digging (Maze.DirectionType.West, x, y);
				}
			}
		}

		foreach (Room room in rooms) {
			room.Connect();
		}

		bool delete = true;
		while(delete) {
			delete = false;
			for (int y=1; y<height-1; y++) {
				for (int x=1; x<width-1; x++) {
					if(0 != GetTileGroupID(x, y))
					{
						int count = 0;
						for (int dy = -1; dy <= 1; dy += 2)
						{
							if (0 == GetTileGroupID(x, y + dy))
							{
								count++;
							}
						}
						for (int dx = -1; dx <= 1; dx += 2)
						{
							if (0 == GetTileGroupID(x + dx, y))
							{
								count++;
							}   
						}

						if(3 <= count)
						{
							tiles_[x + y * width] = 0;
							delete = true;
						}
					}
				}
			}
		}

		for (int y=0; y<height; y++) {
			for (int x=0; x<width; x++) {
				if(0 == this.GetTileGroupID(x, y))
				{
					int count = 0;
					count += GetTileGroupID(x-1, y-1);
					count += GetTileGroupID(x, y-1);
					count += GetTileGroupID(x+1, y-1);
					count += GetTileGroupID(x+1, y);
					count += GetTileGroupID(x+1, y+1);
					count += GetTileGroupID(x, y+1);
					count += GetTileGroupID(x-1, y+1);
					count += GetTileGroupID(x-1, y);

					if(0 != count)
					{
						Wall wall = new Wall();
						wall.SetPosition(new Position(x, y));
					}
				}
			}
		}

		start = rooms [root ["start_room"].AsInt].GetRandomPosition ();
	}

	public void GenerateMonsters()
	{
		spwanSpots = new List<MonsterSpawnSpot> ();
		JSONNode json = root ["monster"];
		for (int i=0; i<json.Count; i++) {
			MonsterSpawnSpot spot = new MonsterSpawnSpot();
			spot.id = json[i]["id"];
			spot.count = json[i]["count"].AsInt;
			spot.interval = json[i]["interval"].AsInt;
			Util.RangeInt roomID = new Util.RangeInt(json[i]["room"]);
			spot.position = rooms[(int)roomID].GetRandomPosition();
			spwanSpots.Add (spot);
		}
	}

	public void GenerateGateways() {
		JSONNode gateways = root ["gateway"];
		for (int i=0; i<gateways.Count; i++) {
			Gateway gateway = new Gateway();
			gateway.dest.id = gateways[i]["id"];
			gateway.dest.name = gateways[i]["name"];
			gateway.dest.map = gateways[i]["map"];
			gateway.SetPosition(rooms[gateways[i]["room"].AsInt].GetRandomPosition());
		}
	}

	public override void Update()
	{
		foreach (MonsterSpawnSpot spot in spwanSpots)
		{
			spot.Update();
		}
	}

	public int GetTileGroupID(int x, int y)
	{
		if (0 > x || x >= width || 0 > y || y >= height)
		{
			return 0;
		}
		return tiles_[x + y * width];
	}

	public class Maze
	{
		public enum DirectionType
		{
			North, East, South, West, Max
		}
		public int groupID;
		public Dungeon dungeon;

		public Maze(Dungeon dungeon)
		{
			this.dungeon = dungeon;
			this.groupID = dungeon.groupID++;
		}

		public void Digging(DirectionType direction, int x, int y)
		{
			if (1 > x || x >= dungeon.width-1 || 1 > y || y >= dungeon.height-1)
			{
				return;
			}

			int count = 0;
			for (int dy = -1; dy <= 1; dy += 2)
			{
				if (0 == dungeon.GetTileGroupID(x, y + dy))
				{
					count++;
				}
			}
			for (int dx = -1; dx <= 1; dx += 2)
			{
				if (0 == dungeon.GetTileGroupID(x + dx, y))
				{
					count++;
				}   
			}

			if(3 > count)
			{
				return;
			}
			dungeon.tiles_[x + y * dungeon.width] = groupID;

			List<DirectionType> directions = new List<DirectionType>();

			if (1 == y % 2) {
				directions.Add (DirectionType.East);
				directions.Add (DirectionType.West);
			}
			if (1 == x % 2) {
				directions.Add (DirectionType.North);
				directions.Add (DirectionType.South);
			}

			directions.Add (direction);
			if (0 == Random.Range (0, 2)) {
				direction = directions [Random.Range (0, directions.Count)];
			}

			while (0 < directions.Count)
			{
				directions.Remove(direction);
				switch (direction)
				{
				case DirectionType.North:
					Digging(direction, x, y - 1);
					break;
				case DirectionType.East:
					Digging(direction, x + 1, y);
					break;
				case DirectionType.South:
					Digging(direction, x, y + 1);
					break;
				case DirectionType.West:
					Digging(direction, x - 1, y);
					break;
				}
				if (0 == directions.Count)
				{
					return;
				}
				direction = directions[Random.Range (0, directions.Count)];
			}
		}
	}
		public class Connector {
		public enum DirectionType {
			Vertical, Horizon
		};
		public int x, y;
		public DirectionType direction;
	}
	*/
}