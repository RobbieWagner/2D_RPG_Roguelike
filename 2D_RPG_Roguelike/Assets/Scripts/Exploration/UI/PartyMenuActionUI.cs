using RobbieWagnerGames.StrategyCombat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class PartyMenuActionUI : Selectable
    {
        [SerializeField] private RectTransform containerTransform;
        [SerializeField] private Image actionIcon;
        [SerializeField] private TextMeshProUGUI actionName;
        [SerializeField] private TextMeshProUGUI actionDescription;

        private CombatAction combatAction = null;
        public CombatAction CombatAction
        {
            get 
            {
                return combatAction;
            }
            set 
            {
                combatAction = value;
                SyncUI();
            }
        }

        private void SyncUI()
        {
            actionIcon.sprite = CombatAction.actionIcon;
            actionName.text = CombatAction.actionName;
            actionDescription.text = CombatAction.description;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            actionDescription.gameObject.SetActive(true);
            containerTransform.sizeDelta = new Vector2 (containerTransform.sizeDelta.x, 100);

            base.OnSelect(eventData);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            actionDescription.gameObject.SetActive(false);
            containerTransform.sizeDelta = new Vector2(containerTransform.sizeDelta.x, 32);

            base.OnDeselect(eventData);
        }
    }
}