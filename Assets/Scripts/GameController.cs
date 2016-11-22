using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour
{
  // TODO move to class that deals with constants
  public static int DiceRange = 20;


  public Camera main;

  private GameObject selectedUnit;

  // Use this for initialization
  void Start()
  {
    // temp initilization
    Player interactor = new Player();
    interactor.Name = "Karath";

    Player enemy = new Player();
    enemy.Name = "Algast";

    #region TempTerrain
    // temp manual terrain 
    Terrain.TerrainType[,] terrainLayout = new Terrain.TerrainType[20,20];
    // Fill with grass
    for(int i = 0; i<terrainLayout.GetLength(0); i++)
    {
      for(int j = 0; j < terrainLayout.GetLength(1); j++)
      {
        terrainLayout[i, j] = Terrain.TerrainType.Grass;
      }
    }
    // top left castle
    tempBuildCastle(terrainLayout, 2, 2);
    // top right castle
    tempBuildCastle(terrainLayout, 16, 2);
    // center castle 
    tempBuildCastle(terrainLayout, 10, 7);
    // bottom center castle
    tempBuildCastle(terrainLayout, 9, 15);
    // bottom right castle
    tempBuildCastle(terrainLayout, 15, 18);

    // top right forest
    for(int i = 0; i < 4; i++)
    {
      terrainLayout[16 + i, 0] = Terrain.TerrainType.Forest;
    }
    terrainLayout[19, 1] = Terrain.TerrainType.Forest;
    terrainLayout[19, 2] = Terrain.TerrainType.Forest;

    // bottom left forest
    for(int i = 0; i < 2; i++)
    {
      for(int j = 0; j < 2; j++)
      {
        terrainLayout[4+i, 17+j] = Terrain.TerrainType.Forest;
      }
    }
    terrainLayout[6, 18] = Terrain.TerrainType.Forest;

    // center mountain
    terrainLayout[9, 4] = Terrain.TerrainType.Mountain;
    terrainLayout[9, 5] = Terrain.TerrainType.Mountain;
    for(int i = 0; i<2; i++)
    {
      for(int j=0; j < 2; j++)
      {
        terrainLayout[7+i, 5+j] = Terrain.TerrainType.Mountain;
      }
    }
    terrainLayout[6, 7] = Terrain.TerrainType.Mountain;
    terrainLayout[7, 7] = Terrain.TerrainType.Mountain;

    // center road
    for (int i = 0; i < 7; i++)
    {
      terrainLayout[9, 8 + i] = Terrain.TerrainType.Road;
    }

    // Center bridge, overwrites road
    terrainLayout[9, 12] = Terrain.TerrainType.Bridge;

    // bottom road
    for (int i = 0; i < 2; i++)
    {
      terrainLayout[11+i, 16] = Terrain.TerrainType.Road;
    }
    for (int i = 0; i < 4; i++)
    {
      terrainLayout[12+i, 17] = Terrain.TerrainType.Road;
    }

    // water
    tempWaterVertLine(terrainLayout, 4, 15, 19);
    tempWaterVertLine(terrainLayout, 6, 14, 18);
    tempWaterVertLine(terrainLayout, 6, 13, 17);
    tempWaterVertLine(terrainLayout, 8, 12, 16);
    tempWaterVertLine(terrainLayout, 8, 12, 15);
    tempWaterVertLine(terrainLayout, 11, 12, 14);
    tempWaterHoriLine(terrainLayout, 10, 13, 12);

    for(int i = 0; i < 6; i++)
    {
      for(int j = 0; j < 3; j++)
      {
        terrainLayout[8+i, 0+j] = Terrain.TerrainType.Water;
      }
    }

    tempWaterVertLine(terrainLayout, 0, 4, 7);
    tempWaterVertLine(terrainLayout, 0, 6, 6);
    tempWaterVertLine(terrainLayout, 0, 7, 5);
    tempWaterHoriLine(terrainLayout, 0, 5, 0);

    tempWaterVertLine(terrainLayout, 5, 13, 4);
    tempWaterVertLine(terrainLayout, 5, 14, 3);
    tempWaterVertLine(terrainLayout, 5, 19, 2);
    tempWaterVertLine(terrainLayout, 5, 19, 1);
    tempWaterVertLine(terrainLayout, 0, 19, 0);

    tempWaterVertLine(terrainLayout, 11, 12, 5);
    tempWaterHoriLine(terrainLayout, 6, 8, 12);
    #endregion
    // instantiate terrain, main function
    instantiateTerrain(terrainLayout);
    

    // temp set owner
    Stack playerStack = GameObject.Find("PlayerStack").GetComponent<Stack>();
    playerStack.Owner = interactor;
    playerStack.Units.Add(new Unit(3, 3));

    Stack enemyStack = GameObject.Find("EnemyStack").GetComponent<Stack>();
    enemyStack.Owner = enemy;
    enemyStack.Units.Add(new Unit(3, 3));
  }

  

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {

      RaycastHit2D hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
      if (hit)
      {
        selectedUnit = hit.collider.gameObject;
        Debug.Log(hit.collider.transform.name);
      }
    }

    if (Input.GetMouseButtonDown(1) && selectedUnit != null)
    {
      RaycastHit2D hit = Physics2D.Raycast(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
      if (hit)
      {
        Debug.Log(hit.collider.transform.name);
        Stack hitStack = hit.collider.gameObject.GetComponent<Stack>();
        Stack curStack = selectedUnit.gameObject.GetComponent<Stack>();
        if (hitStack != null && curStack != null && curStack != hitStack)
        {
          // Do battle
          if (curStack.Battle(hitStack))
          {
            Destroy(hit.transform.gameObject);
          } else
          {
            Destroy(selectedUnit);
          }
        }
      }
      Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
      pos.x = Mathf.Round(pos.x);
      pos.y = Mathf.Round(pos.y);
      pos.z = 0;
      selectedUnit.transform.position = pos;
    }
  }

  private void tempBuildCastle(Terrain.TerrainType[,] terrainLayout ,int xcoord, int ycoord)
  {
    for (int i = 0; i < 2; i++)
    {
      for (int j = 0; j < 2; j++)
      {
        terrainLayout[xcoord + i, ycoord + j] = Terrain.TerrainType.Castle;
      }
    }
  }

  private void tempWaterVertLine(Terrain.TerrainType[,] terrainLayout,int starty, int endy, int x)
  {
    for(int i = 0; i < endy - starty+1; i++)
    {
      terrainLayout[x, starty+i] = Terrain.TerrainType.Water;
    }
  }
  private void tempWaterHoriLine(Terrain.TerrainType[,] terrainLayout, int startx, int endx, int y)
  {
    for (int i = 0; i < endx - startx+1; i++)
    {
      terrainLayout[startx + i, y] = Terrain.TerrainType.Water;
    }
  }

  private void instantiateTerrain(Terrain.TerrainType[,] terrainLayout)
  {
    for (int i = 0; i < terrainLayout.GetLength(0); i++)
    {
      for (int j = 0; j < terrainLayout.GetLength(1); j++)
      {
        string terrainType = "GrassPlaceholder";
        switch (terrainLayout[i, j])
        {
          case Terrain.TerrainType.Grass:
            terrainType = "GrassPlaceholder";
            break;
          case Terrain.TerrainType.Castle:
            terrainType = "CastlePlaceholder";
            break;
          case Terrain.TerrainType.Bridge:
            terrainType = "BridgePlaceholder";
            break;
          case Terrain.TerrainType.Forest:
            terrainType = "ForestPlaceholder";
            break;
          case Terrain.TerrainType.Mountain:
            terrainType = "MountainPlaceholder";
            break;
          case Terrain.TerrainType.Road:
            terrainType = "RoadPlaceholder";
            break;
          case Terrain.TerrainType.Water:
            terrainType = "WaterPlaceholder";
            break;
          default:
            break;
        }
        Instantiate(Resources.Load("Prefabs/" + terrainType, typeof(GameObject)), new Vector3(i, j), Quaternion.identity);
      }
    }
  }
}
