using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatMenuButton : MonoBehaviour
    {
        public Action<int> selectionAction = (int index) => { };
        [HideInInspector] public int buttonIndex;

        [SerializeField] private Button button;
        public TextMeshProUGUI buttonText;

        public void CheckForConsideration(int curIndex)
        {
            if(curIndex == buttonIndex)
            {
                // highlight this button
            }
            else
            {
                // Don't highlight this button
            }
        }

        public void StopConsideringButton()
        {

        }

        public void SelectButton(bool stopConsidering = true)
        {
            selectionAction?.Invoke(buttonIndex);
            if(stopConsidering)
                StopConsideringButton();
        }
    }
}
