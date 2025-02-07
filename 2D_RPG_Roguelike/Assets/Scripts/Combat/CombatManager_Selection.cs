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
        public List<Unit> targets = null;
        public bool isCurrentlySelecting;

        public CombatAction currentSelectedAction = null;
        public CombatItem currentSelectedItem = null;

        public List<CombatItem> combatItemOptions = new List<CombatItem>();

        protected virtual List<Unit> GetUnitsLeftToAct()
        {
            List<Unit> unitsLeftToAct = GetUnitsByInitiative().Where(x => !x.hasActedThisTurn && x.isUnitFighting).ToList();
            if (!unitsLeftToAct.Any())
            {
                foreach (Unit unit in GetUnitsByInitiative())
                    unit.hasActedThisTurn = false;
                unitsLeftToAct = GetUnitsByInitiative().Where(x => !x.hasActedThisTurn && x.isUnitFighting).ToList();
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

            if (IsUnitAlly(selectingUnit))
                StartPlayerActionSelection(selectingUnit);
            else if (IsUnitEnemy(selectingUnit))
                SelectActionForUnit(selectingUnit);
            else
                throw new InvalidOperationException($"Could not complete action selection phase: unit {selectingUnit.GetName()} is not valid for combatInfo!!");

            InputManager.Instance.EnableActionMap(ActionMapName.COMBAT.ToString());

            combatItemOptions = Inventory.GetCombatItems();

            while ((currentSelectedAction == null && currentSelectedItem == null) || targets == null || !targets.Any() || isCurrentlySelecting)
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
            //Debug.Log($"ACTION IS SELECTED: {selectingUnit.GetName()} will use {currentSelectedAction.actionName} on {targetsString}");

            isCurrentlySelecting = false;
        }

        protected virtual List<CombatAction> GetValidCombatActions(List<CombatAction> actions, Unit user)
        {
            List<CombatAction> validActions = actions.Where(action => GetActionTargets(action, user).Any() || action.effects[0].GetType() == typeof(Pass)).ToList();
            //Debug.Log($"valid actions: {validActions.Count}");
            return validActions;
        }

        public virtual List<Unit> GetActionTargets(CombatAction action, Unit user)
        {
            List<Unit> activeUnitsOnUsersSide = IsUnitAlly(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();
            List<Unit> activeUnitsOnOpposition = IsUnitEnemy(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();

            List<Unit> validTargets = new List<Unit>();

            if (action.targetsAllAllies || action.canTargetAllies)
                validTargets.AddRange(activeUnitsOnUsersSide);
            if (action.targetsAllOpposition || action.canTargetOpposition)
                validTargets.AddRange(activeUnitsOnOpposition);
            
            if (!action.canTargetSelf)
                validTargets.Remove(selectingUnit);

            validTargets = validTargets.OrderBy(u => u.unitCombatPos).ToList();

            //Debug.Log($"valid targets for {action.actionName}: {validTargets.Count}");
            return validTargets;
        }

        private List<Unit> SelectActionTargets(CombatAction action, Unit user)
        {
            List<Unit> activeUnitsOnUsersSide = IsUnitAlly(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();
            List<Unit> activeUnitsOnOpposition = IsUnitEnemy(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();

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

        public List<Unit> GetItemTargets(CombatItem item, Unit user)
        {
            //TODO: Combine functionality with action? Would this create a dependency to get rid of this code?
            List<Unit> activeUnitsOnUsersSide = IsUnitAlly(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();
            List<Unit> activeUnitsOnOpposition = IsUnitEnemy(user) ? activeAllies.Select(a => (Unit)a).ToList() : activeEnemies.Select(e => (Unit)e).ToList();

            List<Unit> validTargets = new List<Unit>();

            if (item.targetsAllAllies || item.canTargetAllies)
                validTargets.AddRange(activeUnitsOnUsersSide);
            if (item.canTargetDownedAllies)
                validTargets.AddRange(allyInstances.Where(a => !a.isUnitFighting));
            if (item.targetsAllOpposition || item.canTargetOpposition)
                validTargets.AddRange(activeUnitsOnOpposition);

            if (!item.canTargetSelf)
                validTargets.Remove(selectingUnit);

            validTargets = validTargets.OrderBy(u => u.unitCombatPos).ToList();

            //Debug.Log($"valid targets for {action.actionName}: {validTargets.Count}");
            return validTargets;
        }

        protected virtual void StartPlayerActionSelection(Unit unit)
        {
            selectionUI.SetupUI(unit);
        }

        protected virtual void EndSelectionPhase()
        {
            InputManager.Instance.DisableActionMap(ActionMapName.COMBAT.ToString());
            CurrentCombatPhase = CombatPhase.ACTION_EXECUTION;
        }
    }
}