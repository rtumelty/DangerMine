using UnityEngine;
using System;
using System.Collections;

public class Node : IEquatable<Node> {
	
	public GridCoordinate position;
	public Node parent;
	public int f;
	public int g;
	public int h;

	public Node(Node newParent, GridCoordinate newPosition, GridCoordinate endPosition) {
		parent = newParent;
		position = newPosition;

		//Debug.Log(newParent);
		if (System.Object.ReferenceEquals(newParent, null)) g = 0;
		else {
			g = newParent.g;

			if ((newPosition.x - parent.position.x) != 0 && (newPosition.y - parent.position.y) != 0)
				g += AStar.diagonalCost;
			else
				g += AStar.cost;
		}
		
		h = CalculateHeuristic(newPosition, endPosition);
		f = g + h;
	}
	
	int CalculateHeuristic(GridCoordinate startPosition, GridCoordinate endPosition) {
		// Manhattan
		return (Mathf.Abs(endPosition.x - startPosition.x) + Mathf.Abs(endPosition.y - startPosition.y)) * AStar.cost;
	}
	
	public static bool operator ==(Node a, Node b) {
		if (System.Object.ReferenceEquals(a, b))
			return true;

		if (System.Object.ReferenceEquals(a, null) || System.Object.ReferenceEquals(b, null))
			return false;

		if (a.position == b.position)
			return true;
		
		return false;
	}
	
	public static bool operator !=(Node a, Node b) {
		return !(a == b);
	}
	
	public bool Equals(Node otherNode) {
		if (otherNode == null) return false;
		
		if (otherNode.position == this.position)
			return true;
		else return false;
	}
	
	public override bool Equals(System.Object other) {
		if (other == null) return false;
		
		Node otherNode = other as Node;
		if (otherNode == null) return false;

		return Equals(otherNode);
	}

	public override int GetHashCode ()
	{
		return this.position.GetHashCode ();
	}
}
