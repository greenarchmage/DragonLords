using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
    [System.Serializable]
    public class DataContainer
    {
        public List<UnitTypeSerialization> UnitTypes = new List<UnitTypeSerialization>();
    }
}
