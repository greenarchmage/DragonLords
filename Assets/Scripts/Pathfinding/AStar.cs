using System;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
  public class AStar
  {
    /// <summary>
    /// Method using only passable array
    /// obsolete
    /// </summary>
    /// <param name="passable">bool array representing passable positions</param>
    /// <param name="startX">starting x position</param>
    /// <param name="startY">starting y position</param>
    /// <param name="goalX">goal x position</param>
    /// <param name="goalY">goal y position</param>
    /// <returns></returns>
    public static List<int[]> ShortestPath(bool[,] passable, int startX, int startY, int goalX, int goalY)
    {
      int[,] distanceMatrix = new int[passable.GetLength(0), passable.GetLength(1)];
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
      Node initial = new Node(startX, startY, 0, ManhattanDistance(startX, startY, goalX, goalY));
      openSet.insert(initial);

      Node Parent = null;
      while (!openSet.IsEmpty())
      {
        Node current = openSet.DelMin();
        current.Parent = Parent;
        Parent = current;
        if (current.x == goalX && current.y == goalY)
        {
          // http://www.policyalmanac.org/games/aStarTutorial.htm
          for (int i = 0; i < distanceMatrix.GetLength(0); i++)
          {
            string line = "";
            for(int j = 0; j<distanceMatrix.GetLength(1); j++)
            {
              line += distanceMatrix[i, j] + ", ";
            }
          }
          List<int[]> path = new List<int[]>();
          while(current.Parent != null)
          {
            path.Add(new int[] {current.x ,current.y });
            current = current.Parent;
          }
          path.Reverse();
          return path;
        }
        // search all the neighbours
        List<Node> neighbours = current.generateNeighbours(passable, distanceMatrix, goalX, goalY);
        openSet.insertRange(neighbours);
      }
      // we failed to find the goal
      return null;
    }

    public static int distance(bool[,] passable, int startX, int startY, int goalX, int goalY)
    {
      int[,] distanceMatrix = new int[passable.GetLength(0), passable.GetLength(1)];
      for (int i = 0; i < distanceMatrix.GetLength(0); i++)
      {
        for (int j = 0; j < distanceMatrix.GetLength(1); j++)
        {
          distanceMatrix[i, j] = -1;
        }
      }
      distanceMatrix[startX, startY] = 0;
      PriorityQueueMin<Node> openSet = new PriorityQueueMin<Node>();
      Node initial = new Node(startX, startY, 0, ManhattanDistance(startX, startY, goalX, goalY));
      openSet.insert(initial);
      while (true)
      {
        if (openSet.IsEmpty())
        {
          // we failed to find the goal
          return -1;
        }
        Node current = openSet.DelMin();
        if (current.x == goalX && current.y == goalY)
        {
          // we found it!
          return current.costToGetHere;
        }
        // search all the neighbours
        List<Node> neighbours = current.generateNeighbours(passable, distanceMatrix, goalX, goalY);
        openSet.insertRange(neighbours);
      }
    }

    public static int ManhattanDistance(int posx1, int posy1, int posx2, int posy2)
    {
      return Math.Abs(posx1 - posx2) + Math.Abs(posy1 - posy2);
    }

    public static bool exists(int x, int y, bool[,] passable)
    {
      return x >= 0 && y >= 0 && x < passable.GetLength(0) && y < passable.GetLength(1);
    }

    public static bool exists(int x, int y, TerrainTile.TerrainType[,] array)
    {
      return x >= 0 && y >= 0 && x < array.GetLength(0) && y < array.GetLength(1);
    }

    public static int testMethod()
    {
      bool[,] testmap = {
                {true , true , false, true , true , true },
                {false, true , false, true , true , false},
                {true , true , false, false, true , true },
                {false, true , true , true , true , false},
                {false, false, false, false, true , false},
                {true , true , true , true , true , true },

        };
      return distance(testmap, 0, 0, 5, 5);
    }

    public static List<int[]> ShortestPath(TerrainTile.TerrainType[,] terrainLayout, int startX, int startY, int goalX, int goalY)
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
      Node initial = new Node(startX, startY, 0, ManhattanDistance(startX, startY, goalX, goalY));
      openSet.insert(initial);

      //Node Parent = null;
      while (!openSet.IsEmpty())
      {
        Node current = openSet.DelMin();
        //current.Parent = Parent;
        //Parent = current;
        if (current.x == goalX && current.y == goalY)
        {
          List<int[]> path = new List<int[]>();
          while (current.Parent != null)
          {
            path.Add(new int[] { current.x, current.y });
            current = current.Parent;
          }
          path.Reverse();
          return path;
        }
        // search all the neighbours
        List<Node> neighbours = current.generateNeighbours(terrainLayout, distanceMatrix, goalX, goalY);
        openSet.insertRange(neighbours);
      }
      // we failed to find the goal
      return null;
    }
  }

  #region Node

  public class Node : IComparable<Node>
  {
    public Node Parent { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int costToGetHere { get; set; }
    public int estimatedCostToGoal { get; set; }

    public Node(int x, int y, int costToGetHere, int estimatedCostToGoal)
    {
      this.x = x;
      this.y = y;
      this.costToGetHere = costToGetHere;
      this.estimatedCostToGoal = estimatedCostToGoal;
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

    public List<Node> generateNeighbours(bool[,] passable, int[,] distanceMatrix, int goalX, int goalY)
    {
      List<Node> list = new List<Node>();
      createAndAdd(x + 1, y, goalX, goalY, passable, distanceMatrix, list);
      createAndAdd(x + 1, y+1, goalX, goalY, passable, distanceMatrix, list);
      createAndAdd(x - 1, y, goalX, goalY, passable, distanceMatrix, list);
      createAndAdd(x - 1, y -1, goalX, goalY, passable, distanceMatrix, list);
      createAndAdd(x, y + 1, goalX, goalY, passable, distanceMatrix, list);
      createAndAdd(x, y - 1, goalX, goalY, passable, distanceMatrix, list);
      return list;
    }

    public List<Node> generateNeighbours(TerrainTile.TerrainType[,] terrainLayout, int[,] distanceMatrix, int goalX, int goalY)
    {
      List<Node> list = new List<Node>();
      createAndAdd(x + 1, y, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x + 1, y + 1, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x + 1, y - 1, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x - 1, y, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x - 1, y - 1, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x - 1, y + 1, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x, y + 1, goalX, goalY, terrainLayout, distanceMatrix, list);
      createAndAdd(x, y - 1, goalX, goalY, terrainLayout, distanceMatrix, list);

      return list;
    }

    private void createAndAdd(int newX, int newY, int goalX, int goalY, bool[,] passable,
                                int[,] distanceMatrix, List<Node> list)
    {
      if (AStar.exists(newX, newY, passable) && passable[newX, newY])
      {
        int newCost = this.costToGetHere + 1;
        int newEstimate = AStar.ManhattanDistance(newX, newY, goalX, goalY);
        Node newNode = new Node(newX, newY, newCost, newEstimate);
        if (distanceMatrix[newX, newY] < 0 || newCost < distanceMatrix[newX, newY])
        {
          // if (newX == goalX && newY == goalY)
          //     System.out.println("adding goal " + newX + " " + newY + " " + newCost);
          list.Add(newNode);
          distanceMatrix[newX, newY] = newCost;
        }
      }
    }

    private void createAndAdd(int newX, int newY, int goalX, int goalY, TerrainTile.TerrainType[,] terrainLayout,
                                int[,] distanceMatrix, List<Node> list)
    {
      if (AStar.exists(newX, newY, terrainLayout))
      {
        bool passableTerrain = false;
        int newCost = this.costToGetHere;
        switch (terrainLayout[newX, newY])
        {
          // TODO move terrain cost to a fixed file or class
          case TerrainTile.TerrainType.Bridge:
            newCost += 1;
            passableTerrain = true;
            break;
          case TerrainTile.TerrainType.Castle:
            newCost += 1;
            passableTerrain = false;
            break;
          case TerrainTile.TerrainType.Forest:
            newCost += 2;
            passableTerrain = false;
            break;
          case TerrainTile.TerrainType.Grass:
            newCost += 2;
            passableTerrain = true;
            break;
          case TerrainTile.TerrainType.Mountain:
            newCost += 2;
            passableTerrain = false;
            break;
          case TerrainTile.TerrainType.Road:
            newCost += 1;
            passableTerrain = true;
            break;
          case TerrainTile.TerrainType.Water:
            newCost += 1;
            passableTerrain = false;
            break;
        }
        if (passableTerrain)
        {
          int newEstimate = AStar.ManhattanDistance(newX, newY, goalX, goalY);
          Node newNode = new Node(newX, newY, newCost, newEstimate);
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
