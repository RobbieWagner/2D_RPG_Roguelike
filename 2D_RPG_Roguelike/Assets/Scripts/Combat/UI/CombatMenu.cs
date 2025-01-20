using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatMenu : MonoBehaviour
    {
        [HideInInspector] public List<CombatMenuButton> optionInstances = new List<CombatMenuButton>();
        [SerializeField] protected CombatMenuButton optionPrefab;

        public CombatMenuButton AddButtonToList(Action<CombatMenuButton> onSelectedAction, string selectionText = null)
        {
            CombatMenuButton newButtonInstance = Instantiate(optionPrefab, transform);
            if (newButtonInstance.buttonText != null)
            {
                newButtonInstance.buttonText.text = selectionText;
                newButtonInstance.gameObject.name = selectionText;
            }
            newButtonInstance.selectionAction += onSelectedAction;
            newButtonInstance.buttonIndex = optionInstances.Count;

            optionInstances.Add(newButtonInstance);
            return newButtonInstance;
        }

        public void InitailizeNavigation()
        {
            for (int i = 0; i < optionInstances.Count; i++) 
            {
                CombatMenuButton button = optionInstances[i];

                Navigation navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i == 0 ? optionInstances[optionInstances.Count-1].button : optionInstances[i - 1].button,
                    selectOnDown = i == optionInstances.Count - 1 ? optionInstances[0].button : optionInstances[i + 1].button
                };

                button.button.navigation = navigation;
            }
        }
    }
}
