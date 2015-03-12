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
			Node lowestF = null;
			foreach (Node node in openList) {
				if (lowestF == null) {
					lowestF = node;
				}
				else if (node.f < lowestF.f)
					lowestF = node;
			}

			openList.Remove(lowestF);
			closedList.Add(lowestF);

			if (lowestF.position == endPosition) {
				List<GridCoordinate> path = new List<GridCoordinate>();
				Node nextNode = lowestF;

				while (nextNode != null) {
					path.Insert(0, nextNode.position);
					nextNode = nextNode.parent;
				}

				return path;
			} 
			else
				FindAdjacents(lowestF, openList, closedList, endPosition);
		}

		return null;
	}

	static void FindAdjacents(Node node, List<Node> openList, List<Node> closedList, GridCoordinate endPosition) {
		// Add adjacent nodes to list
		for (int i = -1; i < 2; i++) {
			if (node.position.y + i >= GridManager.minY && node.position.y + i <= GridManager.maxY) {
				for (int j = -1; j < 2; j++) {
					if (node.position.x + j >= GridManager.minScreenX && node.position.x + i <= GridManager.maxScreenX && !(i == 0 && j == 0)) {
						GridCoordinate nextPosition = node.position + new GridCoordinate(j, i);
						
						Node newNode = new Node(node, nextPosition, endPosition);

						if (closedList.Contains(newNode) || OccupiedNode(nextPosition)) {}
						else if (openList.Contains(newNode)) {
							int index = openList.IndexOf(newNode);
							Node oldNode = openList[index];

							if (oldNode.g > newNode.g) {
								openList[index] = newNode;
							}
						}
						else {
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
				if (entity is Enemy || entity is Obstacle)
					return true;
			}
			return false;
		}
	}
}
