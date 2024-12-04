using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum CombatMenuType
    {
        NONE,
        MAIN,
        ACTION,
        TARGET
    }

    public enum MainCombatMenuOption
    {
        NONE,
        ACTION,
        ITEM,
        FLEE,
    }

    public class CombatSelectionUI : MonoBehaviour
    {
        [SerializeField] protected Canvas canvas;

        protected Dictionary<Unit, int> savedMainSelectionIndices = new Dictionary<Unit, int>();
        protected Dictionary<Unit, int> savedActionSelectionIndices = new Dictionary<Unit, int>();
        protected bool useSavedActionIndex = false;
        protected Dictionary<Unit, int> savedTargetSelectionIndices = new Dictionary<Unit, int>();
        protected bool useSavedTargetIndex = false;

        [SerializeField] protected CombatMenu menuPrefab;
        [SerializeField] protected CombatHUDMenu targetMenuPrefab;

        private CombatMenu mainCombatMenu = null;
        private CombatMenu actionCombatMenu = null;
        private CombatHUDMenu targetCombatMenu = null;

        protected Unit selectingUnit;
        [SerializeField] protected Vector2 menuPositionOffset;
        protected Vector2 InitialMenuPosition => (Vector2)selectingUnit.transform.position + menuPositionOffset;

        public virtual void SetupUI(Unit selectingUnit)
        {
            this.selectingUnit = selectingUnit;
            DisplayMainSelectionUI();
        }

        #region controls subscription
        protected virtual void ToggleControlsSubscription(CombatMenuType type, bool on)
        {
            if(type == CombatMenuType.MAIN)
            {
                if(on)
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed += mainCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed += mainCombatMenu.SelectCurrentButton;
                }
                else
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed -= mainCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed -= mainCombatMenu.SelectCurrentButton;
                }
            }
            else if (type == CombatMenuType.ACTION)
            {
                if (on)
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed += actionCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed += actionCombatMenu.SelectCurrentButton;
                    InputManager.Instance.gameControls.COMBAT.Cancel.performed += ReturnToMainSelectionMenu;
                }
                else
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed -= actionCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed -= actionCombatMenu.SelectCurrentButton;
                    InputManager.Instance.gameControls.COMBAT.Cancel.performed -= ReturnToMainSelectionMenu;
                }
            }
            else if (type == CombatMenuType.TARGET)
            {
                if (on)
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed += targetCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed += targetCombatMenu.SelectCurrentButton;
                    InputManager.Instance.gameControls.COMBAT.Cancel.performed += ReturnToActionSelectionMenu;
                }
                else
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed -= targetCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed -= targetCombatMenu.SelectCurrentButton;
                    InputManager.Instance.gameControls.COMBAT.Cancel.performed -= ReturnToActionSelectionMenu;
                }
            }
        }

        private void ReturnToMainSelectionMenu(InputAction.CallbackContext context)
        {
            DisableActionMenu();
            DisplayMainSelectionUI();
        }

        private void ReturnToActionSelectionMenu(InputAction.CallbackContext context)
        {
            DisableTargetMenu();
            CombatManagerBase.Instance.currentSelectedAction = null;
            DisplayActionSelectionUI();
        }
        #endregion

        public virtual void DisplayMainSelectionUI()
        {
            if (selectingUnit != null)
            {
                mainCombatMenu = Instantiate(menuPrefab, transform);
                mainCombatMenu.transform.position = selectingUnit.transform.position + new Vector3(2, 2, 0);
                mainCombatMenu.AddButtonToList(HandleMainCombatMenuOptionSelection, MainCombatMenuOption.ACTION.ToString());
                mainCombatMenu.AddButtonToList(HandleMainCombatMenuOptionSelection, MainCombatMenuOption.ITEM.ToString());
                mainCombatMenu.AddButtonToList(HandleMainCombatMenuOptionSelection, MainCombatMenuOption.FLEE.ToString());

                ToggleControlsSubscription(CombatMenuType.MAIN, true);
                InputManager.Instance.gameControls.COMBAT.Enable();

                if(savedMainSelectionIndices.TryGetValue(selectingUnit, out int index) && index < 3)
                    mainCombatMenu.CurIndex = index;
                else
                    mainCombatMenu.CurIndex = 0;
            }
            else
                Debug.LogWarning("Could not display combatInfo selection UI: unit found null!!");
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
                Debug.LogWarning($"Error thrown while executing main combatInfo menu option selection: {e}");
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

            DisableMainCombatMenu();
        }

        private void DisableMainCombatMenu()
        {
            ToggleControlsSubscription(CombatMenuType.MAIN, false);
            Destroy(mainCombatMenu.gameObject);
            mainCombatMenu = null;
        }

        public virtual void DisplayActionSelectionUI()
        {
            if (selectingUnit != null && selectingUnit.unitActions != null && selectingUnit.unitActions.Any())
            {
                actionCombatMenu = Instantiate(menuPrefab, transform);
                actionCombatMenu.transform.position = selectingUnit.transform.position + new Vector3(2, 2, 0);
                for (int i = 0; i < selectingUnit.unitActions.Count; i++)
                    actionCombatMenu.AddButtonToList(HandleActionCombatMenuOptionSelection, selectingUnit.unitActions[i].actionName);

                InputManager.Instance.gameControls.COMBAT.Navigate.Reset();
                InputManager.Instance.gameControls.COMBAT.Select.Reset();

                ToggleControlsSubscription(CombatMenuType.ACTION, true);

                if (useSavedActionIndex && savedActionSelectionIndices.TryGetValue(selectingUnit, out int index) && index < selectingUnit.unitActions.Count)
                    actionCombatMenu.CurIndex = index;
                else
                    actionCombatMenu.CurIndex = 0;
            }
            else
                Debug.LogWarning("Could not display combatInfo selection UI: unit or their actions found null!!");
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

            DisableActionMenu();

            DisplayTargetSelectionUI();
        }

        protected void DisableActionMenu()
        {
            ToggleControlsSubscription(CombatMenuType.ACTION, false);
            Destroy(actionCombatMenu.gameObject);
            actionCombatMenu = null;
        }

        #region target selection
        public virtual void DisplayTargetSelectionUI()
        {
            if (CombatManagerBase.Instance.currentSelectedAction != null)
            {
                targetCombatMenu = Instantiate(targetMenuPrefab, transform);
                List<Unit> targets = CombatManagerBase.Instance.GetActionTargets(CombatManagerBase.Instance.currentSelectedAction, selectingUnit);
                foreach (Unit target in targets)
                    targetCombatMenu.AddButtonToList(HandleTargetSelection, target.transform, Vector3.up);

                InputManager.Instance.gameControls.COMBAT.Navigate.Reset();
                InputManager.Instance.gameControls.COMBAT.Select.Reset();

                ToggleControlsSubscription(CombatMenuType.TARGET, true);
                InputManager.Instance.gameControls.COMBAT.Enable();

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
            
            DisableTargetMenu();

            CombatManagerBase.Instance.isCurrentlySelecting = false;
        }

        private void DisableTargetMenu()
        {
            ToggleControlsSubscription(CombatMenuType.TARGET, false);
            Destroy(targetCombatMenu.gameObject);
            targetCombatMenu = null;
        }
        #endregion
    }
}