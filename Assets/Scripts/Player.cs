using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Units;
using Assets.Scripts.Utility;

public class Player
{
    public string Name { get; set; }

    public PriorityQueueMin<UnitType> PlayerUnits { get; set; }

    public int Gold { get; set; }

    public string BanneSpriteName { get; set; }
}

