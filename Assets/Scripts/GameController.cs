using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;

public class GameController : MonoBehaviour
{
  // Related GameObjects
  public Camera main;
  public GameObject CastleMenuUI;

  public GameObject mainUI;

  //private static int size = 20;
  private GameObject selectedUnit;
  public TerrainTile[,] TerrainTiles { get; private set; }
  public List<Stack>[,] AllStacksGrid = new List<Stack>[Constants.Size, Constants.Size];
  public List<Stack> AllStacks = new List<Stack>();
  private List<Castle> allCastles = new List<Castle>();

  public List<UnitType> UnitTypes = new List<UnitType>();

  //Camera
  private float minX;
  private float maxX;
  private float minY;
  private float maxY;

  private float panSpeed = 0.10f;

  public static GameController Instance;

  public GameObject soundManager;
  public int score;

  void Awake()
  {
    if (Instance != null)
      GameObject.Destroy(Instance);
    else
      Instance = this;

    DontDestroyOnLoad(this);

    // for error handling and testing purpose
    Application.logMessageReceived += handleUnityLog;
  }

  // Use this for initialization
  void Start()
  {
    /********************************************
     * TEMP code for testing
     ********************************************/
    // Unit Types
    UnitType heavyInf = new UnitType("Heavy Infantry", 3, 2, 16, 1,"HeavyInfantry", 80,1, TerrainTile.MoveType.Normal);
    UnitType cavalry = new UnitType("Cavalry", 4, 2, 24, 2, "Cavalry", 120,2, TerrainTile.MoveType.Normal);
    UnitType dragon = new UnitType("Dragon", 9, 3, 20, 3, "Dragon", 300,3, TerrainTile.MoveType.Flying);
    UnitTypes.Add(heavyInf);
    UnitTypes.Add(cavalry);
    UnitTypes.Add(dragon);
    // temp initilization
    Player interactor = new Player();
    interactor.Name = "Karath";
    interactor.PlayerUnits = new Assets.Scripts.Utility.PriorityQueueMin<UnitType>(new UnitType[] { heavyInf, cavalry, dragon });
    interactor.Gold = 1000;

    Player enemy = new Player();
    enemy.Name = "Algast";
    enemy.PlayerUnits = new Assets.Scripts.Utility.PriorityQueueMin<UnitType>(new UnitType[] { heavyInf, cavalry });
    enemy.Gold = 100;

    #region TempTerrain
    // temp manual terrain 
    TerrainTiles = new TerrainTile[Constants.Size, Constants.Size];
    // Fill with grass
    for (int i = 0; i < TerrainTiles.GetLength(0); i++)
    {
      for (int j = 0; j < TerrainTiles.GetLength(1); j++)
      {
        TerrainTiles[i, j] = new TerrainTile(TerrainTile.TerrainType.Grass);
      }
    }
    // top left castle
    tempBuildCastle(TerrainTiles, 2, 2);
    // top right castle
    tempBuildCastle(TerrainTiles, 16, 2);
    // center castle 
    tempBuildCastle(TerrainTiles, 10, 7);
    // bottom center castle
    tempBuildCastle(TerrainTiles, 9, 15);
    // bottom right castle
    tempBuildCastle(TerrainTiles, 15, 18);

    // top right forest
    for (int i = 0; i < 4; i++)
    {
      TerrainTiles[16 + i, 0] = new TerrainTile( TerrainTile.TerrainType.Forest);
    }
    TerrainTiles[19, 1] = new TerrainTile(TerrainTile.TerrainType.Forest);
    TerrainTiles[19, 2] = new TerrainTile(TerrainTile.TerrainType.Forest);

    // bottom left forest
    for (int i = 0; i < 2; i++)
    {
      for (int j = 0; j < 2; j++)
      {
        TerrainTiles[4 + i, 17 + j] = new TerrainTile( TerrainTile.TerrainType.Forest);
      }
    }
    TerrainTiles[6, 18] = new TerrainTile(TerrainTile.TerrainType.Forest);

    // center mountain
    TerrainTiles[9, 4] = new TerrainTile( TerrainTile.TerrainType.Mountain);
    TerrainTiles[9, 5] = new TerrainTile(TerrainTile.TerrainType.Mountain);
    for (int i = 0; i < 2; i++)
    {
      for (int j = 0; j < 2; j++)
      {
        TerrainTiles[7 + i, 5 + j] = new TerrainTile(TerrainTile.TerrainType.Mountain);
      }
    }
    TerrainTiles[6, 7] = new TerrainTile(TerrainTile.TerrainType.Mountain);
    TerrainTiles[7, 7] = new TerrainTile(TerrainTile.TerrainType.Mountain);

    // center road
    for (int i = 0; i < 7; i++)
    {
      TerrainTiles[9, 8 + i] = new TerrainTile( TerrainTile.TerrainType.Road);
    }

    // Center bridge, overwrites road
    TerrainTiles[9, 12] = new TerrainTile( TerrainTile.TerrainType.Bridge);

    // bottom road
    for (int i = 0; i < 2; i++)
    {
      TerrainTiles[11 + i, 16] = new TerrainTile(TerrainTile.TerrainType.Road);
    }
    for (int i = 0; i < 4; i++)
    {
      TerrainTiles[12 + i, 17] = new TerrainTile(TerrainTile.TerrainType.Road);
    }

    // water
    tempWaterVertLine(TerrainTiles, 4, 15, 19);
    tempWaterVertLine(TerrainTiles, 6, 14, 18);
    tempWaterVertLine(TerrainTiles, 6, 13, 17);
    tempWaterVertLine(TerrainTiles, 8, 12, 16);
    tempWaterVertLine(TerrainTiles, 8, 12, 15);
    tempWaterVertLine(TerrainTiles, 11, 12, 14);
    tempWaterHoriLine(TerrainTiles, 10, 13, 12);

    for (int i = 0; i < 6; i++)
    {
      for (int j = 0; j < 3; j++)
      {
        TerrainTiles[8 + i, 0 + j] =new TerrainTile( TerrainTile.TerrainType.Water);
      }
    }

    tempWaterVertLine(TerrainTiles, 0, 4, 7);
    tempWaterVertLine(TerrainTiles, 0, 6, 6);
    tempWaterVertLine(TerrainTiles, 0, 7, 5);
    tempWaterHoriLine(TerrainTiles, 0, 5, 0);

    tempWaterVertLine(TerrainTiles, 5, 13, 4);
    tempWaterVertLine(TerrainTiles, 5, 14, 3);
    tempWaterVertLine(TerrainTiles, 5, 19, 2);
    tempWaterVertLine(TerrainTiles, 5, 19, 1);
    tempWaterVertLine(TerrainTiles, 0, 19, 0);

    tempWaterVertLine(TerrainTiles, 11, 12, 5);
    tempWaterHoriLine(TerrainTiles, 6, 8, 12);
    TerrainTiles[2, 5] = new TerrainTile( TerrainTile.TerrainType.Bridge);
    #endregion
    // instantiate terrain, main function
    instantiateTerrain(TerrainTiles);


    // temp set owner
    Stack playerStack = GameObject.Find("PlayerStack").GetComponent<Stack>();
    playerStack.Owner = interactor;
    Unit playerUnit = new Unit(heavyInf);
    Unit playerUnit2 = new Unit(heavyInf);
    Unit playerUnit3 = new Unit(dragon);
    playerStack.AddUnit(playerUnit);
    playerStack.AddUnit(playerUnit2);
    playerStack.AddUnit(playerUnit3);

    AddStackToAllStacks(playerStack);

    Stack enemyStack = GameObject.Find("EnemyStack").GetComponent<Stack>();
    enemyStack.Owner = enemy;
    Unit enemyUnit = new Unit(heavyInf);
    enemyStack.AddUnit(enemyUnit);

    AddStackToAllStacks(enemyStack);

    Stack enemyStack2 = GameObject.Find("EnemyStack2").GetComponent<Stack>();
    enemyStack2.Owner = enemy;
    Unit enemyUnit2 = new Unit(heavyInf);
    enemyStack2.AddUnit(enemyUnit2);

    AddStackToAllStacks(enemyStack2);

    // camera init vals
    // limit the camera based on the map size
    float vertExtent = main.orthographicSize;
    float horzExtent = vertExtent * Screen.width / Screen.height;

    // Size taken from bottom panel
    float uiPadding = 2*main.orthographicSize *(mainUI.GetComponent<RectTransform>().rect.height / Screen.height);
    // off set is calculated based on the size of the map
    float offset = Constants.Size/2f -0.5f;
    minX = horzExtent - ((float)Constants.Size) / 2.0f + offset;
    maxX = ((float)Constants.Size) / 2.0f - horzExtent + offset;
    minY = vertExtent - ((float)Constants.Size) / 2.0f + offset- uiPadding;
    maxY = ((float)Constants.Size) / 2.0f - vertExtent + offset;
    if(maxX- minX < 0)
    {
      float diffX = minX - maxX;
      maxX += diffX/2;
      minX -= diffX / 2;
    }
    if(maxY - minY < 0)
    {
      float diffY = minY - maxY;
      maxY += diffY / 2;
      minY -= diffY / 2;
    }
  }

  // Update is called once per frame
  void Update()
  {
    // Camera panning
    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
    { main.transform.Translate(Vector3.left* panSpeed); }
    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.RightArrow))
    { main.transform.Translate(Vector3.right * panSpeed); }
    if (Input.GetKey(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow))
    { main.transform.Translate(Vector3.up * panSpeed); }
    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow))
    { main.transform.Translate(Vector3.down * panSpeed); }

    // limit camera
    Vector3 v3 = main.transform.position;
    v3.x = Mathf.Clamp(v3.x, minX, maxX);
    v3.y = Mathf.Clamp(v3.y, minY, maxY);
    main.transform.position = v3;

    /**************************
     * Right click mouse actions
     **************************/

    // Mouse pointer section
    if (Input.GetMouseButtonDown(0) && selectedUnit == null && !CastleMenuUI.activeSelf)
    {
      // Position of mouse pointer
      Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
      pos.x = Mathf.Round(pos.x);
      pos.y = Mathf.Round(pos.y);
      pos.z = 0;

      RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
      foreach (RaycastHit2D h in hits)
      {
        Stack curStack = h.transform.GetComponent<Stack>();
        if (curStack != null)
        {
          selectedUnit = h.collider.gameObject;
          mainUI.GetComponent<MainUI>().SetSelectedStack(curStack);
        }
      }
    }

    // Handle stack interactions
    if (Input.GetMouseButtonDown(0) && selectedUnit != null && !CastleMenuUI.activeSelf)
    {
      // Position of mouse pointer
      Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
      pos.x = Mathf.Round(pos.x);
      pos.y = Mathf.Round(pos.y);
      pos.z = 0;

      // check if selected object is a stack
      Stack curStack = selectedUnit.gameObject.GetComponent<Stack>();
      if(curStack != null && checkMovementPossible(curStack, TerrainTiles[(int)pos.x, (int)pos.y]))
      {
        // assume the stack will move, unless something is hit
        bool setMove = true;
        // hit all objects, incase of castle beneath stack
        // TODO handle multiple stacked stacks
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
        foreach (RaycastHit2D h in hits)
        {
          // check hit componenent
          Castle cas = h.collider.gameObject.GetComponent<Castle>();
          Stack hitStack = h.collider.gameObject.GetComponent<Stack>();
          // check if the castle is hostile then do battle
          if (cas != null && cas.Owner !=curStack.Owner && (Vector3.Distance(cas.transform.position, curStack.transform.position) < 3f))
          {
            // Handle battle against castles
            bool win = true;
            // Fight the entire garrison
            //TODO handle fights in groups, adding all the stack together in a great stack, such that bonuses stack
            foreach (Stack st in cas.Garrison)
            {
              if (curStack.Battle(st))
              {
                Debug.Log("Kill hit stack");
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
              Debug.Log("Kill cur stack");
              killStack(curStack);
              selectedUnit = null;
            }
            else
            {
              cas.ChangeOwner(curStack.Owner);
            }
            break;
          }
          else if (hitStack != null && curStack != null && curStack != hitStack)
          {
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
                selectedUnit = null;
                setMove = false;
              }
            }
          }
        }
        if (setMove)
        {
          // Set blocked paths depending on the castles and stacks
          bool[,] obstructed = generateObstructedFields(curStack,AllStacks, allCastles);

          // Set stack movement path
          curStack.Path = AStar.ShortestPath(TerrainTiles, obstructed, (int)selectedUnit.transform.position.x,
            (int)selectedUnit.transform.position.y, (int)pos.x, (int)pos.y);
        }
        // update the UI, to deal with dead
        if (selectedUnit != null)
        {
          mainUI.GetComponent<MainUI>().SetSelectedStack(selectedUnit.GetComponent<Stack>());
        } else if(selectedUnit == null)
        {
          mainUI.GetComponent<MainUI>().ClearSelectedStack();
        }
      }
    }


    /**************************
     * Left click mouse actions
     **************************/
    if (Input.GetMouseButtonDown(1))
    {
      // TODO fix for better handling of right click
      Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
      pos.x = Mathf.Round(pos.x);
      pos.y = Mathf.Round(pos.y);
      pos.z = 0;
      RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
      foreach(RaycastHit2D hit in hits)
      {
        Castle hitCas = hit.transform.gameObject.GetComponent<Castle>();
        if (hitCas != null && hitCas.Owner != null)
        {
          CastleMenuUI.SetActive(true);
          CastleMenuUI.GetComponent<CastleMenu>().SetCastle(hitCas);
        }
      }
    }

    /************************************
     * Hotkey actions
     ***********************************/
    // Next turn. Set all stacks movement. Should be called from else where
    if (Input.GetKeyDown(KeyCode.Return))
    {
      //first castles for unit creation
      foreach (Castle cas in allCastles)
      {
        cas.NextTurn();
      }

      foreach (Stack st in AllStacks)
      {
        st.NextTurn();
      }
      // update the UI, to deal with new Units
      if (selectedUnit != null)
      {
        mainUI.GetComponent<MainUI>().SetSelectedStack(selectedUnit.GetComponent<Stack>());
      }
    }

    // Deselect all
    if (Input.GetKeyDown(KeyCode.Space))
    {
      selectedUnit = null;
      mainUI.GetComponent<MainUI>().ClearSelectedStack();
    }

    //Test location mechanism
    if (Input.GetKeyDown(KeyCode.LeftControl))
    {
      foreach(List<Stack> list in AllStacksGrid)
      {
        if(list != null)
        {
          foreach (Stack st in list)
          {
            Debug.Log("Pos " + st.transform.position.ToString());
          }
        }
      }
    }
  }

  private void handleUnityLog(string logString, string stackTrace, LogType type)
  {
    // do stuff
  }


  private bool[,] generateObstructedFields(Stack curStack, List<Stack> allStacks, List<Castle> allCastles)
  {
    bool[,] obstructed = new bool[Constants.Size, Constants.Size];
    foreach(Stack st in allStacks)
    {
      if(st.Owner != curStack.Owner)
      {
        obstructed[(int)st.transform.position.x, (int)st.transform.position.y] = true;
      }
    }
    foreach(Castle cas in allCastles)
    {
      if(cas.Owner != curStack.Owner)
      {
        float casPosX = cas.transform.position.x;
        float casPosY = cas.transform.position.y;
        obstructed[Mathf.FloorToInt(casPosX), Mathf.FloorToInt(casPosY)] = true;
        obstructed[Mathf.FloorToInt(casPosX), Mathf.CeilToInt(casPosY)] = true;
        obstructed[Mathf.CeilToInt(casPosX), Mathf.FloorToInt(casPosY)] = true;
        obstructed[Mathf.CeilToInt(casPosX), Mathf.CeilToInt(casPosY)] = true;
      }
    }
    return obstructed;
  }

  private bool checkMovementPossible(Stack curStack, TerrainTile tile)
  {
    return curStack.Movement >= tile.Movecost;
  }

  private void killStack(Stack stack)
  {
    RemoveStackFromAllStacks(stack);
    Destroy(stack.gameObject);
  }

  public void RemoveStackFromAllStacks(Stack stack)
  {
    AllStacks.Remove(stack);
    AllStacksGrid[(int)stack.transform.position.x, (int)stack.transform.position.y].Remove(stack);
  }
  public void AddStackToAllStacks(Stack stack)
  {
    AllStacks.Add(stack);
    addStackToGrid(stack);
  }

  public void UpdateStackPosition(Stack stack, Vector3 oldPos)
  {
    AllStacksGrid[(int)oldPos.x, (int)oldPos.y].Remove(stack);
    addStackToGrid(stack);
  }

  private void addStackToGrid(Stack stack)
  {
    if (AllStacksGrid[(int)stack.transform.position.x, (int)stack.transform.position.y] == null)
    {
      AllStacksGrid[(int)stack.transform.position.x, (int)stack.transform.position.y] = new List<Stack>();
    }
    AllStacksGrid[(int)stack.transform.position.x, (int)stack.transform.position.y].Add(stack);
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
        if (terrainType == "CastlePlaceholder")
        {
          bool toClose = false;
          foreach (Castle cas in allCastles)
          {
            if(Vector3.Distance( cas.transform.position,new Vector3(i,j)) < 2)
            {
              toClose = true;
            }
          }
          if (!toClose)
          {
            GameObject obj = Instantiate(Resources.Load("Prefabs/Castle", typeof(GameObject)), new Vector3(i + 0.5f, j + 0.5f), Quaternion.identity) as GameObject;
            Castle cas = obj.GetComponent<Castle>();
            allCastles.Add(cas);
          }
        } else
        {
          Instantiate(Resources.Load("Prefabs/" + terrainType, typeof(GameObject)), new Vector3(i, j), Quaternion.identity);
        }
        
      }
    }
  }
}
