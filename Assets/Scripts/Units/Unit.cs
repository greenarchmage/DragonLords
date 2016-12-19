using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Units;

public class Unit : IComparable<Unit> {

  public int Order { get; set; }
  public string Name { get; set; }
  public int Strength { get; set; }
  public int Hits { get; set; }
  public int Speed { get; set; }
  public string SpriteName { get; set; }
  public Unit() { }
  public Unit(string name, int strength, int hits, int speed, int order, string sprite)
  {
    Name = name;
    Strength = strength;
    Hits = hits;
    Order = order;
    Speed = speed;
    SpriteName = sprite;
  }

  public Unit(UnitType type)
  {
    Name = type.Name;
    Order = type.Order;
    Hits = type.Hits;
    Speed = type.Speed;
    Strength = type.Strength;
    SpriteName = type.SpriteName;
  }

  public int CompareTo(Unit other)
  {
    return Order-other.Order;
  }
}
