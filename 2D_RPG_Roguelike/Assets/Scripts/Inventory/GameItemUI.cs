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

        private GameItem gameItem = null;
        public GameItem GameItem
        { 
            get 
            { 
                return gameItem; 
            } 
            set 
            {
                if (gameItem == value && value != null)
                    return;

                gameItem = value;
                OnSetNewGameItem(gameItem);
            }
        }

        private void OnSetNewGameItem(GameItem gameItem)
        {
            if (itemIconView != null) itemIconView.sprite = gameItem.itemIcon;
            if (itemNameText != null) itemNameText.text = gameItem.name;
            if (quantityText != null) quantityText.text = Inventory.inventory[gameItem].ToString();
            if (descriptionText != null) descriptionText.text = gameItem.description;
        }
    }
}