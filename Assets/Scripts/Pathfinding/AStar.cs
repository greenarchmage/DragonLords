using System;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
  public class AStar
  {
    public static int ManhattanDistance(int posx1, int posy1, int posx2, int posy2)
    {
      return Math.Abs(posx1 - posx2) + Math.Abs(posy1 - posy2);
    }
    
    public static bool exists(int x, int y, TerrainTile[,] array)
    {
      return x >= 0 && y >= 0 && x < array.GetLength(0) && y < array.GetLength(1);
    }

    public static List<PathNode> ShortestPath(TerrainTile[,] terrainLayout, bool[,] obstructed, int startX, int startY, int goalX, int goalY)
    {
      int[,] distanceMatrix = new int[terrainLayout.GetLength(0), terrainLayout.GetLength(1)];
      for (int i = 0; i < distanceMatrix.GetLength(0); i++)
      {
        for (int j = 0; j < distanceMatrix.GetLength(1); j++)
        {
          distanceMatrix[i, j] = -1;
        }
      }
      distanceMatrix[startX, startY] = 0;

      // Add start node
      PriorityQueueMin<Node> openSet = new PriorityQueueMin<Node>();
      Node initial = new Node(startX, startY, 0, ManhattanDistance(startX, startY, goalX, goalY),terrainLayout[startX,startY].MoveGroup,0);
      initial.MoveCost = 10;
      openSet.Insert(initial);

      while (!openSet.IsEmpty())
      {
        Node current = openSet.DelMin();
        if (current.x == goalX && current.y == goalY)
        {
          List<PathNode> path = new List<PathNode>();
          while (current.Parent != null)
          {
            path.Add(new PathNode(current.MoveCost,new int[] { current.x, current.y }));
            current = current.Parent;
          }
          path.Reverse();
          return path;
        }
        // search all the neighbours
        List<Node> neighbours = current.generateNeighbours(terrainLayout, obstructed, distanceMatrix, goalX, goalY);
        openSet.insertRange(neighbours);
      }
      // we failed to find the goal
      return null;
    }
  }

  #region Node

  public class Node : IComparable<Node>
  {
    public TerrainTile.MoveType Type { get; set; }
    public Node Parent { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int costToGetHere { get; set; }
    public int estimatedCostToGoal { get; set; }
    public int MoveCost { get; set; }
    public Node(int x, int y, int costToGetHere, int estimatedCostToGoal, TerrainTile.MoveType type, int moveCost)
    {
      this.x = x;
      this.y = y;
      this.costToGetHere = costToGetHere;
      this.estimatedCostToGoal = estimatedCostToGoal;
      this.Type = type;
      this.MoveCost = moveCost;
    }

    public int total()
    {
      return costToGetHere + estimatedCostToGoal;
    }
    public int CompareTo(Node node)
    {
      Node other = node;
      if (this.total() == other.total())
      {
        return 0;
      }
      else return (this.total() < other.total() ? -1 : 1);
    }

    public List<Node> generateNeighbours(TerrainTile[,] terrainLayout,bool[,] obstructed, int[,] distanceMatrix, int goalX, int goalY)
    {
      List<Node> list = new List<Node>();
      createAndAdd(x + 1, y, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x + 1, y + 1, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x + 1, y - 1, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x - 1, y, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x - 1, y - 1, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x - 1, y + 1, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x, y + 1, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);
      createAndAdd(x, y - 1, goalX, goalY, terrainLayout, obstructed, distanceMatrix, list);

      return list;
    }

    private void createAndAdd(int newX, int newY, int goalX, int goalY, TerrainTile[,] terrainLayout,
                                bool[,] obstructed, int[,] distanceMatrix, List<Node> list)
    {
      if (AStar.exists(newX, newY, terrainLayout))
      {
        bool passableTerrain = false;
        int newCost = this.costToGetHere + terrainLayout[newX, newY].Movecost;
        int moveCost = terrainLayout[newX, newY].Movecost;
        if( Type == TerrainTile.MoveType.Impassable || terrainLayout[newX, newY].MoveGroup == TerrainTile.MoveType.Impassable ||
          obstructed[newX,newY])
        {
          passableTerrain = false;
        } else
        {
          switch (terrainLayout[newX, newY].MoveGroup)
          {
            case TerrainTile.MoveType.Flying:
              // TODO check stack for passable, if stack has flying
              passableTerrain = false;
              break;
            case TerrainTile.MoveType.Normal:
              if (Type == TerrainTile.MoveType.Normal || Type == TerrainTile.MoveType.Transition)
              {
                passableTerrain = true;
              }
              else
              {
                passableTerrain = false;
              }
              break;
            case TerrainTile.MoveType.Transition:
              if (Type == TerrainTile.MoveType.Transition || Type == TerrainTile.MoveType.WaterPassable || Type == TerrainTile.MoveType.Normal)
              {
                passableTerrain = true;
              }
              else
              {
                passableTerrain = false;
              }
              break;
            case TerrainTile.MoveType.WaterPassable:
              if (Type == TerrainTile.MoveType.Transition || Type == TerrainTile.MoveType.WaterPassable)
              {
                passableTerrain = true;
              }
              else
              {
                passableTerrain = false;
              }
              break;
          }
        }
        
        if (passableTerrain)
        {
          int newEstimate = AStar.ManhattanDistance(newX, newY, goalX, goalY);
          Node newNode = new Node(newX, newY, newCost, newEstimate,terrainLayout[newX,newY].MoveGroup,moveCost);
          newNode.Parent = this;
          if (distanceMatrix[newX, newY] < 0 || newCost < distanceMatrix[newX, newY])
          {
            list.Add(newNode);
            distanceMatrix[newX, newY] = newCost;
          }
        }
      }
    }
  }
  #endregion
}
