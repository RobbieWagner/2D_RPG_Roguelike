using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
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

        protected virtual IEnumerator ExecuteCombatAction(CombatAction combatAction, Unit executingUnit, List<Unit> targets)
        {
            Debug.Log(combatAction.actionName);
            yield return null;

            foreach(ActionEffect effect in combatAction.effects)
            {
                if (effect.GetType() == typeof(Attack))
                    yield return StartCoroutine(ExecuteDamageEffect((Attack) effect, executingUnit, targets));
                if (effect.GetType() == typeof(Heal))
                    yield return StartCoroutine(ExecuteHealingEffect((Heal)effect, executingUnit, targets));
                if (effect.GetType() == typeof(StatRaise))
                    yield return StartCoroutine(ExecuteStatRaiseEffect((StatRaise)effect, executingUnit, targets));
                if (effect.GetType() == typeof(StatLower))
                    yield return StartCoroutine(ExecuteStatLowerEffect((StatLower)effect, executingUnit, targets));
                if (effect.GetType() == typeof(Pass))
                    yield return StartCoroutine(PassTurn());
            }
        }

        private IEnumerator PassTurn()
        {
            yield return new WaitForSeconds(.25f);
        }

        protected virtual IEnumerator ExecuteDamageEffect(Attack effect, Unit user, List<Unit> targets)
        {
            foreach (Unit target in targets)
            {
                effect.AttemptAttack(user, target);
                yield return new WaitForSeconds(.25f);
                Debug.Log($"Attack attempted from {user.unitName} on {target.unitName}. Targets health is now {target.HP}/{target.GetMaxHP()}");
            }
        }

        protected virtual IEnumerator ExecuteHealingEffect(Heal effect, Unit user, List<Unit> targets)
        {
            foreach (Unit target in targets)
            {
                effect.AttemptHeal(user, target);
                yield return new WaitForSeconds(.25f);
                Debug.Log($"Heal attempted from {user.unitName} on {target.unitName}. Targets health is now {target.HP}/{target.GetMaxHP()}");
            }
        }

        protected virtual IEnumerator ExecuteStatRaiseEffect(StatRaise effect, Unit user, List<Unit> targets)
        {
            foreach (Unit target in targets)
            {
                effect.AttemptStatRaise(user, target);
                yield return new WaitForSeconds(.25f);
                Debug.Log($"Stat raise attempted from {user.unitName} on {target.unitName}. Targets {effect.stat} is now {target.Stats[effect.stat]}/{target.GetBaseStatValue(effect.stat)}");
            }
        }

        protected virtual IEnumerator ExecuteStatLowerEffect(StatLower effect, Unit user, List<Unit> targets)
        {
            foreach (Unit target in targets)
            {
                effect.AttemptStatLower(user, target);
                yield return new WaitForSeconds(.25f);
                Debug.Log($"Stat lower attempted from {user.unitName} on {target.unitName}. Targets {effect.stat} is now {target.Stats[effect.stat]}/{target.GetBaseStatValue(effect.stat)}");
            }
        }

        protected virtual void EndExecutionPhase()
        {
            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }
    }
}