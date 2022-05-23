using Assets.Scripts.Units;

namespace Assets.Scripts.Serialization
{
    public static class UnitTypeSerializationExtension
    {
        public static UnitType ToUnitType(this UnitTypeSerialization unitTypeSerilization)
        {
            var unitType = new UnitType(
                unitTypeSerilization.Name,
                unitTypeSerilization.Strength,
                unitTypeSerilization.Hits,
                unitTypeSerilization.Speed,
                unitTypeSerilization.Order,
                unitTypeSerilization.SpriteName,
                unitTypeSerilization.Price,
                unitTypeSerilization.ProductionTurns,
                unitTypeSerilization.MoveType
                );
            return unitType;
        }
    }
}
