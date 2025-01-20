using RobbieWagnerGames.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationManager : MonoBehaviourSingleton<ExplorationManager>
    {
        public ExplorationConfiguration currentExplorationConfiguration;
        public ExplorationMenu explorationMenu;

        protected override void Awake()
        {
            base.Awake();

            InputManager.Instance.gameControls.EXPLORATION.ExplorationMenu.performed += ToggleExplorationMenu;
        }

        public void ToggleExplorationMenu(InputAction.CallbackContext context)
        {
            explorationMenu.enabled = !explorationMenu.enabled;
            explorationMenu.canvas.enabled = explorationMenu.enabled;

            if (explorationMenu.enabled)
                InputManager.Instance.DisableActionMap(ActionMapName.PAUSE.ToString());
            else
                InputManager.Instance.EnableActionMap(ActionMapName.PAUSE.ToString());
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
    }
}