using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatMenuButton : MonoBehaviour
    {
        public Action<CombatMenuButton> selectionAction = (CombatMenuButton button) => { };
        [HideInInspector] public int buttonIndex;

        public Button button;
        public TextMeshProUGUI buttonText;

        private void Awake()
        {
            button.onClick.AddListener(() => SelectButton());
        }

        public void SelectButton()
        {
            selectionAction?.Invoke(this);
        }
    }
}
