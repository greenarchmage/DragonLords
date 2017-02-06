using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utility;

public class Player
{
  public string Name { get; set; }

  public PriorityQueueMin<Unit> PlayerUnits { get; set; }// TODO should perhaps be unitTypes???
}

