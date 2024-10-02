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
        private CombatMenu targetCombatMenu = null;

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

                controls.MainSelectionMenu.Navigate.Reset();
                controls.MainSelectionMenu.Select.Reset();

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
                    mainCombatMenu.CurIndex = index;
                else
                    mainCombatMenu.CurIndex = 0;
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

            Destroy(actionCombatMenu.gameObject);
            actionCombatMenu = null;
            controls.ActionSelectionMenu.Disable();

            DisplayTargetSelectionUI();
        }

        #region target selection
        public virtual void DisplayTargetSelectionUI()
        {
            // Have a HUD UI for the targets
            // When navigating, enable the HUD over the considered target
        }

        public void NavigateTargets(InputAction.CallbackContext context)
        {
            float inputValue = context.ReadValue<float>();

            if (inputValue < 0)
            {

            }
        }

        public virtual void HandleTargetSelectionUI(int index)
        {
            if(savedTargetSelectionIndices.TryGetValue(selectingUnit, out int value))
                savedTargetSelectionIndices[selectingUnit] = index;
            else
                savedTargetSelectionIndices.Add(selectingUnit, index);
        }
        #endregion
    }
}