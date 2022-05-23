using Assets.Scripts.Serialization;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class GameDataContainer
    {
        public List<UnitType> UnitTypes { get; set; }

        public GameDataContainer()
        {

        }

        public void LoadUnitTypes()
        {
            // Test to verify structure. Should be removed 
            DataContainer dataContainerTest = new DataContainer()
            {
                UnitTypes = new List<UnitTypeSerialization>()
                {
                    new UnitTypeSerialization()
                    {
                        Name = "Heavy Infantry",
                        Hits = 2,
                        MoveType = TerrainTile.MoveType.Normal,
                        Order = 1,
                        Price = 80,
                        ProductionTurns = 1,
                        Strength = 3,
                        Speed = 16,
                        SpriteName = "HeavyInfantry",
                    }
                }
            };
            string jsonParseString = JsonUtility.ToJson(dataContainerTest);

            // Load all the unit types in the TestLevelData.json
            TextAsset testLevelData = (TextAsset)Resources.Load("JsonData/TestLevelData");

            // Assign the deserialized Values
            var dataContainer = JsonUtility.FromJson<DataContainer>(testLevelData.text);
            UnitTypes = dataContainer.UnitTypes.Select(uts => uts.ToUnitType()).ToList();
        }
    }
}
