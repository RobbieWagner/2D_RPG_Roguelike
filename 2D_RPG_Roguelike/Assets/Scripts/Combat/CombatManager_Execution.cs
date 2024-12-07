using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        protected CombatItem currentConsumingItem;
        private bool continueExecution = true;

        protected virtual IEnumerator HandleExecutionPhase()
        {
            if (CheckForCombatCompletion())
                yield break;

            executingUnit = selectingUnit;

            if (currentSelectedAction != null)
            {
                currentExecutingAction = currentSelectedAction;
                yield return StartCoroutine(ExecuteCombatAction(currentExecutingAction, executingUnit, targets));
            }
            else if(currentSelectedItem != null)
            {
                currentConsumingItem = currentSelectedItem;
                yield return StartCoroutine(ConsumeCombatItem(currentConsumingItem, executingUnit, targets));
            }

            executingUnit.hasActedThisTurn = true;
            executingUnit.GetComponentInChildren<SpriteRenderer>().color = Color.green;

            EndExecutionPhase();
        }

        #region Combat Action
        protected virtual IEnumerator ExecuteCombatAction(CombatAction combatAction, Unit executingUnit, List<Unit> targets)
        {
            //Debug.Log(combatAction.actionName);
            yield return StartCoroutine(executionUI.DisplayExecutingAction(combatAction));
            continueExecution = true;

            foreach (ActionEffect effect in combatAction.effects)
            {
                if(!continueExecution)
                    break;

                if (effect.GetType() == typeof(Pass))
                    yield return StartCoroutine(PassTurn());
               
                yield return StartCoroutine(ExecuteActionEffect(effect, executingUnit, targets));
            }
        }

        private IEnumerator PassTurn()
        {
            yield return new WaitForSeconds(1.25f);
        }

        protected virtual IEnumerator ExecuteActionEffect(ActionEffect effect, Unit user, List<Unit> targets)
        {
            foreach (Unit target in targets)
            {
                continueExecution = effect.AttemptActionEffect(user, target) || !effect.FailureStopsActionExecution || targets.Count > 1;
                yield return new WaitForSeconds(1.25f);
                Debug.Log($"Action of type {effect.GetType()} executed by {user.unitName} on {target.unitName}");
            }
        }
        #endregion

        #region Combat Item
        protected virtual IEnumerator ConsumeCombatItem(CombatItem item, Unit executingUnit, List<Unit> targets)
        {
            yield return StartCoroutine(executionUI.DisplayConsumingAction(item));
            continueExecution = true;

            foreach (ActionEffect effect in item.effects)
            {
                if (!continueExecution)
                    break;

                yield return StartCoroutine(ExecuteActionEffect(effect, executingUnit, targets));
            }

            Inventory.TryRemoveItemFromInventory(item);
        }
        #endregion

        protected virtual void EndExecutionPhase()
        {
            currentExecutingAction = null;
            currentConsumingItem = null;
            currentSelectedAction = null;
            currentSelectedItem = null;

            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }
    }
}