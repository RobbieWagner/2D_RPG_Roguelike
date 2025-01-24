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
    public class InventoryMenu : MenuTab
    {
        [SerializeField] private ExplorationMenu explorationMenu;
        [SerializeField] private GameItemUI gameItemUIPrefab;
        private List<GameItemUI> gameItemUIInstances = new List<GameItemUI>();
        [SerializeField] private InventoryMenuPartyMemberUI partyMemberUIPrefab;
        private List<InventoryMenuPartyMemberUI> partyMemberUIInstances = new List<InventoryMenuPartyMemberUI>();
        [SerializeField] private Transform partyMemberUIParent;
        [SerializeField] private ScrollRect inventoryUI;
        [SerializeField] private Transform inventoryListParent;
        [SerializeField] private GameItemUI detailsDisplay;

        private GameItemUI consideredGameItemUI;
        private GameItem consideredGameItem;
        private GameItem selectedGameItem;
        public GameItem SelectedGameItem
        {
            get 
            {
                return selectedGameItem;
            }
            set 
            {
                selectedGameItem = value;
                if (selectedGameItem == null)
                {
                    explorationMenu.CanCloseMenu = true;
                    InputManager.Instance.gameControls.UI.Cancel.performed -= CancelItemConfirmation;
                    explorationMenu.ToggleTabNavigation(true);
                }
                else if (selectedGameItem.GetType() == typeof(CombatItem))
                {
                    explorationMenu.CanCloseMenu = false;
                    InputManager.Instance.gameControls.UI.Cancel.performed += CancelItemConfirmation;
                    EventSystemManager.Instance.eventSystem.SetSelectedGameObject(partyMemberUIInstances.First().gameObject);
                    explorationMenu.ToggleTabNavigation(false);
                }
                else if (selectedGameItem.GetType() == typeof(KeyItem))
                {
                    Debug.Log("used a key item!");
                }
            }
        }

        public override void OnOpenTab()
        {
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(null);
            DestroyAllInventoryUIInstances();
            RebuildInventoryListUI();
            RebuildInventoryPartyUI();
            SetupMenuNavigation();
            inventoryUI.verticalNormalizedPosition = 1;

            if(gameItemUIInstances.Any()) 
                defaultSelection = gameItemUIInstances.First().gameObject;
            base.OnOpenTab();
        }

        public override void OnCloseTab()
        {
            DestroyAllInventoryUIInstances();

            base.OnCloseTab();
        }

        private void DestroyAllInventoryUIInstances()
        {
            foreach (GameItemUI uiInstance in gameItemUIInstances)
            {
                uiInstance.OnSelectButton.OnButtonSelected -= ConsiderItem;
                Destroy(uiInstance.gameObject);
            }
            gameItemUIInstances.Clear();

            foreach (InventoryMenuPartyMemberUI uiInstance in partyMemberUIInstances)
            {
                Destroy(uiInstance.gameObject);
            }
            partyMemberUIInstances.Clear();
        }

        private void RebuildInventoryListUI()
        {
            foreach (GameItem item in Inventory.inventory.Keys)
            {
                GameItemUI newItemUI = Instantiate(gameItemUIPrefab, inventoryListParent);
                newItemUI.GameItem = item;
                newItemUI.inventoryMenu = this;
                newItemUI.OnSelectButton.OnButtonSelected += ConsiderItem;
                gameItemUIInstances.Add(newItemUI);
            }
        }

        private void RebuildInventoryPartyUI()
        {
            if (Party.party != null)
            {
                foreach (SerializableAlly ally in Party.party)
                {
                    InventoryMenuPartyMemberUI newPartyMemberUI = Instantiate(partyMemberUIPrefab, partyMemberUIParent);
                    newPartyMemberUI.Unit = ally;
                    partyMemberUIInstances.Add(newPartyMemberUI);
                    newPartyMemberUI.Initialize();
                    newPartyMemberUI.inventoryMenu = this;
                }
            }
        }

        private void UpdateInventoryList()
        {
            int currentItemIndex = gameItemUIInstances.IndexOf(consideredGameItemUI);

            if (currentItemIndex == -1) 
                currentItemIndex = 0;

            RemoveZeroQuantityItems();

            if (currentItemIndex >= gameItemUIInstances.Count)
                currentItemIndex = gameItemUIInstances.Count - 1;

            foreach (GameItemUI ui in gameItemUIInstances)
                ui.SyncWithInventory();

            consideredGameItemUI = gameItemUIInstances[currentItemIndex];
            if (!Inventory.inventory.TryGetValue(selectedGameItem, out int i) || i <= 0)
                CancelItemConfirmation();
        }

        private void UpdatePartyUI()
        {
            foreach(InventoryMenuPartyMemberUI ui in partyMemberUIInstances)
                ui.SyncToUnit();
        }

        private void RemoveZeroQuantityItems()
        {
            foreach(GameItemUI ui in gameItemUIInstances)
            {
                if (!Inventory.inventory.TryGetValue(ui.GameItem, out int i) || i <= 0)
                {
                    Destroy(ui.gameObject);
                }
            }
            gameItemUIInstances = gameItemUIInstances.Where(x => Inventory.inventory.TryGetValue(x.GameItem, out int i) && i > 0).ToList();
        }

        private void SetupMenuNavigation()
        {
            for (int i = 0; i < gameItemUIInstances.Count; i++)
            {
                UnityEngine.UI.Button button = gameItemUIInstances[i].OnSelectButton;

                Navigation navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i == 0 ? null : gameItemUIInstances[i - 1].OnSelectButton,
                    selectOnDown = i == gameItemUIInstances.Count - 1 ? null : gameItemUIInstances[i + 1].OnSelectButton,
                };

                button.navigation = navigation;
            }

            for (int i = 0; i < partyMemberUIInstances.Count; i++)
            {
                UnityEngine.UI.Button partyMemberButton = partyMemberUIInstances[i].button;

                Navigation navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i == 0 ? partyMemberUIInstances[partyMemberUIInstances.Count - 1].button : partyMemberUIInstances[i - 1].button,
                    selectOnDown = i == partyMemberUIInstances.Count - 1 ? partyMemberUIInstances[0].button : partyMemberUIInstances[i + 1].button,
                };

                partyMemberButton.navigation = navigation;
            }
        }

        private void ConsiderItem(UnityEngine.UI.Button button)
        {
            consideredGameItemUI = button.GetComponent<GameItemUI>();
            consideredGameItem = consideredGameItemUI?.GameItem;
            detailsDisplay.GameItem = consideredGameItem;
        }

        private void CancelItemConfirmation(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            CancelItemConfirmation();
        }

        private void CancelItemConfirmation()
        {
            EventSystemManager.Instance.eventSystem.SetSelectedGameObject(consideredGameItemUI.gameObject);
            SelectedGameItem = null;
        }

        public void UseSelectedCombatItemOnUnit(SerializableAlly unit)
        {
            try
            {
                CombatItem item = (CombatItem)selectedGameItem;
                bool success = false;
                foreach (ActionEffect effect in item.effects)
                {
                    if (!effect.ApplyEffectOutsideOfCombat(unit))
                        break;
                    success = true;
                }

                if (success)
                {
                    UpdatePartyUI();
                    if (!selectedGameItem.reusable)
                    {
                        Inventory.TryRemoveItemFromInventory(selectedGameItem, 1);
                        UpdateInventoryList();
                    }
                }
                else
                    Debug.Log("failed to use item on unit");
            }
            catch (Exception e) 
            {
                Debug.LogError($"error thrown while trying to apply item to unit {e}");
            }
        }
    }
}