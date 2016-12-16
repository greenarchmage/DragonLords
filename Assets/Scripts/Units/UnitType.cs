using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Units
{
  public class UnitType
  {
    public string Name { get; set; }
    public int Strength { get; set; }
    public int Hits { get; set; }
    public int Speed { get; set; }
    public int Order { get; set; }
    public UnitType(string name, int str, int hit, int spe, int ord)
    {
      Name = name;
      Strength = str;
      Hits = hit;
      Speed = spe;
      Order = ord;
    }
  }
}
