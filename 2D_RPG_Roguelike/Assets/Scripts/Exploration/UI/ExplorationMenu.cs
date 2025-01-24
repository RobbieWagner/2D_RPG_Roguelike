using RobbieWagnerGames.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationMenu : MenuWithTabs
    {

        public Action<bool> OnCanCloseMenuChanged = (bool canClose) => { };
        private bool canCloseMenu = true;
        public bool CanCloseMenu
        {
            get 
            {
                return canCloseMenu;
            }
            set
            {
                if(canCloseMenu == value)
                    return;
                canCloseMenu = value;
                OnCanCloseMenuChanged?.Invoke(canCloseMenu);
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputManager.Instance.gameControls.UI.Cancel.performed += ExitMenu;
            InputManager.Instance.gameControls.EXPLORATION.EscapeMenu.performed += ExitMenu;
            InputManager.Instance.gameControls.EXPLORATION.ExplorationMenu.performed += ExitMenu;
            Time.timeScale = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Time.timeScale = 1;
        }

        private void ExitMenu(InputAction.CallbackContext context)
        {
            if (canCloseMenu)
            {
                InputManager.Instance.gameControls.UI.Cancel.performed -= ExitMenu;
                InputManager.Instance.gameControls.EXPLORATION.EscapeMenu.performed -= ExitMenu;
                InputManager.Instance.gameControls.EXPLORATION.ExplorationMenu.performed -= ExitMenu;
                ExplorationManager.Instance.DisableExplorationMenu(context);
            }
        }
    }
}