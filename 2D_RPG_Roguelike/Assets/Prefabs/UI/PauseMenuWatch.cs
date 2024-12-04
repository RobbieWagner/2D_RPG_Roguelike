using RobbieWagnerGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class PauseMenuWatch : MonoBehaviour
    {
        [SerializeField] private PauseMenu pauseMenu;
        private List<string> pausedActionMaps;

        private void Awake()
        {
            InputManager.Instance.gameControls.UI.PauseGame.performed += TogglePauseMenu;
            InputManager.Instance.EnableActionMap(ActionMapName.UI.ToString());
            pausedActionMaps = new List<string>();
        }

        private void TogglePauseMenu(InputAction.CallbackContext context)
        {
            if (pauseMenu.enabled && pauseMenu.thisCanvas.enabled)
            {
                pauseMenu.paused = false;
                pauseMenu.enabled = false;
                foreach(string map in pausedActionMaps)
                    InputManager.Instance.EnableActionMap(map);
                pausedActionMaps.Clear();
            }
            else if (!pauseMenu.enabled)
            {
                pauseMenu.enabled = true;
                foreach (InputActionMap actionMap in InputManager.Instance.gameControls.asset.actionMaps)
                {
                    if (actionMap.enabled)
                        pausedActionMaps.Add(InputManager.Instance.actionMaps[actionMap.name].name);
                }
            }
        }

        private void OnDestroy()
        {
            InputManager.Instance.gameControls.UI.PauseGame.performed -= TogglePauseMenu;
        }
    }
}