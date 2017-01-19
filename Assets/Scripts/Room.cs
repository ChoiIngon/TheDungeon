using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Room
{
	public const int North = 0;
	public const int East = 1;
	public const int South = 2;
	public const int West = 3;
	public const int Max = 4;

	public Room() {
		id = 0;
		visit = false;
		sprite = null;
		for (int direction = 0; direction < Max; direction++) {
			next [direction] = null;
		}
	}
	public int id;
	public int group;
	public bool visit;
	public Sprite sprite;
	public Room[] next = new Room[Max];

	public Room Connect(int direction)
	{
		Room other = GetNeighbor (direction);
		if (null == other) {
			return null;
		}
		next [direction] = other;

		switch (direction) {
		case North:
			other.next [South] = this;
			break;
		case East:
			other.next [West] = this;
			break;
		case South:
			other.next [North] = this;
			break;
		case West:
			other.next [East] = this;
			break;
		}
		return other;
	}
	private Room GetNeighbor(int direction)
	{
		if (0 > direction || Max <= direction) {
			throw new System.Exception ("room id:" + direction + ", invalid direction:" + direction);
		}
		switch (direction) {
		case North:
			if (0 < id - Map.WIDTH) {
				return Map.Instance.rooms [id - Map.WIDTH];
			}
			break;
		case East:
			if ((id + 1) % Map.WIDTH != 0) {
				return Map.Instance.rooms [id + 1];
			}
			break;
		case South:
			if (id + Map.WIDTH < Map.WIDTH * Map.HEIGHT) {
				return Map.Instance.rooms [id + Map.WIDTH];
			}
			break;
		case West:
			if ((id % Map.WIDTH) - 1 >= 0) {
				return Map.Instance.rooms [id - 1];
			}
			break;
		}
		return null;
	}
	public Room GetNext(int direction)
	{
		if (0 > direction || Max <= direction) {
			throw new System.Exception ("room id:" + direction + ", invalid direction:" + direction);
		}
		return next [direction];
	}
};