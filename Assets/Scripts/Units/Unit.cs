using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Units;

namespace Assets.Scripts.Units
{
    public class Unit : IComparable<Unit>
    {

        public int Order { get; private set; }
        public string Name { get; private set; }
        public int Strength { get; private set; }
        // TODO make private
        public int Hits { get; set; }
        public int Speed { get; private set; }
        public string SpriteName { get; private set; }
        public int RemainingMovement { get; set; }

        public Unit(string name, int strength, int hits, int speed, int order, string sprite)
        {
            Name = name;
            Strength = strength;
            Hits = hits;
            Order = order;
            Speed = speed;
            SpriteName = sprite;
        }

        public Unit(UnitType type)
        {
            Name = type.Name;
            Order = type.Order;
            Hits = type.Hits;
            Speed = type.Speed;
            Strength = type.Strength;
            SpriteName = type.SpriteName;
            RemainingMovement = type.Speed;
        }

        public int CompareTo(Unit other)
        {
            return Order - other.Order;
        }
    }
}