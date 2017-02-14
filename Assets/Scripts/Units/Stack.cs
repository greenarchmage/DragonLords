using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.Utility;
using Assets.Scripts.Units;

public class Stack : MonoBehaviour
{
  public Player Owner { get; set; }
  public int StackSize;
  /// <summary>
  /// Should not be used to insert units
  /// </summary>
  public PriorityQueueMin<Unit> Units { get { return units; } set { units = value; } }
  public int Movement { get; set; }

  public List<PathNode> Path { get; set; }

  private PriorityQueueMin<Unit> units = new PriorityQueueMin<Unit>();

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(Path != null && Path.Count != 0)
    {
      // clear path if you can't move to next target
      if (Path.Count > 0 && Movement - Path[0].Cost < 0 || Movement == 0)
      {
        Path = null;
      }
    }
    if (Path != null && Path.Count != 0 && Movement > 0)
    {
      // move towards current target
      Vector3 newpos = new Vector3(Path[0].Coord[0], Path[0].Coord[1]);
      transform.position = Vector3.MoveTowards(transform.position, newpos, 0.05f);

      // when target is reached, check if you can move to next target
      if (Vector3.Distance(transform.position, new Vector3(Path[0].Coord[0], Path[0].Coord[1])) <= 0)
      {
        Movement -= Path[0].Cost;
        Path.Remove(Path[0]);
      }
    }
  }

  /// <summary>
  /// Should be used to insert all units, to ensure the sprite is changed
  /// </summary>
  /// <param name="unit"></param>
  public void AddUnit(Unit unit)
  {
    // Set stack appearance
    units.Insert(unit);
    Unit heighestOrder = unit;
    foreach(Unit u in units)
    {
      if(u.Order > heighestOrder.Order)
      {
        heighestOrder = u;
      }
    }
    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("UnitSprites/" + heighestOrder.SpriteName, typeof(Sprite)) as Sprite;
    StackSize = Units.Count;
  }
  private void SetStackStartMovement()
  {
    Movement = int.MaxValue;
    foreach (Unit u in units)
    {
      Movement = Movement > u.Speed ? u.Speed : Movement;
    }
  }

  public void NextTurn()
  {
    SetStackStartMovement();
  }

  public bool Battle(Stack hitStack)
  {
    Debug.Log("Battle starting");
    // Select first unit in cur stack, select first unit in hit stack
    // Do damage roll, till one dies
    // Select next unit in stack where unit died, if null, the other stack wins
    while (units.Count != 0 && hitStack.Units.Count != 0)
    {
      Unit curStackUnit = units.Min();
      Unit hitStackUnit = hitStack.Units.Min();

      //Battle section
      //TODO add special abilities
      while (hitStackUnit.Hits > 0 && curStackUnit.Hits > 0)
      {
        int hitUnitRoll = Random.Range(0, Constants.DiceRange) + 1 + hitStackUnit.Strength;
        int curUnitRoll = Random.Range(0, Constants.DiceRange) + 1 + curStackUnit.Strength;

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
        hitStack.Units.DelMin();//.Remove(hitStackUnit);
        if (hitStack.Units.Count == 0)
        {
          return true;
        }
      }
      else
      {
        Debug.Log("Current stack lost unit");
        units.DelMin();// Remove(curStackUnit);
        if (units.Count == 0)
        {
          return false;
        }
      }
    }
    return true;
  }
}
