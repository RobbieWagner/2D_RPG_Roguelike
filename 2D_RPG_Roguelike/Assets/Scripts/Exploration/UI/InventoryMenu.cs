using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class InventoryMenu : MenuTab
    {
        [SerializeField] private GameItemUI gameItemUIPrefab;
        private List<GameItemUI> gameItemUIInstances = new List<GameItemUI>();
        [SerializeField] private InventoryMenuPartyMemberUI partyMemberUIPrefab;
        private List<InventoryMenuPartyMemberUI> partyMemberUIInstances = new List<InventoryMenuPartyMemberUI>();
        [SerializeField] private Transform partyMemberUIParent;
        [SerializeField] private ScrollRect inventoryUI;
        [SerializeField] private Transform inventoryListParent;
        [SerializeField] private GameItemUI detailsDisplay;

        private GameItem consideredGameItem;

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
                //newItemUI.transform.SetParent(inventoryListParent);
                newItemUI.GameItem = item;
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
                    //newPartyMemberUI.transform.SetParent(partyMemberUIParent);
                    newPartyMemberUI.Unit = ally;
                    partyMemberUIInstances.Add(newPartyMemberUI);
                }
            }
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
                    selectOnRight = Party.party != null && Party.party.Any() ? partyMemberUIInstances.First().button : null,
                };

                button.navigation = navigation;
            }
        }

        private void ConsiderItem(UnityEngine.UI.Button button)
        {
            GameItemUI consideredItemUI = button.GetComponent<GameItemUI>();
            consideredGameItem = consideredItemUI?.GameItem;
            if (consideredGameItem != null) UpdatePartyUINavigation(button);
            detailsDisplay.GameItem = consideredGameItem;
        }

        private void UpdatePartyUINavigation(UnityEngine.UI.Button button)
        {
            for (int i = 0; i < partyMemberUIInstances.Count; i++)
            {
                UnityEngine.UI.Button partyMemberButton = partyMemberUIInstances[i].button;

                Navigation navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i == 0 ? partyMemberUIInstances[partyMemberUIInstances.Count - 1].button : partyMemberUIInstances[i - 1].button,
                    selectOnDown = i == partyMemberUIInstances.Count - 1 ? partyMemberUIInstances[0].button : partyMemberUIInstances[i + 1].button,
                    selectOnLeft = button
                };

                partyMemberButton.navigation = navigation;
            }
        }

        public void UseConsideredItemOnUnit(SerializableAlly unit)
        {
            //TODO: Implement
            if(CanUseItemOnUnit(unit, consideredGameItem))
            {
                Debug.Log($"used {consideredGameItem.itemName} on {unit.UnitName}");
            }
        }

        private bool CanUseItemOnUnit(SerializableAlly unit, GameItem item)
        {
            //TODO: Implement
            return true;
        }
    }
}