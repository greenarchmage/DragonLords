using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TerrainTile
{
  public enum TerrainType
  {
    Road, Forest, Mountain, Castle, Bridge, Water, Grass
  }

  public enum MoveType
  {
    WaterPassable, Impassable, Normal, Flying, Transition
  }

  public TerrainType Type { get; set; }
  public MoveType MoveGroup { get; set; }
  public int Movecost { get; set; }

  public TerrainTile(TerrainTile.TerrainType type)
  {
    Type = type;
    switch (type)
    {
      case TerrainTile.TerrainType.Bridge:
        Movecost = 1;
        MoveGroup = MoveType.Transition;
        break;
      case TerrainTile.TerrainType.Castle:
        Movecost = 1;
        MoveGroup = MoveType.Normal;
        break;
      case TerrainTile.TerrainType.Forest:
        Movecost = 2;
        MoveGroup = MoveType.Flying;
        break;
      case TerrainTile.TerrainType.Grass:
        Movecost = 2;
        MoveGroup = MoveType.Normal;
        break;
      case TerrainTile.TerrainType.Mountain:
        Movecost = 2;
        MoveGroup = MoveType.Flying;
        break;
      case TerrainTile.TerrainType.Road:
        Movecost = 1;
        MoveGroup = MoveType.Transition;
        break;
      case TerrainTile.TerrainType.Water:
        Movecost = 1;
        MoveGroup = MoveType.WaterPassable;
        break;
    }
  }
}

