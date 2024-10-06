using RobbieWagnerGames.StrategyCombat;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class UnitUI : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI nameText;

        [SerializeField] protected TextMeshProUGUI hpText;
        [SerializeField] protected Slider hpSlider;

        private Unit unit;
        public Unit Unit
        {
            get
            { 
                return unit; 
            }
            set 
            {
                if (unit == value)
                    return;
                UnsubscribeUnit();
                unit = value;
                SubscribeUnit();
            }
        }
        

        private void Awake()
        {
            
        }

        private void UnsubscribeUnit()
        {
            if (unit != null)
                unit.OnHPChanged -= UpdateUnitHP;
        }

        private void SubscribeUnit()
        {
            if (unit != null)
            {
                unit.OnHPChanged += UpdateUnitHP;
                nameText.text = unit.unitName;
                UpdateUnitHP(unit.HP);
            }
        }

        private void UpdateUnitHP(int hpValue = -1)
        {
            hpText.text = $"{hpValue}/{unit.GetMaxHP()}";
            hpSlider.maxValue = unit.GetMaxHP();
            hpSlider.minValue = 0;
            hpSlider.value = hpValue;
        }
    }
}