using System;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatHUDMenu : CombatMenu
    {
        public void AddButtonToList(Action<CombatMenuButton> onSelectedAction, Transform objectToFollow, Vector3 offset)
        {
            CombatMenuButton button = AddButtonToList(onSelectedAction);

            button.transform.position = objectToFollow.position + offset;
        }
    }
}