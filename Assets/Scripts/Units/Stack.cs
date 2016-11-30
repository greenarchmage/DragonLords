using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack : MonoBehaviour
{
  public Player Owner { get; set; }
  public int StackSize { get; set; }
  public List<Unit> Units { get { return units; } set { units = value; } }
  public int Movement { get; set; }

  public List<int[]> Path { get; set; }

  private List<Unit> units = new List<Unit>();

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(Path != null && Path.Count != 0)
    {
      Vector3 newpos = new Vector3(Path[0][0], Path[0][1]);
      transform.position = Vector3.MoveTowards(transform.position, newpos,1);
      //transform.position = newpos;
      Path.Remove(Path[0]);
    }
  }


  public void SetStackStartMovement()
  {
    for(int i = 0; i < units.Count; i++)
    {
      Movement = Movement > units[i].Speed ? units[i].Speed : Movement;
    }
  }

  public bool Battle(Stack hitStack)
  {
    Debug.Log("Battle starting");
    // Select first unit in cur stack, select first unit in hit stack
    // Do damage roll, till one dies
    // Select next unit in stack where unit died, if null, the other stack wins
    while (units.Count != 0 && hitStack.Units.Count != 0)
    {
      //TODO should be done with a priority queue
      int curUnitInt = 0;
      Unit curStackUnit = units[curUnitInt];
      int hitUnitInt = 0;
      Unit hitStackUnit = hitStack.Units[hitUnitInt];

      //Battle section
      //TODO add special abilities
      while (hitStackUnit.Hits > 0 && curStackUnit.Hits > 0)
      {
        int hitUnitRoll = Random.Range(0, GameController.DiceRange) + 1 + hitStackUnit.Strength;
        int curUnitRoll = Random.Range(0, GameController.DiceRange) + 1 + curStackUnit.Strength;

        int fightRes = curUnitRoll - hitUnitRoll;

        if (fightRes > 0)
        {
          hitStackUnit.Hits -= 1;
        }
        else if (fightRes < 0)
        {
          curStackUnit.Hits -= 1;
        }
      }

      if (hitStackUnit.Hits == 0)
      {
        Debug.Log("Hit stack lost unit");
        hitStack.Units.Remove(hitStackUnit);
        if (hitStack.Units.Count == 0)
        {
          return true;
        }
      }
      else
      {
        Debug.Log("Current stack lost unit");
        units.Remove(curStackUnit);
        if (units.Count == 0)
        {
          return false;
        }
      }
    }
    return true;
  }



  //public Stack() { }
  //public Stack(int stackSize)
  //{
  //  StackSize = stackSize;
  //  Units = new List<Unit>(stackSize);
  //}
}
