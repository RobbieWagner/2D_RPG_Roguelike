using DG.Tweening.Core;
using RobbieWagnerGames.UI;
using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class PauseMenuWatch : MonoBehaviourSingleton<PauseMenuWatch>
    {
        [SerializeField] private PauseMenu pauseMenu;
        private List<string> pausedActionMaps;

        protected override void Awake()
        {
            base.Awake();
            InputManager.Instance.gameControls.PAUSE.PauseGame.performed += EnablePauseMenu;
            InputManager.Instance.EnableActionMap(ActionMapName.PAUSE.ToString());
            pausedActionMaps = new List<string>();
        }

        private void EnablePauseMenu(InputAction.CallbackContext context)
        {
            if (!pauseMenu.enabled)
            {
                pauseMenu.enabled = true;
                foreach (InputActionMap actionMap in InputManager.Instance.gameControls.asset.actionMaps)
                {
                    if (actionMap.enabled)
                        pausedActionMaps.Add(InputManager.Instance.actionMaps[actionMap.name].name);
                }
            }
        }

        public void DisablePauseMenu(InputAction.CallbackContext context)
        {
            if (pauseMenu.enabled && pauseMenu.canvas.enabled)
            {
                pauseMenu.paused = false;
                pauseMenu.enabled = false;
                foreach (string map in pausedActionMaps)
                    InputManager.Instance.EnableActionMap(map);
                pausedActionMaps.Clear();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            InputManager.Instance.gameControls.PAUSE.PauseGame.performed -= EnablePauseMenu;
        }
    }
}