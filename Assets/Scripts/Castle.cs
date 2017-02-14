using UnityEngine;
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
    //Debug.Log("Entering castle " + other.transform.name);
    Stack enterStack = other.gameObject.GetComponent<Stack>();
    if (enterStack != null)
    {
      if (enterStack.Owner == Owner)
      {
        //Debug.Log("Add stack to castle");
        Garrison.Add(enterStack);
      }
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    //Debug.Log("Exiting castle " + other.transform.name);
    Stack enterStack = other.gameObject.GetComponent<Stack>();
    if (enterStack != null)
    {
      if (enterStack.Owner == Owner)
      {
        //Debug.Log("Remove stack from castle");
        Garrison.Remove(enterStack);
      }
    }
  }

  public void NextTurn()
  {
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

    if (CurrentProduction != null && Owner != null)
    {
      currentProductionTime++;
      if (currentProductionTime == CurrentProduction.ProdutionTurns)
      {
        Unit newUnit = new Unit(CurrentProduction);

        // reset production time
        currentProductionTime = 0;

        // Algorithm for unit placement
        bool unitPlaced = false;
        int counter = 0;
        Vector3 initPos = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f);
        Vector3 curPos = initPos;
        // info for outside loop
        bool outsideSet = false;
        int direction = 1; // the sign of the direction the loop goes
        int xi = 0;
        int yj = 0; // has to be 3 for the first run
        while (!unitPlaced)
        {
          #region insideCastle
          if (counter < 3)
          {
            curPos = initPos;
            // still inside the castle
            for (int y = 0; y < 2; y++)
            {
              bool posChange = false;
              for (int x = 0; x < 2; x++)
              {
                curPos = new Vector3(initPos.x + x, initPos.y - y);
                //check if garrison is at position
                for (int i = 0; i < Garrison.Count; i++)
                {
                  if (Garrison[i].transform.position == curPos)
                  {
                    if (Garrison[i].Units.Count == Constants.StackSize)
                    {
                      // The stack is full, skip to next stack
                      // change position
                      posChange = true;
                      break;
                    }
                    else
                    {
                      // There is an unfull stack at the position, add new unit to stack
                      Garrison[i].AddUnit(newUnit);
                      unitPlaced = true;
                      break;
                    }
                  }
                }
                // if unit is added break x increment
                if (unitPlaced)
                {
                  break;
                }
                // some check to see if loop has not been completed
                if (posChange)
                {
                  counter++;
                  posChange = false;
                }
                else
                {
                  // instantiate new stack at pos
                  GameObject obj = Instantiate(Resources.Load("Prefabs/Stack", typeof(GameObject)),
                    curPos, Quaternion.identity) as GameObject;
                  Stack newStack = obj.GetComponent<Stack>();
                  newStack.AddUnit(newUnit);
                  newStack.Owner = Owner;
                  Garrison.Add(newStack);
                  GameController.Instance.AddStackToAllStacks(newStack);
                  unitPlaced = true;
                  break;
                }
              }
              // if unit is placed break y increment
              if (unitPlaced)
              {
                break;
              }
            }
          }
          #endregion
          else
          {
            // skips to the akward position
            if (!outsideSet)
            {
              curPos = initPos;
              curPos.y += -2;
              outsideSet = true;
            }
            // The castle is full, check the rest of the map
            
            if ((xi+1) < counter)
            {
              bool posChange = false;
              #region CheckPos
              //check if garrison is at position
              foreach (Stack st in GameController.Instance.AllStacks)
              {
                if (st.transform.position == curPos)
                {
                  if (st.Units.Count == Constants.StackSize)
                  {
                    // The stack is full, skip to next stack
                    // change position
                    posChange = true;
                    break;
                  }
                  else
                  {
                    // There is an unfull stack at the position, add new unit to stack
                    Debug.Log("Add unit to existing stack outside city");
                    st.AddUnit(newUnit);
                    unitPlaced = true;
                    break;
                  }
                }
              }
              // if unit is added break all
              if (unitPlaced)
              {
                break;
              }
              // some check to see if loop has not been completed
              if (posChange)
              {
                Debug.Log("Changing pos");
                xi++;
                // change curPos
                curPos.x += 1 * direction;
                Debug.Log("Pos " + curPos.ToString());
              }
              else
              {
                // instantiate new stack at pos
                GameObject obj = Instantiate(Resources.Load("Prefabs/Stack", typeof(GameObject)),
                  curPos, Quaternion.identity) as GameObject;
                Stack newStack = obj.GetComponent<Stack>();
                newStack.AddUnit(newUnit);
                newStack.Owner = Owner;
                Garrison.Add(newStack);
                GameController.Instance.AddStackToAllStacks(newStack);
                unitPlaced = true;
                break;
              }
              #endregion
            } else if ((yj+1) < counter)
            #region outsideCastle
            {
              Debug.Log("Y change");
              bool posChange = false;
             
              //check if garrison is at position
              // TODO change AllStacks collection to be coordinate based
              foreach (Stack st in GameController.Instance.AllStacks)
              {
                if (st.transform.position == curPos)
                {
                  if (st.Units.Count == Constants.StackSize)
                  {
                    // The stack is full, skip to next stack
                    // change position
                    posChange = true;
                    break;
                  }
                  else
                  {
                    // There is an unfull stack at the position, add new unit to stack
                    st.AddUnit(newUnit);
                    unitPlaced = true;
                  }
                }
              }
              // some check to see if loop has not been completed
              if (posChange)
              {
                yj++;
                // change curPos
                curPos.y += 1 * direction;
              }
              else
              {
                // instantiate new stack at pos
                Debug.Log("Place y");
                GameObject obj = Instantiate(Resources.Load("Prefabs/Stack", typeof(GameObject)),
                  curPos, Quaternion.identity) as GameObject;
                Stack newStack = obj.GetComponent<Stack>();
                newStack.AddUnit(newUnit);
                newStack.Owner = Owner;
                Garrison.Add(newStack);
                GameController.Instance.AddStackToAllStacks(newStack);
                unitPlaced = true;
              }
            }
            else
            {
              yj = 0;
              xi = 0;
              direction *= -1;
              counter++;
            }
            #endregion
          }
        }
      }
    }
  }
}
