using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.Utility;
using Assets.Scripts.Units;
using System.Linq;

public class Stack : MonoBehaviour
{
    public StackData StackData { get; set; }
    public int StackSize;

    public int Movement { get; set; }

    public List<PathNode> Path { get; set; }

    // Use this for initialization
    void Start()
    {
        StackData = new StackData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Path != null && Path.Count != 0)
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
                var movementPrice = Path[0].Cost;
                Movement -= movementPrice;
                UpdateUnitMovement(-movementPrice);
                Path.Remove(Path[0]);
            }
        }
    }

    private void UpdateUnitMovement(int movementChange)
    {
        // TODO handle different cost for different units
        foreach(var unit in StackData.Units)
        {
            unit.RemainingMovement += movementChange;
        }
    }

    /// <summary>
    /// Should be used to insert all units, to ensure the sprite is changed
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(Unit unit)
    {
        // Set stack appearance
        StackData.Units.Insert(unit);
        Unit highestOrder = unit;
        foreach (Unit u in StackData.Units)
        {
            if (u.Order > highestOrder.Order)
            {
                highestOrder = u;
            }
        }
        SetVisuals(highestOrder.SpriteName);
        StackSize = StackData.Units.Count;
    }

    private void SetVisuals(string unitSpriteName)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("UnitSprites/" + unitSpriteName, typeof(Sprite)) as Sprite;
        if(StackData.Owner != null)
        {
            var banner = Resources.Load("UnitSprites/" + StackData.Owner.BanneSpriteName, typeof(Sprite)) as Sprite;
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = banner;
        }
    }

    private void SetStackStartMovement()
    {
        Movement = int.MaxValue;
        foreach (Unit u in StackData.Units)
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
        while (StackData.Units.Count != 0 && hitStack.StackData.Units.Count != 0)
        {
            Unit curStackUnit = StackData.Units.Min();
            Unit hitStackUnit = hitStack.StackData.Units.Min();

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
                hitStack.StackData.Units.DelMin();//.Remove(hitStackUnit);
                if (hitStack.StackData.Units.Count == 0)
                {
                    return true;
                }
            }
            else
            {
                Debug.Log("Current stack lost unit");
                StackData.Units.DelMin();// Remove(curStackUnit);
                if (StackData.Units.Count == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SetStackData(StackData stackData)
    {
        StackData = stackData;
        Unit highestOrder = StackData.Units.First();
        foreach (Unit u in StackData.Units)
        {
            if (u.Order > highestOrder.Order)
            {
                highestOrder = u;
            }
        }
        SetVisuals(highestOrder.SpriteName);
        StackSize = StackData.Units.Count;
    }
}
