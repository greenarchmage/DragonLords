using UnityEngine;

namespace Assets.Scripts.Serialization
{
    [System.Serializable]
    public class UnitTypeSerialization 
    {
        public string Name;
        public int Strength;
        public int Hits;
        public int Speed;
        public int Order;
        public string SpriteName;
        public int Price;
        public int ProductionTurns;
        public TerrainTile.MoveType MoveType;

        public UnitTypeSerialization() { }
        public UnitTypeSerialization(string name, int strength, int hits, int speed, int order, string spriteName, int price, int productionTurns, TerrainTile.MoveType moveType)
        {
            Name = name;
            Strength = strength;
            Hits = hits;
            Speed = speed;
            Order = order;
            SpriteName = spriteName;
            Price = price;
            ProductionTurns = productionTurns;
            MoveType = moveType;
        }
    }
}
