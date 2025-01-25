using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class InventoryMenuPartyMemberUI : MonoBehaviour
    {
        public Button button;
        public InventoryMenu inventoryMenu;

        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;

        private SerializableAlly unit;
        public SerializableAlly Unit
        {
            get
            {
                return unit;
            }
            set 
            {
                if(unit == value)
                    return;
                unit = value;
                SyncToUnit();
            }
        }

        public void SyncToUnit()
        {
            unitNameText.text = unit.UnitName;
            hpSlider.maxValue = unit.MaxHP;
            hpSlider.value = unit.HP;
            hpText.text = $"{unit.HP}/{unit.MaxHP}";
        }

        public void Initialize()
        {
            button.onClick.AddListener(OnSelectPartyMember);
        }

        private void OnSelectPartyMember()
        {
            if(inventoryMenu.SelectedGameItem.GetType() == typeof(CombatItem))
                inventoryMenu.UseSelectedCombatItemOnUnit(unit);
        }
    }
}