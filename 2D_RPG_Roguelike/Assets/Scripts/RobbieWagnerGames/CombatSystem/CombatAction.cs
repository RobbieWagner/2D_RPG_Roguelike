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
        HEAL,
        STAT_RAISE,
        STAT_LOWER,
        PASS
    }

    [CreateAssetMenu(menuName = "Combat Action")]
    public class CombatAction : ScriptableObject
    {
        public string actionName;
        public Sprite actionIcon;
        public ActionType actionType;
        [TextArea] public string description;

        public bool targetsAllOpposition;
        public bool targetsAllAllies;

        public bool canTargetSelf;
        public bool canTargetOpposition;
        public bool canTargetAllies;

        [SerializeReference] public List<ActionEffect> effects;

        [ContextMenu(nameof(AddAttackEffect))] void AddAttackEffect(){effects.Add(new Attack());}
        [ContextMenu(nameof(AddHealEffect))] void AddHealEffect(){effects.Add(new Heal());}
        [ContextMenu(nameof(AddStatRaiseEffect))] void AddStatRaiseEffect(){effects.Add(new StatRaise());}
        [ContextMenu(nameof(AddStatLowerEffect))] void AddStatLowerEffect(){effects.Add(new StatLower());}
        [ContextMenu(nameof(AddPassTurnEffect))] void AddPassTurnEffect(){effects.Add(new Pass());}
        [ContextMenu(nameof(Clear))] void Clear(){effects.Clear();}
    }
}