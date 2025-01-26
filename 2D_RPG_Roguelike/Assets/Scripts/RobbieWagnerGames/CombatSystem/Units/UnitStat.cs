using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;

namespace RobbieWagnerGames.StrategyCombat
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StatType
    {
        NONE = -1,
        BRAWN = 0,
        AGILITY = 1,
        FOCUS = 2,
        HEART = 3,
        PSYCH = 4,
    }

    [Serializable]
    public class UnitStat
    {
        [SerializeField] private StatType unitStatType;
        [SerializeField] private int baseValue;
        [SerializeField] private int currentValue;

        [JsonIgnore] public Action<UnitStat> OnStatSet = (UnitStat changedStat) => { };

        public StatType UnitStatType;

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
            set 
            {
                if (value == baseValue) return;

                baseValue = value;
            }
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