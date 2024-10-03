using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.TurnBasedCombat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum MainCombatMenuOption
    {
        NONE,
        ACTION,
        ITEM,
        FLEE
    }

    public class CombatSelectionUI : MonoBehaviour
    {
        [SerializeField] protected Canvas canvas;

        protected Dictionary<Unit, int> savedMainSelectionIndices = new Dictionary<Unit, int>();
        protected Dictionary<Unit, int> savedActionSelectionIndices = new Dictionary<Unit, int>();
        protected bool useSavedActionIndex = false;
        protected Dictionary<Unit, int> savedTargetSelectionIndices = new Dictionary<Unit, int>();
        protected bool useSavedTargetIndex = false;

        protected CombatControls controls;

        [SerializeField] protected CombatMenu menuPrefab;
        [SerializeField] protected CombatHUDMenu targetMenuPrefab;

        private CombatMenu mainCombatMenu = null;
        private CombatMenu actionCombatMenu = null;
        private CombatHUDMenu targetCombatMenu = null;

        protected Unit selectingUnit;
        [SerializeField] protected Vector2 menuPositionOffset;
        protected Vector2 InitialMenuPosition => (Vector2)selectingUnit.transform.position + menuPositionOffset;

        protected virtual void Awake()
        {
            controls = new CombatControls();
        }

        public virtual void SetupUI(Unit selectingUnit)
        {
            this.selectingUnit = selectingUnit;
            DisplayMainSelectionUI();
        }

        public virtual void DisplayMainSelectionUI()
        {
            if (selectingUnit != null)
            {
                mainCombatMenu = Instantiate(menuPrefab, transform);
                mainCombatMenu.AddButtonToList(HandleMainCombatMenuOptionSelection, MainCombatMenuOption.ACTION.ToString());
                mainCombatMenu.AddButtonToList(HandleMainCombatMenuOptionSelection, MainCombatMenuOption.ITEM.ToString());
                mainCombatMenu.AddButtonToList(HandleMainCombatMenuOptionSelection, MainCombatMenuOption.FLEE.ToString());

                controls.MainSelectionMenu.Navigate.performed += mainCombatMenu.NavigateMenu;
                controls.MainSelectionMenu.Select.performed += mainCombatMenu.SelectCurrentButton;
                controls.MainSelectionMenu.Enable();

                if(savedMainSelectionIndices.TryGetValue(selectingUnit, out int index) && index < 3)
                    mainCombatMenu.CurIndex = index;
                else
                    mainCombatMenu.CurIndex = 0;
            }
            else
                Debug.LogWarning("Could not display combat selection UI: unit found null!!");
        }

        protected virtual void HandleMainCombatMenuOptionSelection(int index)
        {
            try
            {
                if (index == 0)
                {
                    DisplayActionSelectionUI();
                }
                else if (index == 1)
                {
                    throw new NotImplementedException($"{MainCombatMenuOption.ITEM} option is not yet implemented");
                }
                else if (index == 2)
                {
                    throw new NotImplementedException($"{MainCombatMenuOption.FLEE} option is not yet implemented");
                }
                else
                    throw new NotImplementedException($"Index {index} does not have an implemented action");
            }
            catch (NotImplementedException e) 
            {
                Debug.LogWarning($"Error thrown while executing main combat menu option selection: {e}");
                return;
            }

            if (savedMainSelectionIndices.TryGetValue(selectingUnit, out int value))
            {
                savedMainSelectionIndices[selectingUnit] = index;
                useSavedActionIndex = true;
            }
            else
            {
                savedMainSelectionIndices.Add(selectingUnit, index);
                useSavedActionIndex = false;
            }

            controls.MainSelectionMenu.Navigate.performed -= mainCombatMenu.NavigateMenu;
            controls.MainSelectionMenu.Select.performed -= mainCombatMenu.SelectCurrentButton;
            Destroy(mainCombatMenu.gameObject);
            mainCombatMenu = null;
            controls.MainSelectionMenu.Disable();
        }

        public virtual void DisplayActionSelectionUI()
        {
            if (selectingUnit != null && selectingUnit.unitActions != null && selectingUnit.unitActions.Any())
            {
                actionCombatMenu = Instantiate(menuPrefab, transform);
                for (int i = 0; i < selectingUnit.unitActions.Count; i++)
                    actionCombatMenu.AddButtonToList(HandleActionCombatMenuOptionSelection, selectingUnit.unitActions[i].actionName);

                controls.ActionSelectionMenu.Navigate.Reset();
                controls.ActionSelectionMenu.Select.Reset();

                controls.ActionSelectionMenu.Navigate.performed += actionCombatMenu.NavigateMenu;
                controls.ActionSelectionMenu.Select.performed += actionCombatMenu.SelectCurrentButton;
                controls.ActionSelectionMenu.Enable();

                if (useSavedActionIndex && savedActionSelectionIndices.TryGetValue(selectingUnit, out int index) && index < selectingUnit.unitActions.Count)
                    actionCombatMenu.CurIndex = index;
                else
                    actionCombatMenu.CurIndex = 0;
            }
            else
                Debug.LogWarning("Could not display combat selection UI: unit or their actions found null!!");
        }

        protected virtual void HandleActionCombatMenuOptionSelection(int index)
        {
            CombatAction action = selectingUnit.unitActions[index];
            CombatManagerBase.Instance.currentSelectedAction = action;

            if (savedActionSelectionIndices.TryGetValue(selectingUnit, out int value))
            {
                savedActionSelectionIndices[selectingUnit] = index;
                useSavedTargetIndex = true;
            }
            else
            {
                savedActionSelectionIndices.Add(selectingUnit, index);
                useSavedTargetIndex = false;
            }

            controls.ActionSelectionMenu.Navigate.performed -= actionCombatMenu.NavigateMenu;
            controls.ActionSelectionMenu.Select.performed -= actionCombatMenu.SelectCurrentButton;
            Destroy(actionCombatMenu.gameObject);
            actionCombatMenu = null;
            controls.ActionSelectionMenu.Disable();

            DisplayTargetSelectionUI();
        }

        #region target selection
        public virtual void DisplayTargetSelectionUI()
        {
            if (CombatManagerBase.Instance.currentSelectedAction != null)
            {
                targetCombatMenu = Instantiate(targetMenuPrefab, transform);
                List<Unit> targets = CombatManagerBase.Instance.GetActionTargets(CombatManagerBase.Instance.currentSelectedAction, selectingUnit);
                foreach (Unit target in targets)
                    targetCombatMenu.AddButtonToList(HandleTargetSelection, target.transform, Vector3.zero);

                controls.TargetSelectionMenu.Navigate.Reset();
                controls.TargetSelectionMenu.Select.Reset();

                controls.TargetSelectionMenu.Navigate.performed += targetCombatMenu.NavigateMenu;
                controls.TargetSelectionMenu.Select.performed += targetCombatMenu.SelectCurrentButton;
                controls.TargetSelectionMenu.Enable();

                if (useSavedTargetIndex && savedTargetSelectionIndices.TryGetValue(selectingUnit, out int index) && index < targets.Count)
                    targetCombatMenu.CurIndex = index;
                else
                    targetCombatMenu.CurIndex = 0;
            }
        }

        public virtual void HandleTargetSelection(int index)
        {
            List<Unit> targets = CombatManagerBase.Instance.GetActionTargets(CombatManagerBase.Instance.currentSelectedAction, selectingUnit);
            CombatManagerBase.Instance.targets = new List<Unit>() { targets[index] };

            if (savedTargetSelectionIndices.TryGetValue(selectingUnit, out int value))
                savedTargetSelectionIndices[selectingUnit] = index;
            else
                savedTargetSelectionIndices.Add(selectingUnit, index);

            controls.TargetSelectionMenu.Navigate.performed -= targetCombatMenu.NavigateMenu;
            controls.TargetSelectionMenu.Select.performed -= targetCombatMenu.SelectCurrentButton;
            Destroy(targetCombatMenu.gameObject);
            targetCombatMenu = null;
            controls.TargetSelectionMenu.Disable();

            CombatManagerBase.Instance.isCurrentlySelecting = false;
        }
        #endregion
    }
}