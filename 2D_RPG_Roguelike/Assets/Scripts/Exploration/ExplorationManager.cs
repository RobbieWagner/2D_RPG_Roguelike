using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationManager : MonoBehaviour
    {
        public ExplorationConfiguration currentExplorationConfiguration;

        public static ExplorationManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public IEnumerator StartExploration(ExplorationConfiguration explorationConfiguration)
        {
            currentExplorationConfiguration = explorationConfiguration; 

            yield return null;
            PlayerMovement.Instance.CanMove = true;
            if(explorationConfiguration != null)
                PlayerMovement.Instance.Warp(new Vector3(explorationConfiguration.playerPositionX, explorationConfiguration.playerPositionY, explorationConfiguration.playerPositionZ));
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnDisable()
        {
            Instance = null;
        }
    }
}