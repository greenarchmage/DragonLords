﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using Assets.Scripts.Units;

public class Castle : MonoBehaviour
{

    private UnitType curProduc;
    public UnitType CurrentProduction
    {
        get
        {
            return curProduc;
        }
        set
        {
            curProduc = value;
            currentProductionTime = 0;
        }
    }
    public Player Owner { get; set; }
    public int Income { get; set; }

    private int currentProductionTime;

    public PriorityQueueMin<UnitType> ProductionUnits { get; set; }
    public List<Stack> Garrison { get; set; }
    // Use this for initialization
    void Start()
    {
        Garrison = new List<Stack>();
        ProductionUnits = new PriorityQueueMin<UnitType>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Stack enterStack = other.gameObject.GetComponent<Stack>();
        if (enterStack != null)
        {
            if (enterStack.StackData.Owner == Owner)
            {
                Garrison.Add(enterStack);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Stack enterStack = other.gameObject.GetComponent<Stack>();
        if (enterStack != null)
        {
            if (enterStack.StackData.Owner == Owner)
            {
                Garrison.Remove(enterStack);
            }
        }
    }

    /// <summary>
    /// TODO move out of UI class
    /// </summary>
    public void NextTurn()
    {
        // clean garrision
        // TODO use grid
        List<Stack> garRemove = new List<Stack>();
        for (int i = 0; i < Garrison.Count; i++)
        {
            if (Garrison[i] == null)
            {
                garRemove.Add(Garrison[i]);
            }
        }
        foreach (Stack st in garRemove)
        {
            Garrison.Remove(st);
        }

        // Castle production
        if (CurrentProduction != null && Owner != null)
        {
            currentProductionTime++;
            if (currentProductionTime == CurrentProduction.ProdutionTurns)
            {
                // reset production time
                currentProductionTime = 0;

                PlaceUnit();
            }
        }

        // increase player income
        if (Owner != null)
        {
            Owner.Gold += Income;
        }
    }

    public void ChangeOwner(Player own)
    {
        Owner = own;
        CurrentProduction = null;
        currentProductionTime = 0;
        ProductionUnits.Clear();
    }

    private void PlaceUnit()
    {
        // Algorithm for unit placement
        bool unitPlaced = false;
        Vector3 initPos = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f);
        Vector3 curPos = initPos;
        // info for outside loop
        bool outsideSet = false;
        bool outsideFirst = true;
        int direction = 1; // the sign of the direction the loop goes
        int n = 0;
        int armLength = 2;
        int coordChange = 0;

        bool inside = true;
        while (!unitPlaced)
        {
            var gameDate = GameController.Instance.CurrentGameData;
            if ((int)curPos.x >= gameDate.MapSize && (int)curPos.y >= gameDate.MapSize)
            {
                // the position is outside the map
                //TODO handle castles not being centered
                break;
            }
            #region insideCastle
            if (inside)
            {
                Vector3 insidePos = initPos;
                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        insidePos.x = initPos.x + x;
                        // the placement goes down, hence -y
                        insidePos.y = initPos.y - y;
                        // check pos inside
                        unitPlaced = CheckInside(insidePos);
                        if (unitPlaced)
                        {
                            break;
                        }
                    }
                    if (unitPlaced)
                    {
                        break;
                    }
                }
                inside = false;
            }
            #endregion
            else
            {
                #region outsideCastle
                // The castle is full, check the rest of the map
                // skips to the akward position
                if (!outsideSet)
                {
                    curPos = initPos;
                    curPos.y += -2;
                    outsideSet = true;
                }

                // checks initial place
                if (outsideFirst)
                {
                    unitPlaced = CheckOutside(curPos);
                    outsideFirst = false;
                    if (unitPlaced)
                    {
                        break;
                    }
                }

                for (int i = 0; i < armLength; i++)
                {
                    curPos[coordChange] += direction;
                    unitPlaced = CheckOutside(curPos);
                    if (unitPlaced)
                    {
                        break;
                    }
                }
                coordChange = coordChange == 0 ? 1 : 0;
                if ((n + 1) % 2 == 0)
                {
                    direction *= -1;
                }
                else
                {
                    armLength++;
                }
                n++;
                #endregion
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos">Position in the castle</param>
    /// <returns>if the unit is placed</returns>
    private bool CheckInside(Vector3 pos)
    {
        Unit newUnit = new Unit(CurrentProduction);
        for (int i = 0; i < Garrison.Count; i++)
        {
            if (Garrison[i].transform.position == pos)
            {
                if (Garrison[i].StackData.Units.Count == Constants.StackSize)
                {
                    // The stack is full, skip to next stack
                    // change position
                    return false;
                }
                else
                {
                    // There is an unfull stack at the position, add new unit to stack
                    Garrison[i].AddUnit(newUnit);
                    return true;
                }
            }
        }
        // instantiate new stack at pos

        InstantiateUnit(pos, newUnit);
        return true;
    }

    private bool CheckOutside(Vector3 pos)
    {
        Unit newUnit = new Unit(CurrentProduction);
        foreach (Stack st in GameController.Instance.CurrentGameData.AllStacks)
        {
            if (st.transform.position == pos)
            {
                if (st.StackData.Units.Count == Constants.StackSize)
                {
                    // The stack is full, skip to next stack
                    // change position
                    return false;
                }
                else
                {
                    // There is an unfull stack at the position, add new unit to stack
                    st.AddUnit(newUnit);
                    return true;
                }
            }
        }
        var gameDate = GameController.Instance.CurrentGameData;
        if ((int)pos.x >= gameDate.MapSize || (int)pos.y >= gameDate.MapSize)
        {
            return false;
        }
        TerrainTile.MoveType currentTile = gameDate.TerrainTiles[(int)pos.x, (int)pos.y].MoveGroup;
        if (currentTile == TerrainTile.MoveType.Impassable ||
          (currentTile == TerrainTile.MoveType.Flying || currentTile == TerrainTile.MoveType.WaterPassable) && CurrentProduction.MoveType != TerrainTile.MoveType.Flying)
        {
            return false;
        }
        // instantiate new stack at pos
        InstantiateUnit(pos, newUnit);
        return true;
    }

    private void InstantiateUnit(Vector3 pos, Unit newUnit)
    {
        // instantiate new stack at pos
        var stackData = new StackData()
        {
            Owner = Owner,
            Units = new PriorityQueueMin<Unit>(),
        };
        stackData.Units.Insert(newUnit);
        var newStack = GameController.Instance.InstantiateStack(pos, stackData);
        Garrison.Add(newStack);
    }
}
