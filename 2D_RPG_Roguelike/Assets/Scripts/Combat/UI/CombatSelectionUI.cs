using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

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

        [SerializeField] protected Canvas worldSpaceCanvas;
        [SerializeField] protected Canvas overlayCanvas;

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
                    InputManager.Instance.gameControls.COMBAT.Cancel.performed += ReturnToPreTargetMenu;
                }
                else
                {
                    InputManager.Instance.gameControls.COMBAT.Navigate.performed -= targetCombatMenu.NavigateMenu;
                    InputManager.Instance.gameControls.COMBAT.Select.performed -= targetCombatMenu.SelectCurrentButton;
                    InputManager.Instance.gameControls.COMBAT.Cancel.performed -= ReturnToPreTargetMenu;
                }
            }
        }

        private void ReturnToMainSelectionMenu(InputAction.CallbackContext context)
        {
            DisableActionMenu();
            DisplayMainSelectionUI();
        }

        private void ReturnToPreTargetMenu(InputAction.CallbackContext context)
        {
            DisableTargetMenu();

            if (CombatManagerBase.Instance.currentSelectedAction != null)
            {
                CombatManagerBase.Instance.currentSelectedAction = null;
                DisplayActionSelectionUI();
            }
            else if (CombatManagerBase.Instance.currentSelectedItem != null)
            {
                CombatManagerBase.Instance.currentSelectedItem = null;
                DisplayItemSelectionUI();
            }
            
        }
        #endregion

        public virtual void DisplayMainSelectionUI()
        {
            if (selectingUnit != null)
            {
                CombatManagerBase.Instance.currentSelectedAction = null;
                CombatManagerBase.Instance.currentSelectedItem = null;

                mainCombatMenu = Instantiate(menuPrefab, worldSpaceCanvas.transform);
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
                    DisplayActionSelectionUI();
                else if (index == 1)
                    DisplayItemSelectionUI();
                else if (index == 2)
                    AttemptFleeCombat();
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

        private void AttemptFleeCombat()
        {
            if (!CombatManagerBase.Instance.currentCombat.isBossFight)
                StartCoroutine(CombatManagerBase.Instance.EndCombat());
        }

        #region Action Selection
        public virtual void DisplayActionSelectionUI()
        {
            if (selectingUnit != null && selectingUnit.unitActions != null && selectingUnit.unitActions.Any())
            {
                actionCombatMenu = Instantiate(menuPrefab, worldSpaceCanvas.transform);
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
        #endregion

        #region Item Selection
        public void DisplayItemSelectionUI()
        {
            if (CombatManagerBase.Instance.combatItemOptions.Count > 0)
            {
                actionCombatMenu = Instantiate(menuPrefab, worldSpaceCanvas.transform);
                actionCombatMenu.transform.position = selectingUnit.transform.position + new Vector3(2, 2, 0);
                for (int i = 0; i < CombatManagerBase.Instance.combatItemOptions.Count; i++)
                    actionCombatMenu.AddButtonToList(HandleItemCombatMenuOptionSelection, CombatManagerBase.Instance.combatItemOptions[i].itemName);

                InputManager.Instance.gameControls.COMBAT.Navigate.Reset();
                InputManager.Instance.gameControls.COMBAT.Select.Reset();

                ToggleControlsSubscription(CombatMenuType.ACTION, true);

                if (useSavedActionIndex && savedActionSelectionIndices.TryGetValue(selectingUnit, out int index) && index < CombatManagerBase.Instance.combatItemOptions.Count)
                    actionCombatMenu.CurIndex = index;
                else
                    actionCombatMenu.CurIndex = 0;
            }
            else
            {
                DisableItemMenu();
                DisplayMainSelectionUI();
            }
        }

        protected virtual void HandleItemCombatMenuOptionSelection(int index)
        {
            CombatItem item = CombatManagerBase.Instance.combatItemOptions[index];
            CombatManagerBase.Instance.currentSelectedItem = item;

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

            DisableItemMenu();
            DisplayTargetSelectionUI();
        }

        protected void DisableItemMenu()
        {
            ToggleControlsSubscription(CombatMenuType.ACTION, false);
            Destroy(actionCombatMenu.gameObject);
            actionCombatMenu = null;
        }
        #endregion

        #region Target Selection
        public virtual void DisplayTargetSelectionUI()
        {
            List<Unit> targets;

            if (CombatManagerBase.Instance.currentSelectedAction != null)
                targets = CombatManagerBase.Instance.GetActionTargets(CombatManagerBase.Instance.currentSelectedAction, selectingUnit);
            else if (CombatManagerBase.Instance.currentSelectedItem != null)
                targets = CombatManagerBase.Instance.GetItemTargets(CombatManagerBase.Instance.currentSelectedItem, selectingUnit);
            else
                return;

            targetCombatMenu = Instantiate(targetMenuPrefab, worldSpaceCanvas.transform);
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

        public virtual void HandleTargetSelection(int index)
        {
            //TODO : make target list private global?
            List<Unit> targets = new List<Unit>();
            if (CombatManagerBase.Instance.currentSelectedAction != null)
                targets = CombatManagerBase.Instance.GetActionTargets(CombatManagerBase.Instance.currentSelectedAction, selectingUnit);
            else if (CombatManagerBase.Instance.currentSelectedItem != null)
                targets = CombatManagerBase.Instance.GetItemTargets(CombatManagerBase.Instance.currentSelectedItem, selectingUnit);

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