using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class PartyMenu : MenuTab
    {
        [SerializeField] private ButtonListener characterButton;
        [SerializeField] private Image characterVisual;

        [SerializeField] private ScrollRect actionListScrollRect;
        [SerializeField] private Transform actionListParent;
        [SerializeField] private PartyMenuActionUI actionUIPrefab;
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
            characterButton.onClick.AddListener(EnterActionList);
        }

        public override void OnOpenTab()
        {
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(null);
            CurrentAllyIndex = 0;
            characterVisual.enabled = true;
            base.OnOpenTab();
        }

        public override void OnCloseTab()
        {
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(null);
            characterVisual.enabled = false;    
            base.OnCloseTab();
        }

        private void ToggleCharacterSelectionControls(bool on)
        {
            if (on) InputManager.Instance.gameControls.UI.Navigate.performed += NavigateCharacters;
            else InputManager.Instance.gameControls.UI.Navigate.performed -= NavigateCharacters;
        }

        private void ConsiderCharacter(SerializableAlly ally)
        {
            characterVisual.sprite = Resources.Load<Sprite>(StaticGameStats.characterSpriteFilePath + ally.dialogueSpriteRelativePath);

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
            actionListScrollRect.verticalNormalizedPosition = 1;
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
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(actionUIInstances.First().gameObject);
        }

        private void ExitActionList(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            InputManager.Instance.gameControls.UI.Cancel.performed -= ExitActionList;
        }
    }
}