﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct TilePosition {

	public int x;
	public int y;

	public TilePosition(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override bool Equals(object obj) {
		if (!(obj is TilePosition))
			return false;

		TilePosition tp = (TilePosition)obj;
		return tp.x == this.x && tp.y == this.y;
	}

	public override int GetHashCode() {
		var hashCode = 1861411795;
		hashCode = hashCode * -1521134295 + x.GetHashCode();
		hashCode = hashCode * -1521134295 + y.GetHashCode();
		return hashCode;
	}

	public static bool operator ==(TilePosition obj1, TilePosition obj2) {
		return obj1.x == obj2.x && obj1.y == obj2.y;
	}

	public static bool operator !=(TilePosition obj1, TilePosition obj2) {
		return !(obj1 == obj2);
	}
}

public class Direction {
	public static Direction None { get => new Direction(Vector2.zero); }
	public static Direction Right { get => new Direction(Vector2.right); }
	public static Direction Left { get => new Direction(Vector2.left); }
	public static Direction Down { get => new Direction(Vector2.down); }
	public static Direction Up { get => new Direction(Vector2.up); }

	public Vector2 Value { get; }
	private Direction(Vector2 v) {
		Value = v;
	}

	public static implicit operator bool(Direction d) {
		return d.Value.sqrMagnitude > 0.1f;
	}
}

public static class DirectionHelpers {
	
}