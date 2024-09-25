using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using RobbieWagnerGames.StrategyCombat;

namespace RobbieWagnerGames.TurnBasedCombat
{
    /// <summary>
    /// Handles the ACTION_EXECUTION phase of combat
    /// </summary>
    public partial class CombatManagerBase : MonoBehaviour
    {
        [Space(10)]
        [Header("Action Execution")]
        [SerializeField] protected CombatExecutionUI executionUI;
        protected Unit executingUnit;
        protected CombatAction currentExecutingAction;

        protected virtual IEnumerator HandleExecutionPhase()
        {
            executingUnit = selectingUnit;
            currentExecutingAction = currentSelectedAction;

            yield return StartCoroutine(ExecuteCombatAction(currentExecutingAction, executingUnit, targets));

            yield return new WaitForSeconds(2f);

            EndExecutionPhase();
        }

        protected virtual IEnumerator ExecuteCombatAction(CombatAction currentExecutingAction, Unit executingUnit, List<Unit> targets)
        {
            throw new NotImplementedException();
        }

        protected virtual void EndExecutionPhase()
        {
            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }
    }
}