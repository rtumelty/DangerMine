using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar {
	public const int cost = 10;
	public const int diagonalCost = 14;
	const int iterationLimit = 3;

	/// <summary>
	/// Searches adjacent positions to find shortest path to end position. 
	/// Iterations limited to prevent infinite searches if no valid path.
	/// </summary>
	/// <returns>If successful, returns the shortest path as estimated by the heuristic algorithm. If no path is found, returns null.</returns>
	/// <param name="position">Origin position of the search.</param>
	/// <param name="endPosition">The target position of the search.</param>
	public static List<GridCoordinate> GetPath(GridCoordinate startPosition, GridCoordinate endPosition) {
		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();

		// Move start position to open list
		openList.Add(new Node(null, startPosition, endPosition));

		// Execute pathfinding algorithm on adjacent nodes
		while (openList.Count > 0) {
			Node lowestF = GetLowestFNode(openList);

			openList.Remove(lowestF);
			closedList.Add(lowestF);

			if (lowestF.position == endPosition) {
				return BuildPath(lowestF);
			} 
			else
				FindAdjacents(lowestF, openList, closedList, endPosition);
		}

		return BuildPath(FindNearestNode(closedList));
	}
	
	static Node GetLowestFNode(List<Node> list) {
		//Debug.Log("Finding lowest f node in " + list + ", size " + list.Count);
		Node lowestF = null;
		
		foreach (Node node in list) {
			if (lowestF == null) {
				lowestF = node;
			}
			else if (node.f < lowestF.f)
				lowestF = node;
		}
		
		return lowestF;
	}
	
	static Node FindNearestNode(List<Node> list) {
		//Debug.Log("Finding lowest f node in " + list + ", size " + list.Count);
		Node nearestNode = null;
		
		foreach (Node node in list) {
			if (nearestNode == null) {
				nearestNode = node;
			}
			else if (node.h < nearestNode.h)
				nearestNode = node;
		}
		
		return nearestNode;
	}

	static void FindAdjacents(Node node, List<Node> openList, List<Node> closedList, GridCoordinate endPosition) {
		//Debug.Log("Finding adjacents");
		// Add adjacent nodes to list
		for (int i = -1; i < 2; i++) {
			if (node.position.y + i >= GridManager.minY && node.position.y + i <= GridManager.maxY) {
				for (int j = -1; j < 2; j++) {
					if (node.position.x + j >= GridManager.minWorldX && node.position.x + i <= GridManager.maxWorldX && !(i == 0 && j == 0)) {
						GridCoordinate nextPosition = node.position + new GridCoordinate(j, i);
						
						Node newNode = new Node(node, nextPosition, endPosition);

						if (closedList.Contains(newNode) || OccupiedNode(nextPosition)) {}
						else if (openList.Contains(newNode)) {
							int index = openList.IndexOf(newNode);
							Node oldNode = openList[index];

							if (oldNode.g > newNode.g) {
								//Debug.Log("Replacing node at " + oldNode.position);
								openList[index] = newNode;
							}
						}
						else {
							//Debug.Log("Adding new node: " + newNode.position);
							openList.Add(newNode);
						}
					}
				}
			}
		}

	}

	static bool OccupiedNode(GridCoordinate coord) {
		if (!GridManager.Instance.IsOccupied(GridManager.Grid.WorldGrid, coord))
			return false;
		else {
			List<GameEntity> entities = GridManager.Instance.EntitiesAt(GridManager.Grid.WorldGrid, coord);

			foreach (GameEntity entity in entities) {
				if (entity is Enemy || entity is Obstacle || entity is Hole)
					return true;
			}
			return false;
		}
	}

	static List<GridCoordinate> BuildPath(Node node) {
		//Debug.Log("Building path...");
		List<GridCoordinate> path = new List<GridCoordinate>();
		Node nextNode = node;
		
		while (nextNode != null) {
			//Debug.Log("Adding node at " + nextNode.position);
			path.Insert(0, nextNode.position);

			nextNode = nextNode.parent;
			//if ( nextNode == null)
			//	Debug.Log("Reached path end, returning.");
		}
		
		return path;
	}
}
