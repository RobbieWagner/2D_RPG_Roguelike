using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;

namespace RobbieWagnerGames.TurnBasedCombat
{
    /// <summary>
    /// Handles the ACTION_SELECTION phase of combat
    /// </summary>
    public partial class CombatManagerBase : MonoBehaviour
    {
        [Space(10)]
        [Header("Action Selection")]
        [SerializeField] protected CombatSelectionUI selectionUI;
        protected Unit selectingUnit;
        protected CombatAction currentSelectedAction = null;
        protected List<Unit> targets = null;
        protected bool isCurrentlySelecting;

        protected virtual IEnumerator HandleSelectionPhase()
        {
            if (selectingUnit.GetType() == typeof(Ally))
                selectionUI.SetupUI((Ally)selectingUnit);
            else if (selectingUnit.GetType() == typeof(Enemy))
                SelectActionForUnit(selectingUnit, true);
            else
                throw new InvalidOperationException("Could not complete action selection phase: type of unit is not valid for combat!!");

            while(currentSelectedAction == null && (targets == null || !targets.Any()) && isCurrentlySelecting)
            {
                yield return null;
            }

            EndSelectionPhase();
        }

        private void SelectActionForUnit(Unit selectingUnit, bool v)
        {
            throw new NotImplementedException();
        }

        protected virtual void EndSelectionPhase()
        {
            CurrentCombatPhase = CombatPhase.ACTION_EXECUTION;
        }
    }
}