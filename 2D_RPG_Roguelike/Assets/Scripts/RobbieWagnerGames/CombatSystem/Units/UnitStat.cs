using System;
using UnityEngine;

namespace RobbieWagnerGames.StrategyCombat
{
    [Serializable]
    public enum StatType
    {
        BRAWN = 0,
        AGILITY = 1,
        LORE = 2,
        HEART = 3,
        OCCULT = 4,
    }

    [Serializable]
    public class UnitStat
    {
        [SerializeField] private StatType unitStatType;
        [SerializeField] private int baseValue;
        [SerializeField] private int currentValue;

        public Action<UnitStat> OnStatSet = (UnitStat changedStat) => { };

        public StatType UnitStatType { get => unitStatType; protected set { unitStatType = value; } }

        public int CurrentValue
        {
            get { return currentValue; }
            set
            {
                if (value == currentValue) return;

                currentValue = value;
                OnStatSet?.Invoke(this);
            }
        }

        public int BaseValue
        {
            get => baseValue;
            private set { }
        }


        public UnitStat(StatType statType, int baseStatValue = 10)
        {
            UnitStatType = statType;
            baseValue = baseStatValue;
            currentValue = baseValue;
        }

        public UnitStat() { }
    }
}