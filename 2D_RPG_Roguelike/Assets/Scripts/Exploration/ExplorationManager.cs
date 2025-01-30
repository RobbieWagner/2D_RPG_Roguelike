using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationManager : MonoBehaviourSingleton<ExplorationManager>
    {
        public ExplorationConfiguration currentExplorationConfiguration;
        public ExplorationMenu explorationMenu;

        private bool isObjectInteractionEnabled = false;
        public bool IsObjectInteractionEnabled
        {
            get
            {
                return isObjectInteractionEnabled;
            }
            set
            {
                if (isObjectInteractionEnabled == value)
                    return;
                isObjectInteractionEnabled = value;
            }
        }

        private Interactable currentInteractable = null;
        public Interactable CurrentInteractable
        {
            get 
            {
                return currentInteractable;
            }
            set 
            {
                if (currentInteractable == value) 
                    return;
                currentInteractable = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            InputManager.Instance.gameControls.EXPLORATION.ExplorationMenu.performed += EnableExplorationMenu;
        }

        public void EnableExplorationMenu(InputAction.CallbackContext context)
        {
            if(!explorationMenu.enabled && GameManager.Instance.CurrentGameMode == GameMode.EXPLORATION)
            {
                explorationMenu.enabled = true;
                explorationMenu.canvas.enabled = true;

                if (explorationMenu.enabled)
                    InputManager.Instance.DisableActionMap(ActionMapName.PAUSE.ToString());
            }
        }

        public void DisableExplorationMenu(InputAction.CallbackContext context)
        {
            if (explorationMenu.enabled)
            {
                explorationMenu.enabled = false;
                explorationMenu.canvas.enabled = false;

                if (!explorationMenu.enabled)
                    InputManager.Instance.EnableActionMap(ActionMapName.PAUSE.ToString());
            }
        }

        public IEnumerator StartExploration(ExplorationConfiguration explorationConfiguration)
        {
            currentExplorationConfiguration = explorationConfiguration; 

            yield return null;
            yield return StartCoroutine(SetupPlayerMovement(explorationConfiguration));

            InputManager.Instance.EnableActionMap(ActionMapName.EXPLORATION.ToString());
        }

        private IEnumerator SetupPlayerMovement(ExplorationConfiguration explorationConfiguration)
        {
            yield return null;
            PlayerMovement.Instance.CanMove = true;
            if (explorationConfiguration != null)
                PlayerMovement.Instance.Warp(new Vector3(explorationConfiguration.playerPositionX, explorationConfiguration.playerPositionY, explorationConfiguration.playerPositionZ));
        }

        public void ToggleInteractability(bool on)
        {
            if (on)
                InputManager.Instance.gameControls.EXPLORATION.Interact.Enable();
            else
                InputManager.Instance.gameControls.EXPLORATION.Interact.Disable();
        }

    }
}