using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;
using Assets.Scripts.Units;

public class Castle : MonoBehaviour {

  private UnitType curProduc;
  public UnitType CurrentProduction { get
    {
      return curProduc;
    }
    set
    {
      curProduc = value;
      currentProductionTime = 0;
    } }
  public Player Owner { get; set; }

  private int currentProductionTime;
  
  public PriorityQueueMin<UnitType> ProductionUnits { get; set; }
  public List<Stack> Garrison { get; set; }
	// Use this for initialization
	void Start () {
    Garrison = new List<Stack>();
    ProductionUnits = new PriorityQueueMin<UnitType>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter2D(Collider2D other)
  {
    Debug.Log("Entering castle " + other.transform.name);
    Stack enterStack = other.gameObject.GetComponent<Stack>();
    if (enterStack != null)
    {
      if (enterStack.Owner == Owner)
      {
        Debug.Log("Add stack to castle");
        Garrison.Add(enterStack);
      }
    }
  }

  void OnTriggerExit2D(Collider2D other)
  {
    Debug.Log("Exiting castle " + other.transform.name);
    Stack enterStack = other.gameObject.GetComponent<Stack>();
    if (enterStack != null)
    {
      if (enterStack.Owner == Owner)
      {
        Debug.Log("Remove stack from castle");
        Garrison.Remove(enterStack);
      }
    }
  }

  public void NextTurn()
  {
    if(CurrentProduction != null && Owner != null)
    {
      currentProductionTime++;
      if (currentProductionTime == CurrentProduction.ProdutionTurns)
      {
        Unit newUnit = new Unit(CurrentProduction);
        // instantiate unit for player
        // TODO spawns two level 1 units
        // TODO handle full castles
        // TODO optimize
        Vector3[] posArr = new Vector3[4];
        posArr[0] = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f);
        posArr[1] = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f);
        posArr[2] = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f);
        posArr[3] = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f);
        int pos = 0;
        bool posFound = false;
        while (!posFound)
        {
          // check if the garrison is full
          for (int i = 0; i < Garrison.Count; i++)
          {
            if (Garrison[i].transform.position == posArr[pos])
            {
              if (Garrison[i].Units.Count == Constants.StackSize)
              {
                // The stack is full, skip to next stack
                // change position
                pos++;
                if(pos == posArr.Length)
                {
                  // TODO well fuck castle is full
                  posFound = true;
                }
                break;
              }
              else
              {
                // There is an unfull stack at the position, add new unit to stack
                Garrison[i].AddUnit(newUnit);
                posFound = true;
              }
            } else
            {
              GameObject obj = Instantiate(Resources.Load("Prefabs/Stack", typeof(GameObject)),
        posArr[pos], Quaternion.identity) as GameObject;
              Stack newStack = obj.GetComponent<Stack>();
              newStack.AddUnit(newUnit);
              newStack.Owner = Owner;
              Garrison.Add(newStack);
              GameController.Instance.AddStack(newStack);
              posFound = true;
            }
          }
          // if garrison < 4, spawn at the reached pos
          if (!posFound && pos > Garrison.Count-1)
          {
            GameObject obj = Instantiate(Resources.Load("Prefabs/Stack", typeof(GameObject)),
        posArr[pos], Quaternion.identity) as GameObject;
            Stack newStack = obj.GetComponent<Stack>();
            newStack.AddUnit(newUnit);
            newStack.Owner = Owner;
            Garrison.Add(newStack);
            GameController.Instance.AddStack(newStack);
            posFound = true;
          }
        }
        
        // reset production time
        currentProductionTime = 0;
      }
    }
  }
}
