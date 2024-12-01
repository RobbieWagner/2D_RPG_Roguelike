using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CombatItemEffect
{
    [Header("General Info")]
    [SerializeField] public bool usableOutsideCombat;
    [SerializeField] private bool failureStopsEffectExecution;
    public bool FailureStopsEffectExecution
    {
        get { return failureStopsEffectExecution; }
        private set { failureStopsEffectExecution = value; }
    }
}

[Serializable]
public class HealItemEffect : CombatItemEffect
{
    public int healAmount;
}

[Serializable]
public class ReviveItemEffect : CombatItemEffect
{
    public int healAmount;
}

[Serializable]
public class ReplenishStatItemEffect : CombatItemEffect
{
    public List<UnitStat> statsToReplenish;
    public int amount;
}
