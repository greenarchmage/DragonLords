using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Units
{
    public class StackData
    {
        public Player Owner { get; set; }
        public PriorityQueueMin<Unit> Units { get; set; }

        public StackData()
        {
            Units = new PriorityQueueMin<Unit>();
        }
    }
}
