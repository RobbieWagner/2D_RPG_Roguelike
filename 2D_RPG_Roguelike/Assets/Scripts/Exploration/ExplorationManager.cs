using RobbieWagnerGames.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationManager : MonoBehaviourSingleton<ExplorationManager>
    {
        public ExplorationMenu explorationMenu;
        public static float explorationTimer { get; protected set; }
        [SerializeField] private float refreshInterval;
        private Coroutine refreshCoroutine = null;

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

        public IEnumerator StartExploration(ExplorationConfiguration explorationConfiguration)
        {
            if(explorationConfiguration != null)
                ExplorationData.ExplorationConfiguration = explorationConfiguration; 

            yield return null;
            yield return StartCoroutine(SetupPlayerMovement(explorationConfiguration));
            
            if (explorationConfiguration != null)
            {
                if (explorationConfiguration.spawnEnemies && explorationConfiguration.enemyPrefabs.Any())
                    StartCoroutine(OverworldEnemyManager.Instance.SpawnLevelEnemies());
            }
            else
            {
                StartCoroutine(OverworldEnemyManager.Instance.RefreshEnemies());
            }

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

        public void EnableExplorationMenu(InputAction.CallbackContext context)
        {
            if (!explorationMenu.enabled && GameManager.Instance.CurrentGameMode == GameMode.EXPLORATION)
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

        private void Update()
        {
            if (GameManager.Instance.CurrentGameMode == GameMode.EXPLORATION)
                UpdateExplorationTimer();
        }

        private void UpdateExplorationTimer()
        {
            //Debug.Log($"{explorationTimer}");
            explorationTimer += Time.deltaTime;

            if (refreshCoroutine == null && explorationTimer > 1 && (int) explorationTimer % refreshInterval == 0)
                refreshCoroutine = StartCoroutine(RefreshExplorationMode());
        }

        private IEnumerator RefreshExplorationMode()
        {
            //Debug.Log($"refreshing at {explorationTimer}");
            yield return StartCoroutine(OverworldEnemyManager.Instance.RefreshEnemies());

            yield return new WaitForSeconds(1.25f);
            refreshCoroutine = null;
        }
    }
}