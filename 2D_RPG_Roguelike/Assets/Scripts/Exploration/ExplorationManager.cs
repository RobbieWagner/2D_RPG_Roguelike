using RobbieWagnerGames.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationManager : MonoBehaviour
    {
        public static ExplorationManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public IEnumerator StartExploration()
        {
            yield return null;
            PlayerMovement.Instance.CanMove = true;
            GameManager.Instance.CurrentGameMode = GameMode.EXPLORATION;
        }
    }
}