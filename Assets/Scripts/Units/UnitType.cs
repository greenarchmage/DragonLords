﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Units
{
  public class UnitType : IComparable<UnitType>
  {
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Hits { get; set; }
    public int Speed { get; set; }
    public int Order { get; set; }
    public string SpriteName { get; set; }
    public int Price { get; set; }
    public UnitType(string name, int str, int hit, int spe, int ord, string sprit, int price)
    {
      Name = name;
      Strength = str;
      Hits = hit;
      Speed = spe;
      Order = ord;
      SpriteName = sprit;
      Price = price;
    }

    public int CompareTo(UnitType other)
    {
      return Order - other.Order;
    }
  }
}
