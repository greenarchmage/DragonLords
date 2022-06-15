using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Pathfinding;
using System.Collections.Generic;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using Assets.Scripts.GameLogic;
using System.Linq;

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
    private List<Castle> AllCastles = new List<Castle>();

    public GameDataContainer GameDataContainer { get; private set; }
    public CurrentGameData CurrentGameData { get; private set; }

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
        // Container for all application data
        GameDataContainer = new GameDataContainer();
        GameDataContainer.LoadUnitTypes();

        UnitType heavyInf = GameDataContainer.UnitTypes[0];
        UnitType dragon = GameDataContainer.UnitTypes[2];

        // temp initilization
        CurrentGameData = new CurrentGameData(GameDataContainer);

        // instantiate terrain, main function
        InstantiateTerrain(CurrentGameData.TerrainTiles);

        // temp set owner
        Stack playerStack = GameObject.Find("PlayerStack").GetComponent<Stack>();
        playerStack.StackData.Owner = CurrentGameData.Players[0];
        var playerUnit = new Unit(heavyInf);
        var playerUnit2 = new Unit(heavyInf);
        var playerUnit3 = new Unit(dragon);
        playerStack.AddUnit(playerUnit);
        playerStack.AddUnit(playerUnit2);
        playerStack.AddUnit(playerUnit3);
        playerStack.NextTurn();

        AddStackToAllStacks(playerStack);

        Stack enemyStack = GameObject.Find("EnemyStack").GetComponent<Stack>();
        enemyStack.StackData.Owner = CurrentGameData.Players[1];
        var enemyUnit = new Unit(heavyInf);
        enemyStack.AddUnit(enemyUnit);
        enemyStack.NextTurn();

        AddStackToAllStacks(enemyStack);

        Stack enemyStack2 = GameObject.Find("EnemyStack2").GetComponent<Stack>();
        enemyStack2.StackData.Owner = CurrentGameData.Players[1];
        var enemyUnit2 = new Unit(heavyInf);
        enemyStack2.AddUnit(enemyUnit2);
        enemyStack2.NextTurn();

        AddStackToAllStacks(enemyStack2);

        // Set starting player
        TopPanel.GetComponent<TopPanelUI>().SetCurrentPlayer(CurrentGameData.CurrentPlayer);

        #region CameraInitialized
        // camera init vals
        // limit the camera based on the map size
        float vertExtent = MainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        // Size taken from bottom panel
        float uiPaddingBot = 2 * MainCamera.orthographicSize * (BottomUI.GetComponent<RectTransform>().rect.height / Screen.height);
        float uiPaddingTop = 2 * MainCamera.orthographicSize * (TopPanel.GetComponent<RectTransform>().rect.height / Screen.height);
        // off set is calculated based on the size of the map
        float offset = CurrentGameData.MapSize / 2f - 0.5f;
        CameraMinX = horzExtent - ((float)CurrentGameData.MapSize) / 2.0f + offset;
        CameraMaxX = ((float)CurrentGameData.MapSize) / 2.0f - horzExtent + offset;
        CameraMinY = vertExtent - ((float)CurrentGameData.MapSize) / 2.0f + offset - uiPaddingBot;
        CameraMaxY = ((float)CurrentGameData.MapSize) / 2.0f - vertExtent + offset + uiPaddingTop;
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
                if (curStack != null && curStack.StackData.Owner == CurrentGameData.CurrentPlayer)
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
            Vector3 mousePosClick = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosClick.x = Mathf.Round(mousePosClick.x);
            mousePosClick.y = Mathf.Round(mousePosClick.y);
            mousePosClick.z = 0;

            
            Stack curStack = SelectedUnit.GetComponent<Stack>();
            // check if selected object is a stack
            if (curStack != null)
            {
                // Handle stack merge
                if (mousePosClick == curStack.transform.position)
                {
                    // if there is more stacks on the same field and the stack is selected, merge them
                    RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosClick, Vector2.zero);
                    var possibleMergeStack = new List<Stack>();
                    foreach (RaycastHit2D h in hits)
                    {
                        Stack hitStack = h.transform.GetComponent<Stack>();
                        if (curStack != hitStack)
                        {
                            possibleMergeStack.Add(hitStack);
                        }
                    }
                    // merge the stacks
                    foreach(var stack in possibleMergeStack)
                    {
                        curStack.StackData.Units.InsertRange(stack.StackData.Units.ToEnumerable().ToList());
                        KillStack(stack);
                    }
                }
                
                if (CheckMovementPossible(curStack, CurrentGameData.TerrainTiles[(int)mousePosClick.x, (int)mousePosClick.y]))
                {
                    // assume the stack will move, unless something is hit
                    bool setMove = true;
                    // hit all objects, incase of castle beneath stack
                    // TODO handle multiple stacked stacks
                    RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosClick, Vector2.zero);
                    foreach (RaycastHit2D h in hits)
                    {
                        // check hit componenent
                        Castle cas = h.collider.gameObject.GetComponent<Castle>();
                        Stack hitStack = h.collider.gameObject.GetComponent<Stack>();
                        // check if the castle is hostile then do battle
                        if (cas != null && cas.Owner != curStack.StackData.Owner && (Vector3.Distance(cas.transform.position, curStack.transform.position) < 3f))
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
                                cas.ChangeOwner(curStack.StackData.Owner);
                            }
                            break;
                        }
                        else if (hitStack != null && curStack != null && curStack.StackData.Owner != hitStack.StackData.Owner)
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
                        bool[,] obstructed = generateObstructedFields(curStack, CurrentGameData.AllStacks, AllCastles);

                        // Set stack movement path
                        curStack.Path = AStar.ShortestPath(CurrentGameData.TerrainTiles, obstructed, (int)SelectedUnit.transform.position.x,
                          (int)SelectedUnit.transform.position.y, (int)mousePosClick.x, (int)mousePosClick.y);
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

        foreach (Stack st in CurrentGameData.AllStacks)
        {
            st.NextTurn();
        }

        // change current player
        // Change program turn values
        CurrentGameData.NextTurn();
        TopPanel.GetComponent<TopPanelUI>().SetCurrentPlayer(CurrentGameData.CurrentPlayer);
        // clear selected unit
        SelectedUnit = null;
        BottomUI.GetComponent<BottomUI>().ClearSelectedStack();
    }

    private bool[,] generateObstructedFields(Stack curStack, List<Stack> allStacks, List<Castle> allCastles)
    {
        bool[,] obstructed = new bool[CurrentGameData.MapSize, CurrentGameData.MapSize];
        foreach (Stack st in allStacks)
        {
            if (st.StackData.Owner != curStack.StackData.Owner)
            {
                obstructed[(int)st.transform.position.x, (int)st.transform.position.y] = true;
            }
        }
        foreach (Castle cas in allCastles)
        {
            if (cas.Owner != curStack.StackData.Owner)
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

    private bool CheckMovementPossible(Stack curStack, TerrainTile tile)
    {
        return curStack.Movement >= tile.Movecost;
    }

    private void KillStack(Stack stack)
    {
        CurrentGameData.RemoveStackFromAllStacks(stack);
        Destroy(stack.gameObject);
    }

    /// <summary>
    /// Adds stack to the current game data
    /// </summary>
    /// <param name="stack"></param>
    public void AddStackToAllStacks(Stack stack)
    {
        CurrentGameData.AllStacks.Add(stack);
    }

    public Stack InstantiateStack(Vector3 pos, StackData stackData)
    {
        // instantiate new stack at pos
        GameObject obj = Instantiate(Resources.Load("Prefabs/Stack", typeof(GameObject)),
          pos, Quaternion.identity) as GameObject;
        Stack newStack = obj.GetComponent<Stack>();
        newStack.SetStackData(stackData);
        AddStackToAllStacks(newStack);
        return newStack;
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
                    // In order to not place several castles on top each other check distance
                    // TODO handle this more gracefully
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
