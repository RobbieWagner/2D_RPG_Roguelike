using AYellowpaper.SerializedCollections;
using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class PartyMenuUnitUI : MonoBehaviour
    {
        [SerializeField] private Image characterVisual;
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider xpSlider;
        [SerializeField][SerializedDictionary("Stat", "TextMeshProObject")] SerializedDictionary<StatType,TextMeshProUGUI> statTexts;

        private SerializableAlly unit = null;
        public SerializableAlly Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
                SyncUI();
            }
        }

        private void SyncUI()
        {
            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
            {
                bool hasText = statTexts.TryGetValue(stat, out TextMeshProUGUI text);
                if (hasText)
                    text.text = unit.GetStatValue(stat).ToString();
            }

            hpSlider.maxValue = unit.MaxHP;
            hpSlider.value = unit.HP;
            characterVisual.sprite = Resources.Load<Sprite>(StaticGameStats.characterSpriteFilePath + unit.dialogueSpriteRelativePath);
            unitNameText.text = unit.UnitName;
        }
    }
}