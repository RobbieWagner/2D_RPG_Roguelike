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
        SPECIAL, // Potentially harm opponents, or does some other effect to the combat
    }

    [CreateAssetMenu(menuName = "Game Item/Combat")]
    public class CombatItem : GameItem
    {
        public CombatItemType combatItemType;
        [SerializeReference] public List<ActionEffect> effects;

        public bool targetsAllOpposition;
        public bool targetsAllAllies;

        public bool canTargetSelf;
        public bool canTargetOpposition;
        public bool canTargetAllies;

        public bool canTargetDownedAllies;

        [ContextMenu(nameof(AddHealingEffect))] void AddHealingEffect() { effects.Add(new Heal()); }
        [ContextMenu(nameof(AddReviveEffect))] void AddReviveEffect() { effects.Add(new Revive()); }
        [ContextMenu(nameof(AddReplenishStatEffect))] void AddReplenishStatEffect() { effects.Add(new StatRaise()); }
        [ContextMenu(nameof(Clear))] void Clear() { effects.Clear(); }
    }
}