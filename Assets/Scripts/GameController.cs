using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Pathfinding;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
  // TODO move to class that deals with constants
  public static int DiceRange = 20;


  public Camera main;

  private GameObject selectedUnit;

  //private TerrainTile.TerrainType[,] terrainLayout;

  private TerrainTile[,] terrainTiles;

  private List<Stack> allStacks = new List<Stack>();

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
    terrainTiles = new TerrainTile[20, 20];
    // Fill with grass
    for (int i = 0; i < terrainTiles.GetLength(0); i++)
    {
      for (int j = 0; j < terrainTiles.GetLength(1); j++)
      {
        terrainTiles[i, j] = new TerrainTile(TerrainTile.TerrainType.Grass);
      }
    }
    // top left castle
    tempBuildCastle(terrainTiles, 2, 2);
    // top right castle
    tempBuildCastle(terrainTiles, 16, 2);
    // center castle 
    tempBuildCastle(terrainTiles, 10, 7);
    // bottom center castle
    tempBuildCastle(terrainTiles, 9, 15);
    // bottom right castle
    tempBuildCastle(terrainTiles, 15, 18);

    // top right forest
    for (int i = 0; i < 4; i++)
    {
      terrainTiles[16 + i, 0] = new TerrainTile( TerrainTile.TerrainType.Forest);
    }
    terrainTiles[19, 1] = new TerrainTile(TerrainTile.TerrainType.Forest);
    terrainTiles[19, 2] = new TerrainTile(TerrainTile.TerrainType.Forest);

    // bottom left forest
    for (int i = 0; i < 2; i++)
    {
      for (int j = 0; j < 2; j++)
      {
        terrainTiles[4 + i, 17 + j] = new TerrainTile( TerrainTile.TerrainType.Forest);
      }
    }
    terrainTiles[6, 18] = new TerrainTile(TerrainTile.TerrainType.Forest);

    // center mountain
    terrainTiles[9, 4] = new TerrainTile( TerrainTile.TerrainType.Mountain);
    terrainTiles[9, 5] = new TerrainTile(TerrainTile.TerrainType.Mountain);
    for (int i = 0; i < 2; i++)
    {
      for (int j = 0; j < 2; j++)
      {
        terrainTiles[7 + i, 5 + j] = new TerrainTile(TerrainTile.TerrainType.Mountain);
      }
    }
    terrainTiles[6, 7] = new TerrainTile(TerrainTile.TerrainType.Mountain);
    terrainTiles[7, 7] = new TerrainTile(TerrainTile.TerrainType.Mountain);

    // center road
    for (int i = 0; i < 7; i++)
    {
      terrainTiles[9, 8 + i] = new TerrainTile( TerrainTile.TerrainType.Road);
    }

    // Center bridge, overwrites road
    terrainTiles[9, 12] = new TerrainTile( TerrainTile.TerrainType.Bridge);

    // bottom road
    for (int i = 0; i < 2; i++)
    {
      terrainTiles[11 + i, 16] = new TerrainTile(TerrainTile.TerrainType.Road);
    }
    for (int i = 0; i < 4; i++)
    {
      terrainTiles[12 + i, 17] = new TerrainTile(TerrainTile.TerrainType.Road);
    }

    // water
    tempWaterVertLine(terrainTiles, 4, 15, 19);
    tempWaterVertLine(terrainTiles, 6, 14, 18);
    tempWaterVertLine(terrainTiles, 6, 13, 17);
    tempWaterVertLine(terrainTiles, 8, 12, 16);
    tempWaterVertLine(terrainTiles, 8, 12, 15);
    tempWaterVertLine(terrainTiles, 11, 12, 14);
    tempWaterHoriLine(terrainTiles, 10, 13, 12);

    for (int i = 0; i < 6; i++)
    {
      for (int j = 0; j < 3; j++)
      {
        terrainTiles[8 + i, 0 + j] =new TerrainTile( TerrainTile.TerrainType.Water);
      }
    }

    tempWaterVertLine(terrainTiles, 0, 4, 7);
    tempWaterVertLine(terrainTiles, 0, 6, 6);
    tempWaterVertLine(terrainTiles, 0, 7, 5);
    tempWaterHoriLine(terrainTiles, 0, 5, 0);

    tempWaterVertLine(terrainTiles, 5, 13, 4);
    tempWaterVertLine(terrainTiles, 5, 14, 3);
    tempWaterVertLine(terrainTiles, 5, 19, 2);
    tempWaterVertLine(terrainTiles, 5, 19, 1);
    tempWaterVertLine(terrainTiles, 0, 19, 0);

    tempWaterVertLine(terrainTiles, 11, 12, 5);
    tempWaterHoriLine(terrainTiles, 6, 8, 12);
    terrainTiles[2, 5] = new TerrainTile( TerrainTile.TerrainType.Bridge);
    #endregion
    // instantiate terrain, main function
    instantiateTerrain(terrainTiles);


    // temp set owner
    Stack playerStack = GameObject.Find("PlayerStack").GetComponent<Stack>();
    playerStack.Owner = interactor;
    Unit playerUnit = new Unit(3, 3);
    playerUnit.Speed = 8;
    Unit playerUnit2 = new Unit(3, 3);
    playerUnit2.Speed = 8;
    Unit playerUnit3 = new Unit(11, 3);
    playerUnit3.Speed = 8;
    playerStack.Units.Add(playerUnit);
    playerStack.Units.Add(playerUnit2);
    playerStack.Units.Add(playerUnit3);

    allStacks.Add(playerStack);

    Stack enemyStack = GameObject.Find("EnemyStack").GetComponent<Stack>();
    enemyStack.Owner = enemy;
    Unit enemyUnit = new Unit(3, 3);
    enemyUnit.Speed = 8;
    enemyStack.Units.Add(enemyUnit);

    allStacks.Add(enemyStack);

    Stack enemyStack2 = GameObject.Find("EnemyStack2").GetComponent<Stack>();
    enemyStack2.Owner = enemy;
    Unit enemyUnit2 = new Unit(3, 3);
    enemyUnit2.Speed = 8;
    enemyStack2.Units.Add(enemyUnit2);

    allStacks.Add(enemyStack2);

    Castle cas = GameObject.Find("Castle").GetComponent<Castle>();
    cas.Owner = enemy;

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

    // Handle stack interactions
    if (Input.GetMouseButtonDown(1) && selectedUnit != null)
    {
      // Position of mouse pointer
      Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
      pos.x = Mathf.Round(pos.x);
      pos.y = Mathf.Round(pos.y);
      pos.z = 0;

      // check if selected object is a stack
      Stack curStack = selectedUnit.gameObject.GetComponent<Stack>();
      if(curStack != null && checkMovementPossible(curStack, terrainTiles[(int)pos.x, (int)pos.y]))
      {
        // assume the stack will move, unless something is hit
        bool setMove = true;
        // hit all objects, incase of castle beneath stack
        // TODO handle multiple stacked stacks
        RaycastHit2D[] hits = Physics2D.RaycastAll(main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        foreach (RaycastHit2D h in hits)
        {
          // check hit componenent
          Castle cas = h.collider.gameObject.GetComponent<Castle>();
          Stack hitStack = h.collider.gameObject.GetComponent<Stack>();
          // check if the castle is hostile then do battle
          if (cas != null && cas.Owner !=curStack.Owner && (Vector3.Distance(cas.transform.position, curStack.transform.position) < 3f))
          {
            Debug.Log("Castle fight");
            // Handle battle against castles
            bool win = true;
            // Fight the entire garrison
            //TODO handle fights in groups
            foreach (Stack st in cas.Garrison)
            {
              if (curStack.Battle(st))
              {
                killStack(st);
              }
              else
              {
                win = false;
                break;
              }
            }
            if (!win)
            {
              killStack(curStack);
            }
            else
            {
              cas.Owner = curStack.Owner;
            }
          }
          else if (hitStack != null && curStack != null && curStack != hitStack)
          {
            //setMove = false;
            // Check distance, if next to target, do battle
            if (Vector3.Distance(hitStack.transform.position, curStack.transform.position) < 2f)
            {
              if (curStack.Battle(hitStack))
              {
                killStack(hitStack);
              }
              else
              {
                killStack(curStack);
              }
            }
          }
        }

        if (setMove)
        {
          // Set stack movement path
          curStack.Path = AStar.ShortestPath(terrainTiles, (int)selectedUnit.transform.position.x,
            (int)selectedUnit.transform.position.y, (int)pos.x, (int)pos.y);
        }
      }
    }

    // Next turn. Set all stacks movement. Should be called from else where
    if (Input.GetKeyDown(KeyCode.Space))
    {
      foreach(Stack st in allStacks)
      {
        st.SetStackStartMovement();
      }
    }
  }

  private bool checkMovementPossible(Stack curStack, TerrainTile tile)
  {
    return curStack.Movement >= tile.Movecost;
  }

  private void killStack(Stack stack)
  {
    allStacks.Remove(stack);
    Destroy(stack.gameObject);
  }

  private void tempBuildCastle(TerrainTile[,] terrainLayout, int xcoord, int ycoord)
  {
    for (int i = 0; i < 2; i++)
    {
      for (int j = 0; j < 2; j++)
      {
        terrainLayout[xcoord + i, ycoord + j] = new TerrainTile( TerrainTile.TerrainType.Castle);
      }
    }
  }
  private void tempWaterVertLine(TerrainTile[,] terrainLayout, int starty, int endy, int x)
  {
    for (int i = 0; i < endy - starty + 1; i++)
    {
      terrainLayout[x, starty + i] = new TerrainTile( TerrainTile.TerrainType.Water);
    }
  }
  private void tempWaterHoriLine(TerrainTile[,] terrainLayout, int startx, int endx, int y)
  {
    for (int i = 0; i < endx - startx + 1; i++)
    {
      terrainLayout[startx + i, y] = new TerrainTile( TerrainTile.TerrainType.Water);
    }
  }

  private void instantiateTerrain(TerrainTile[,] terrainLayout)
  {
    for (int i = 0; i < terrainLayout.GetLength(0); i++)
    {
      for (int j = 0; j < terrainLayout.GetLength(1); j++)
      {
        string terrainType = "GrassPlaceholder";
        switch (terrainLayout[i, j].Type)
        {
          case TerrainTile.TerrainType.Grass:
            terrainType = "GrassPlaceholder";
            break;
          case TerrainTile.TerrainType.Castle:
            terrainType = "CastlePlaceholder";
            break;
          case TerrainTile.TerrainType.Bridge:
            terrainType = "BridgePlaceholder";
            break;
          case TerrainTile.TerrainType.Forest:
            terrainType = "ForestPlaceholder";
            break;
          case TerrainTile.TerrainType.Mountain:
            terrainType = "MountainPlaceholder";
            break;
          case TerrainTile.TerrainType.Road:
            terrainType = "RoadPlaceholder";
            break;
          case TerrainTile.TerrainType.Water:
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
