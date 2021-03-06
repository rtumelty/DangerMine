﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class GridCoordinate {
	static float xScale = 1;
	static float yScale = 1f;//.15f;
	static float xOffset = .25f;

	public static float XScale {
		get {
			return xScale;
		}
	}

	public static float YScale {
		get {
			return yScale;
		}
	}

	public int x;
	public int y;

	public GridCoordinate(int xCoord, int yCoord) {
		x = xCoord;
		y = yCoord;
	}
	
	public GridCoordinate(float xCoord, float yCoord) {
		x = Mathf.RoundToInt (xCoord);
		y = Mathf.RoundToInt (yCoord);
	}
	
	public GridCoordinate(Vector2 coords) {
		y = Mathf.RoundToInt (coords.y / yScale);
		x = Mathf.RoundToInt ((coords.x - (xOffset * y))/ xScale);
	}
	
	public GridCoordinate(Vector3 coords) {
		y = Mathf.RoundToInt (coords.y / yScale);
		x = Mathf.RoundToInt ((coords.x - (xOffset * y))/ xScale);
	}
	
	public static GridCoordinate operator +(GridCoordinate a, GridCoordinate b) {
		return new GridCoordinate (a.x + b.x, a.y + b.y);
	}
	
	public static GridCoordinate operator -(GridCoordinate a, GridCoordinate b) {
		return new GridCoordinate (a.x - b.x, a.y - b.y);
	}
	
	public static bool operator ==(GridCoordinate a, GridCoordinate b) {
		
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}
		
		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}
		
		// Return true if the fields match:
		return a.x == b.x && a.y == b.y;
	}
	
	public static bool operator !=(GridCoordinate a, GridCoordinate b) {
		return !(a == b);
	}

	public override bool Equals(System.Object obj) {
		if (obj == null)
			return false;

		GridCoordinate coord = obj as GridCoordinate;
		if (coord == null)
			return false;

		return this == coord;
	}
	
	public override int GetHashCode()
	{
		return x ^ y;
	}
	
	public static implicit operator GridCoordinate(Vector2 vector)  
	{return new GridCoordinate(vector);}
	
	public static implicit operator Vector2(GridCoordinate coord)  
	{return new Vector2(coord.x * xScale, coord.y * yScale);}
	
	public static implicit operator GridCoordinate(Vector3 vector)  
	{return new GridCoordinate(vector);}

	public Vector3 ToVector3(float z = 0) {
		return new Vector3(x * xScale + xOffset * y, y * yScale, z);
	}

	public override string ToString ()
	{
		return "(" + x + ", " + y + ")";
	}
}
