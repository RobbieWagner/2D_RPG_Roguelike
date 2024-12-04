using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatMenu : MonoBehaviour
    {
        protected List<CombatMenuButton> optionInstances = new List<CombatMenuButton>();
        [SerializeField] protected CombatMenuButton optionPrefab;

        public Action<int> OnChangeMenuSelectionIndex = (int index) => { };
        protected int curIndex = -1;
        public int CurIndex
        {
            get 
            {
                return curIndex;
            }
            set 
            {
                if(curIndex == value)
                    return;
                curIndex = value;
                OnChangeMenuSelectionIndex?.Invoke(curIndex);
            }
        }

        private void Awake()
        {
             
        }

        public CombatMenuButton AddButtonToList(Action<int> onSelectedAction, string selectionText = null)
        {
            CombatMenuButton newButtonInstance = Instantiate(optionPrefab, transform);
            if (newButtonInstance.buttonText != null)
            {
                newButtonInstance.buttonText.text = selectionText;
                newButtonInstance.gameObject.name = selectionText;
            }
            newButtonInstance.selectionAction += onSelectedAction;
            newButtonInstance.buttonIndex = optionInstances.Count;
            OnChangeMenuSelectionIndex += newButtonInstance.CheckForConsideration;

            optionInstances.Add(newButtonInstance);
            return newButtonInstance;
        }

        public void NavigateMenu(InputAction.CallbackContext context)
        {
            float delta = context.ReadValue<float>();

            if (delta > 0f)
                CurIndex = Math.Clamp(CurIndex + 1, 0, optionInstances.Count - 1);
            else if (delta < 0f)
                CurIndex = Math.Clamp(CurIndex - 1, 0, optionInstances.Count - 1);
        }

        public void SelectCurrentButton(InputAction.CallbackContext context)
        {
            if (CurIndex < optionInstances.Count && CurIndex >= 0)
                optionInstances[CurIndex].SelectButton();
        }
    }
}
