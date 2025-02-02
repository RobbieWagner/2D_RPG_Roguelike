using RobbieWagnerGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ItemPickupInteractable : Interactable
    {
        [SerializeField] private GameItem item;
        [Range(1,99)]
        [SerializeField] private int amount = 1;

        protected override IEnumerator Interact()
        {
            if (Inventory.TryAddItemToInventory(item, amount))
            {
                NotificationsUI.Instance.StartCoroutine(NotificationsUI.Instance.DisplayNotification($"+{amount} {item.itemName}", item.itemIcon));
                
            }

            yield return StartCoroutine(base.Interact());

            canInteract = false;
            Destroy(gameObject);
        }
    }
}