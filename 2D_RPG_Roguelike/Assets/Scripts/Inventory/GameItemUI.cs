using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class GameItemUI : MonoBehaviour
    {
        [SerializeField] private Image itemIconView;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        public ButtonListener OnSelectButton;

        private GameItem gameItem = null;
        public GameItem GameItem
        { 
            get 
            { 
                return gameItem; 
            } 
            set 
            {
                if (gameItem == value && value)
                    return;

                gameItem = value;
                OnSetNewGameItem(gameItem);
            }
        }

        private void OnSetNewGameItem(GameItem gameItem)
        {
            if(gameItem != null)
            {
                if (itemIconView != null)
                {
                    itemIconView.enabled = true;
                    itemIconView.sprite = gameItem.itemIcon;
                }
                if (itemNameText != null) itemNameText.text = gameItem.itemName;
                if (quantityText != null) quantityText.text = Inventory.inventory[gameItem].ToString();
                if (descriptionText != null) descriptionText.text = gameItem.description;
            }
            else
            {
                if (itemIconView != null)
                {
                    itemIconView.enabled = false;
                    itemIconView.sprite = null;
                }
                if (itemNameText != null) itemNameText.text = string.Empty;
                if (quantityText != null) quantityText.text = string.Empty;
                if (descriptionText != null) descriptionText.text = string.Empty;
            }
            
        }
    }
}