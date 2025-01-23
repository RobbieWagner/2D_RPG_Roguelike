using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ButtonListener : Button
    {
        public Action<Button> OnButtonSelected = (Button button) => { };

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnButtonSelected?.Invoke(this);
        }
    }
}