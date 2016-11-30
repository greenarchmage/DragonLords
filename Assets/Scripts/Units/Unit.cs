using UnityEngine;
using System.Collections;

public class Unit {

  public enum UnitType
  {
    Hero, Creature
  }

  public string Name { get; set; }
  public int Strength { get; set; }
  public int Hits { get; set; }
  public int Speed { get; set; }
  public Unit() { }
  public Unit(int strength, int hits)
  {
    Strength = strength;
    Hits = hits;
  }
}
