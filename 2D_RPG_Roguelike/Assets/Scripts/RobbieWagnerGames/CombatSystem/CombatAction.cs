using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.StrategyCombat
{
    public enum ActionType
    {
        NONE,
        ATTACK,
        HEAL
    }

    [CreateAssetMenu(menuName = "Combat Action")]
    public class CombatAction : ScriptableObject
    {
        public string actionName;
        public Sprite actionIcon;
        public ActionType actionType;

        public bool targetsAllOpposition;
        public bool targetsAllAllies;

        public bool canTargetSelf;
        public bool canTargetOpposition;
        public bool canTargetAllies;

        [SerializeReference] public List<ActionEffect> effects;

        [ContextMenu(nameof(AddDamageEffect))] void AddDamageEffect(){effects.Add(new Damage());}
        [ContextMenu(nameof(AddHealEffect))] void AddHealEffect(){effects.Add(new Heal());}
        [ContextMenu(nameof(AddStatRaiseEffect))] void AddStatRaiseEffect(){effects.Add(new StatRaise());}
        [ContextMenu(nameof(AddStatLowerEffect))] void AddStatLowerEffect(){effects.Add(new StatLower());}
        [ContextMenu(nameof(AddPassTurnEffect))] void AddPassTurnEffect(){effects.Add(new Pass());}
        [ContextMenu(nameof(Clear))] void Clear(){effects.Clear();}
    }
}