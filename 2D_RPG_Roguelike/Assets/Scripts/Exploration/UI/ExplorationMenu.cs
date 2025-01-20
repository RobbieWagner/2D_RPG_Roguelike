using RobbieWagnerGames.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationMenu : MenuWithTabs
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputManager.Instance.gameControls.UI.Cancel.performed += ExitMenu;
            InputManager.Instance.gameControls.EXPLORATION.EscapeMenu.performed += ExitMenu;
            Time.timeScale = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Time.timeScale = 1;
        }

        private void ExitMenu(InputAction.CallbackContext context)
        {
            InputManager.Instance.gameControls.UI.Cancel.performed -= ExitMenu;
            InputManager.Instance.gameControls.EXPLORATION.EscapeMenu.performed -= ExitMenu;
            ExplorationManager.Instance.ToggleExplorationMenu(context);
        }
    }
}