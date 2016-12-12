using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Pathfinding
{
  public class PathNode
  {
    public int Cost { get; set; }
    public int[] Coord { get; set; }
    public PathNode(int moveCost, int[] coordinate)
    {
      Cost = moveCost;
      Coord = coordinate;
    }
  }
}
