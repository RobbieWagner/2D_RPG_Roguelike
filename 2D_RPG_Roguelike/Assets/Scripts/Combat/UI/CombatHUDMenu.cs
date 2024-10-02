using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatHUDMenu : CombatMenu
    {
        public void AddButtonToList(Action<int> onSelectedAction, Transform objectToFollow, Vector3 offset)
        {
            CombatMenuButton button = AddButtonToList(onSelectedAction);

            button.transform.position = objectToFollow.position + offset;
        }
    }
}