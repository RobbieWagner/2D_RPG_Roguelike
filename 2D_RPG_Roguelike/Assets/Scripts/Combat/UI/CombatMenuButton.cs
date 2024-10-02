using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatMenuButton : MonoBehaviour
    {
        public Action<int> selectionAction = (int index) => { };
        [HideInInspector] public int buttonIndex;

        [SerializeField] private Button button;
        public TextMeshProUGUI buttonText;

        private void Awake()
        {
            button.onClick.AddListener(() => SelectButton());
        }

        public void CheckForConsideration(int curIndex)
        {
            if(curIndex == buttonIndex)
                EventSystemManager.Instance.eventSystem.SetSelectedGameObject(gameObject);
        }

        public void StopConsideringButton()
        {
            if(EventSystemManager.Instance.eventSystem.currentSelectedGameObject == gameObject)
                EventSystemManager.Instance.eventSystem.SetSelectedGameObject(null);
        }

        public void SelectButton(bool stopConsidering = true)
        {
            selectionAction?.Invoke(buttonIndex);
            if(stopConsidering)
                StopConsideringButton();
        }
    }
}
