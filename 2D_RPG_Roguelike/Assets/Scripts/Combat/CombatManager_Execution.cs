using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            executingUnit.hasActedThisTurn = true;
            executingUnit.GetComponentInChildren<SpriteRenderer>().color = Color.green;

            EndExecutionPhase();
        }

        protected virtual IEnumerator ExecuteCombatAction(CombatAction currentExecutingAction, Unit executingUnit, List<Unit> targets)
        {
            Debug.Log(currentExecutingAction.actionName);
            yield return null;
        }

        protected virtual void EndExecutionPhase()
        {
            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }
    }
}