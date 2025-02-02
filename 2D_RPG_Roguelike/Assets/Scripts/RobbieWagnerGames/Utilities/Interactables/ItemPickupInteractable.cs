using RobbieWagnerGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ItemPickupInteractable : Interactable
    {
        [SerializeField] private GameItem item;

        protected override IEnumerator Interact()
        {
            return base.Interact();
        }
    }
}