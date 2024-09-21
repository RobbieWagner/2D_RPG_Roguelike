using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{

    public enum CombatPhase
    {
        NONE,
        SETUP,
        ACTION_SELECTION,
        ACTION_EXECUTION,
        WIN,
        LOSE
    }

    public partial class CombatManagerBase : MonoBehaviour
    {
        public static Action OnCombatPhaseChange;

        private CombatPhase currentCombatPhase = CombatPhase.NONE;
        public CombatPhase CurrentCombatPhase
        {
            get 
            {
                return currentCombatPhase;
            }
            set 
            {
                if(currentCombatPhase == value)
                    return;
                currentCombatPhase = value;

            }
        }

        public static CombatManagerBase Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

    }
}