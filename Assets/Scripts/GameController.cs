using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using Assets.Scripts.GameLogic;

public class GameController : MonoBehaviour
{
    // Related GameObjects
    public Camera MainCamera;
    // UI
    public GameObject CastleMenuUI;
    public GameObject BottomUI;
    public GameObject TopPanel;

    private GameObject SelectedUnit;

    // TODO move this to terrain handler
    public GameObject TerrainContainer;
    
    //Camera
    // Values for limiting camera movement
    private float CameraMinX { get; set; }
    private float CameraMaxX { get; set; }
    private float CameraMinY { get; set; }
    private float CameraMaxY { get; set; }

    // TODO Move to options
    private float PanSpeed = 0.10f;

    
    // TODO move these out of this class
    // Turn objects
    private int PlayerPointer;
    public TerrainTile[,] TerrainTiles { get; private set; }
    public List<Stack>[,] AllStacksGrid = new List<Stack>[Constants.MapSize, Constants.MapSize];
    public List<Stack> AllStacks = new List<Stack>();
    private List<Castle> AllCastles = new List<Castle>();

    //public List<UnitType> UnitTypes = new List<UnitType>();

    private List<Player> Players = new List<Player>();

    public GameDataContainer GameDataContainer { get; private set; }

    public static GameController Instance;

    void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;

        DontDestroyOnLoad(this);

        // for error handling and testing purpose
        Application.logMessageReceived += HandleUnityLog;
    }

    // Use this for initialization
    void Start()
    {
        /********************************************
         * TEMP code for testing
         ********************************************/
        // Unit Types
        GameDataContainer = new GameDataContainer();
        GameDataContainer.LoadUnitTypes();

        UnitType heavyInf = GameDataContainer.UnitTypes[0];
        UnitType cavalry = GameDataContainer.UnitTypes[1];
        UnitType dragon = GameDataContainer.UnitTypes[2];

        // temp initilization
        Player interactor = new Player
        {
            Name = "Karath",
            PlayerUnits = new PriorityQueueMin<UnitType>(new UnitType[] { heavyInf, cavalry, dragon }),
            Gold = 1000,
            BanneSpriteName = "DragonBanner",
        };
        Players.Add(interactor);

        Player enemy = new Player
        {
            Name = "Algast",
            PlayerUnits = new PriorityQueueMin<UnitType>(new UnitType[] { heavyInf, cavalry }),
            Gold = 100,
            BanneSpriteName = "KnightBanner",
        };
        Players.Add(enemy);

        #region TempTerrain
        // temp manual terrain 
        TerrainTiles = new TerrainTile[Constants.MapSize, Constants.MapSize];
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
            TerrainTiles[16 + i, 0] = new TerrainTile(TerrainTile.TerrainType.Forest);
        }
        TerrainTiles[19, 1] = new TerrainTile(TerrainTile.TerrainType.Forest);
        TerrainTiles[19, 2] = new TerrainTile(TerrainTile.TerrainType.Forest);

        // bottom left forest
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                TerrainTiles[4 + i, 17 + j] = new TerrainTile(TerrainTile.TerrainType.Forest);
            }
        }
        TerrainTiles[6, 18] = new TerrainTile(TerrainTile.TerrainType.Forest);

        // center mountain
        TerrainTiles[9, 4] = new TerrainTile(TerrainTile.TerrainType.Mountain);
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
            TerrainTiles[9, 8 + i] = new TerrainTile(TerrainTile.TerrainType.Road);
        }

        // Center bridge, overwrites road
        TerrainTiles[9, 12] = new TerrainTile(TerrainTile.TerrainType.Bridge);

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
                TerrainTiles[8 + i, 0 + j] = new TerrainTile(TerrainTile.TerrainType.Water);
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
        TerrainTiles[2, 5] = new TerrainTile(TerrainTile.TerrainType.Bridge);
        #endregion
        // instantiate terrain, main function
        InstantiateTerrain(TerrainTiles);


        // temp set owner
        Stack playerStack = GameObject.Find("PlayerStack").GetComponent<Stack>();
        playerStack.Owner = interactor;
        Unit playerUnit = new Unit(heavyInf);
        Unit playerUnit2 = new Unit(heavyInf);
        Unit playerUnit3 = new Unit(dragon);
        playerStack.AddUnit(playerUnit);
        playerStack.AddUnit(playerUnit2);
        playerStack.AddUnit(playerUnit3);
        playerStack.NextTurn();

        AddStackToAllStacks(playerStack);

        Stack enemyStack = GameObject.Find("EnemyStack").GetComponent<Stack>();
        enemyStack.Owner = enemy;
        Unit enemyUnit = new Unit(heavyInf);
        enemyStack.AddUnit(enemyUnit);
        enemyStack.NextTurn();

        AddStackToAllStacks(enemyStack);

        Stack enemyStack2 = GameObject.Find("EnemyStack2").GetComponent<Stack>();
        enemyStack2.Owner = enemy;
        Unit enemyUnit2 = new Unit(heavyInf);
        enemyStack2.AddUnit(enemyUnit2);
        enemyStack2.NextTurn();

        AddStackToAllStacks(enemyStack2);

        // Set starting player
        TopPanel.GetComponent<TopPanelUI>().SetCurrentPlayer(Players[PlayerPointer]);


        #region CameraInitialized
        // camera init vals
        // limit the camera based on the map size
        float vertExtent = MainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        // Size taken from bottom panel
        float uiPaddingBot = 2 * MainCamera.orthographicSize * (BottomUI.GetComponent<RectTransform>().rect.height / Screen.height);
        float uiPaddingTop = 2 * MainCamera.orthographicSize * (TopPanel.GetComponent<RectTransform>().rect.height / Screen.height);
        // off set is calculated based on the size of the map
        float offset = Constants.MapSize / 2f - 0.5f;
        CameraMinX = horzExtent - ((float)Constants.MapSize) / 2.0f + offset;
        CameraMaxX = ((float)Constants.MapSize) / 2.0f - horzExtent + offset;
        CameraMinY = vertExtent - ((float)Constants.MapSize) / 2.0f + offset - uiPaddingBot;
        CameraMaxY = ((float)Constants.MapSize) / 2.0f - vertExtent + offset + uiPaddingTop;
        if (CameraMaxX - CameraMinX < 0)
        {
            float diffX = CameraMinX - CameraMaxX;
            CameraMaxX += diffX / 2;
            CameraMinX -= diffX / 2;
        }
        if (CameraMaxY - CameraMinY < 0)
        {
            float diffY = CameraMinY - CameraMaxY;
            CameraMaxY += diffY / 2;
            CameraMinY -= diffY / 2;
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Camera panning
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A))
        { MainCamera.transform.Translate(Vector3.left * PanSpeed); }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
        { MainCamera.transform.Translate(Vector3.right * PanSpeed); }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.W))
        { MainCamera.transform.Translate(Vector3.up * PanSpeed); }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.S))
        { MainCamera.transform.Translate(Vector3.down * PanSpeed); }

        // limit camera
        Vector3 v3 = MainCamera.transform.position;
        v3.x = Mathf.Clamp(v3.x, CameraMinX, CameraMaxX);
        v3.y = Mathf.Clamp(v3.y, CameraMinY, CameraMaxY);
        MainCamera.transform.position = v3;

        /**************************
         * Right click mouse actions
         **************************/

        // Mouse pointer section
        if (Input.GetMouseButtonDown(0) && SelectedUnit == null && !CastleMenuUI.activeSelf)
        {
            // Position of mouse pointer
            Vector3 pos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = 0;

            RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
            foreach (RaycastHit2D h in hits)
            {
                Stack curStack = h.transform.GetComponent<Stack>();
                if (curStack != null && curStack.Owner == Players[PlayerPointer])
                {
                    SelectedUnit = h.collider.gameObject;
                    BottomUI.GetComponent<BottomUI>().SetSelectedStack(curStack);
                }
            }
        }

        // Handle stack interactions
        if (Input.GetMouseButtonDown(0) && SelectedUnit != null && !CastleMenuUI.activeSelf)
        {
            // Position of mouse pointer
            Vector3 pos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = 0;

            // check if selected object is a stack
            Stack curStack = SelectedUnit.gameObject.GetComponent<Stack>();
            if (curStack != null && checkMovementPossible(curStack, TerrainTiles[(int)pos.x, (int)pos.y]))
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
                    if (cas != null && cas.Owner != curStack.Owner && (Vector3.Distance(cas.transform.position, curStack.transform.position) < 3f))
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
                                KillStack(st);
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
                            KillStack(curStack);
                            SelectedUnit = null;
                        }
                        else
                        {
                            cas.ChangeOwner(curStack.Owner);
                        }
                        break;
                    }
                    else if (hitStack != null && curStack != null && curStack.Owner != hitStack.Owner)
                    {
                        // Check distance, if next to target, do battle
                        if (Vector3.Distance(hitStack.transform.position, curStack.transform.position) < 2f)
                        {
                            if (curStack.Battle(hitStack))
                            {
                                KillStack(hitStack);
                            }
                            else
                            {
                                KillStack(curStack);
                                SelectedUnit = null;
                                setMove = false;
                            }
                        }
                    }
                }
                if (setMove)
                {
                    // Set blocked paths depending on the castles and stacks
                    bool[,] obstructed = generateObstructedFields(curStack, AllStacks, AllCastles);

                    // Set stack movement path
                    curStack.Path = AStar.ShortestPath(TerrainTiles, obstructed, (int)SelectedUnit.transform.position.x,
                      (int)SelectedUnit.transform.position.y, (int)pos.x, (int)pos.y);
                }
                // update the UI, to deal with dead
                if (SelectedUnit != null)
                {
                    BottomUI.GetComponent<BottomUI>().SetSelectedStack(SelectedUnit.GetComponent<Stack>());
                }
                else if (SelectedUnit == null)
                {
                    BottomUI.GetComponent<BottomUI>().ClearSelectedStack();
                }
            }
        }


        /**************************
         * Left click mouse actions
         **************************/
        if (Input.GetMouseButtonDown(1))
        {
            // TODO fix for better handling of right click
            Vector3 pos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.x = Mathf.Round(pos.x);
            pos.y = Mathf.Round(pos.y);
            pos.z = 0;
            RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero);
            foreach (RaycastHit2D hit in hits)
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
            NextTurn();
        }

        // Deselect all
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectedUnit = null;
            BottomUI.GetComponent<BottomUI>().ClearSelectedStack();
        }

        // TODO remove
        //Test location mechanism
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            foreach (List<Stack> list in AllStacksGrid)
            {
                if (list != null)
                {
                    foreach (Stack st in list)
                    {
                        Debug.Log("Pos " + st.transform.position.ToString());
                    }
                }
            }
        }
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        // error message handling for debugging and testing purposes
    }

    private void NextTurn()
    {
        // TODO handle unit production, such that it is handled on player start of turn
        //first castles for unit creation
        foreach (Castle cas in AllCastles)
        {
            cas.NextTurn();
        }

        foreach (Stack st in AllStacks)
        {
            st.NextTurn();
        }

        // change current player
        PlayerPointer++;
        if (PlayerPointer >= Players.Count)
        {
            PlayerPointer = 0;
        }
        TopPanel.GetComponent<TopPanelUI>().SetCurrentPlayer(Players[PlayerPointer]);
        // clear selected unit
        SelectedUnit = null;
        BottomUI.GetComponent<BottomUI>().ClearSelectedStack();

        // TODO set the players last selected stack
        // update the UI, to deal with new Units
        //if (selectedUnit != null)
        //{
        //  mainUI.GetComponent<MainUI>().SetSelectedStack(selectedUnit.GetComponent<Stack>());
        //}
    }

    private bool[,] generateObstructedFields(Stack curStack, List<Stack> allStacks, List<Castle> allCastles)
    {
        bool[,] obstructed = new bool[Constants.MapSize, Constants.MapSize];
        foreach (Stack st in allStacks)
        {
            if (st.Owner != curStack.Owner)
            {
                obstructed[(int)st.transform.position.x, (int)st.transform.position.y] = true;
            }
        }
        foreach (Castle cas in allCastles)
        {
            if (cas.Owner != curStack.Owner)
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

    private void KillStack(Stack stack)
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
                terrainLayout[xcoord + i, ycoord + j] = new TerrainTile(TerrainTile.TerrainType.Castle);
            }
        }
    }
    private void tempWaterVertLine(TerrainTile[,] terrainLayout, int starty, int endy, int x)
    {
        for (int i = 0; i < endy - starty + 1; i++)
        {
            terrainLayout[x, starty + i] = new TerrainTile(TerrainTile.TerrainType.Water);
        }
    }
    private void tempWaterHoriLine(TerrainTile[,] terrainLayout, int startx, int endx, int y)
    {
        for (int i = 0; i < endx - startx + 1; i++)
        {
            terrainLayout[startx + i, y] = new TerrainTile(TerrainTile.TerrainType.Water);
        }
    }

    /// <summary>
    /// Method for instatiating the individual tiles for the level
    /// </summary>
    /// <param name="terrainLayout"></param>
    private void InstantiateTerrain(TerrainTile[,] terrainLayout)
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
                    foreach (Castle cas in AllCastles)
                    {
                        if (Vector3.Distance(cas.transform.position, new Vector3(i, j)) < 2)
                        {
                            toClose = true;
                        }
                    }
                    if (!toClose)
                    {
                        GameObject obj = Instantiate(Resources.Load("Prefabs/Castle", typeof(GameObject)), new Vector3(i + 0.5f, j + 0.5f), Quaternion.identity) as GameObject;
                        obj.transform.parent = TerrainContainer.transform;
                        Castle cas = obj.GetComponent<Castle>();
                        cas.Income = UnityEngine.Random.Range(5, 20);
                        AllCastles.Add(cas);
                    }
                }
                else
                {
                    GameObject obj = Instantiate(Resources.Load("Prefabs/" + terrainType, typeof(GameObject)), new Vector3(i, j), Quaternion.identity) as GameObject;
                    obj.transform.parent = TerrainContainer.transform;
                }

            }
        }
    }
}
