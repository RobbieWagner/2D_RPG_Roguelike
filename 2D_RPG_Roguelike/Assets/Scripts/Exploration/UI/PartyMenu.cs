using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class PartyMenu : MenuTab
    {
        [SerializeField] private ButtonListener characterButton;

        [SerializeField] private ScrollRect actionListScrollRect;
        [SerializeField] private Transform actionListParent;
        [SerializeField] private ExplorationMenu explorationMenu;
        [SerializeField] private PartyMenuActionUI actionUIPrefab;
        [SerializeField] private PartyMenuUnitUI unitUI;
        private List<PartyMenuActionUI> actionUIInstances = new List<PartyMenuActionUI>();

        private SerializableAlly currentAlly;

        private int currentAllyIndex = 0;
        public int CurrentAllyIndex
        {
            get
            {
                return currentAllyIndex;
            }
            set
            {
                currentAllyIndex = value;
                if(currentAllyIndex < 0)
                    currentAllyIndex = Party.party.Count-1;
                if(currentAllyIndex >= Party.party.Count)
                    currentAllyIndex = 0;

                currentAlly = Party.party[currentAllyIndex];

                ConsiderCharacter(currentAlly);
                ToggleCharacterSelectionControls(true);
            }
        }

        private void Awake()
        {
            actionListScrollRect.enabled = false;
            characterButton.onClick.AddListener(EnterActionList);
        }

        public override void OnOpenTab()
        {
            base.OnOpenTab();
            CurrentAllyIndex = 0;
            //characterVisual.enabled = true;
            actionListScrollRect.enabled = true;
            StartCoroutine(FixScrollRect());
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(characterButton.gameObject);
        }

        //TODO: fix issues with scroll rect. Shouldn't have to wait a frame
        public IEnumerator FixScrollRect()
        {
            yield return null;

            actionListScrollRect.verticalNormalizedPosition = 1;
        }

        public override void OnCloseTab()
        {
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(null);
            //characterVisual.enabled = false;
            actionListScrollRect.enabled = false;
            ExitActionList();
            base.OnCloseTab();
        }

        private void ToggleCharacterSelectionControls(bool on)
        {
            if (on) InputManager.Instance.gameControls.UI.Navigate.performed += NavigateCharacters;
            else InputManager.Instance.gameControls.UI.Navigate.performed -= NavigateCharacters;
        }

        private void ConsiderCharacter(SerializableAlly ally)
        {
            unitUI.Unit = ally;
            ClearActionsList();
            CreateActionList(ally);
        }

        private void NavigateCharacters(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            float direction = context.ReadValue<Vector2>().x;
            if (direction > 0)
                CurrentAllyIndex++;
            else if (direction < 0)
                CurrentAllyIndex--;
        }

        private void ClearActionsList()
        {
            foreach(PartyMenuActionUI ui in actionUIInstances)
                Destroy(ui.gameObject);
            actionUIInstances.Clear();
        }

        private void CreateActionList(SerializableAlly ally)
        {
            foreach(CombatAction action in ally.LoadActions())
            {
                PartyMenuActionUI newUIInstance = Instantiate(actionUIPrefab, actionListParent);

                newUIInstance.CombatAction = action;
                actionUIInstances.Add(newUIInstance);
            }

            SetupActionListNavigation();
        }

        private void SetupActionListNavigation()
        {
            for (int i = 0; i < actionUIInstances.Count; i++)
            { 
                Navigation navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i == 0 ? null : actionUIInstances[i - 1],
                    selectOnDown = i == actionUIInstances.Count - 1 ? null : actionUIInstances[i + 1],
                };

                actionUIInstances[i].navigation = navigation;
            }
        }

        private void EnterActionList()
        {
            InputManager.Instance.gameControls.UI.Cancel.performed += ExitActionList;
            InputManager.Instance.gameControls.EXPLORATION.CancelCharacterSelection.performed += ExitActionList;
            ToggleCharacterSelectionControls(false);
            explorationMenu.CanCloseMenu = false;
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(actionUIInstances.First().gameObject);
        }

        private void ExitActionList(UnityEngine.InputSystem.InputAction.CallbackContext context) 
        {
            ExitActionList();
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(characterButton.gameObject);
        }
       
        private void ExitActionList()
        {
            explorationMenu.CanCloseMenu = true;
            ToggleCharacterSelectionControls(true);
            InputManager.Instance.gameControls.UI.Cancel.performed -= ExitActionList;
            InputManager.Instance.gameControls.EXPLORATION.CancelCharacterSelection.performed -= ExitActionList;
        }
    }
}