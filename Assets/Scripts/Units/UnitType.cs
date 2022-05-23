using System;

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
        public int ProdutionTurns { get; set; }
        public TerrainTile.MoveType MoveType { get; set; }
        public UnitType(string name, int strength, int hits, int speed, int order, string spriteName, int price, int productionTurns, TerrainTile.MoveType moveType)
        {
            Name = name;
            Strength = strength;
            Hits = hits;
            Speed = speed;
            Order = order;
            SpriteName = spriteName;
            Price = price;
            ProdutionTurns = productionTurns;
            MoveType = moveType;
        }

        public int CompareTo(UnitType other)
        {
            return Order - other.Order;
        }
    }
}
