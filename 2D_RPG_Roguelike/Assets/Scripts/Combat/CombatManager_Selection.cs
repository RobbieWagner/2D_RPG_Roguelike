using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        protected virtual List<Unit> GetUnitsLeftToAct()
        {
            List<Unit> unitsLeftToAct = GetUnitsByInitiative().Where(x => !x.hasActedThisTurn && x.isUnitFighting).ToList();
            if (!unitsLeftToAct.Any())
            {
                foreach (Unit unit in GetUnitsByInitiative())
                    unit.hasActedThisTurn = false;
                unitsLeftToAct = GetUnitsByInitiative();
            }

            return unitsLeftToAct;
        }

        protected virtual IEnumerator HandleSelectionPhase()
        {
            if (CheckForCombatCompletion())
                yield break;

            if (selectingUnit != null)
                selectingUnit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            selectingUnit = GetUnitsLeftToAct().FirstOrDefault();
            selectingUnit.GetComponentInChildren<SpriteRenderer>().color = Color.red;

            isCurrentlySelecting = true;

            if (IsUnitInCombatAlly(selectingUnit))
                //selectionUI.SetupUI((Ally)selectingUnit);
                SelectActionForUnit(selectingUnit);
            else if (IsUnitInCombatEnemy(selectingUnit))
                SelectActionForUnit(selectingUnit);
            else
                throw new InvalidOperationException($"Could not complete action selection phase: unit {selectingUnit.GetName()} is not valid for combat!!");

            while(currentSelectedAction == null || targets == null || !targets.Any() || isCurrentlySelecting)
                yield return null;

            EndSelectionPhase();
        }

        /// <summary>
        /// TODO: make this selection process smarter. (Use AI?)
        /// Selects 
        /// </summary>
        /// <param name="selectingUnit"></param>
        /// <param name="isUnitEnemy"></param>
        private void SelectActionForUnit(Unit selectingUnit)
        {
            List<CombatAction> validActions = GetValidCombatActions(selectingUnit.unitActions, selectingUnit);
            currentSelectedAction = validActions[UnityEngine.Random.Range(0, validActions.Count)];

            targets = SelectActionTargets(currentSelectedAction, selectingUnit);

            string targetsString = string.Join(", ", targets.Select(t => t.GetName()));
            Debug.Log($"ACTION IS SELECTED: {selectingUnit.GetName()} will use {currentSelectedAction.actionName} on {targetsString}");

            isCurrentlySelecting = false;
        }

        protected virtual List<CombatAction> GetValidCombatActions(List<CombatAction> actions, Unit user)
        {
            List<CombatAction> validActions = actions.Where(action => GetActionTargets(action, user).Any()).ToList(); //TODO: Change to check on the action itself (pass action can still execute even though it hits no targets)
            //Debug.Log($"valid actions: {validActions.Count}");
            return validActions;
        }

        protected virtual List<Unit> GetActionTargets(CombatAction action, Unit user)
        {
            List<Unit> activeUnitsOnUsersSide = IsUnitInCombatAlly(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();
            List<Unit> activeUnitsOnOpposition = IsUnitInCombatEnemy(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();

            List<Unit> validTargets = new List<Unit>();

            if (action.targetsAllAllies || action.canTargetAllies)
                validTargets.AddRange(activeUnitsOnUsersSide);
            if (action.targetsAllOpposition || action.canTargetOpposition)
                validTargets.AddRange(activeUnitsOnOpposition);
            
            if (!action.canTargetSelf)
                validTargets.Remove(selectingUnit);

            //Debug.Log($"valid targets for {action.actionName}: {validTargets.Count}");
            return validTargets;
        }

        private List<Unit> SelectActionTargets(CombatAction action, Unit user)
        {
            List<Unit> activeUnitsOnUsersSide = IsUnitInCombatAlly(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();
            List<Unit> activeUnitsOnOpposition = IsUnitInCombatEnemy(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();

            activeUnitsOnUsersSide = activeUnitsOnUsersSide.Where(u => u.isUnitFighting).ToList();
            activeUnitsOnOpposition = activeUnitsOnOpposition.Where(u => u.isUnitFighting).ToList();

            List<Unit> result = new List<Unit>();

            if (action.targetsAllAllies)
                result.AddRange(activeUnitsOnUsersSide);
            if (action.targetsAllOpposition)
                result.AddRange(activeUnitsOnOpposition);
            
            List<Unit> validTargets = new List<Unit>();
            if (action.canTargetAllies)
                validTargets.AddRange(activeUnitsOnUsersSide);
            if (action.canTargetOpposition)
                validTargets.AddRange(activeUnitsOnOpposition);
            if (!action.canTargetSelf)
                validTargets.Remove(selectingUnit);

            result.Add(validTargets[UnityEngine.Random.Range(0, validTargets.Count)]);
            

            return result;
        }

        protected virtual void EndSelectionPhase()
        {
            CurrentCombatPhase = CombatPhase.ACTION_EXECUTION;
        }
    }
}