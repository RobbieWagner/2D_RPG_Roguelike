using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum CombatItemType
    {
        HEALING, // Healing
        POWER, // Stat raisers
        TRAP, // Potentially harm opponents
        TOOL, // Weapons with special properties, etc.
    }

    [CreateAssetMenu(menuName = "Game Item/Combat")]
    public class CombatItem : GameItem
    {
        public CombatItemType combatItemType;
        [SerializeReference] public List<CombatItemEffect> effects;

        [ContextMenu(nameof(AddHealingEffect))] void AddHealingEffect() { effects.Add(new HealItemEffect()); }
        [ContextMenu(nameof(AddReviveEffect))] void AddReviveEffect() { effects.Add(new ReviveItemEffect()); }
        [ContextMenu(nameof(AddReplenishStatEffect))] void AddReplenishStatEffect() { effects.Add(new ReplenishStatItemEffect()); }
        [ContextMenu(nameof(Clear))] void Clear() { effects.Clear(); }
    }
}