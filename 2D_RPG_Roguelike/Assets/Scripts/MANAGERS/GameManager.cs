using RobbieWagnerGames.Utilities.SaveData;
using System;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum GameMode
    {
        NONE,
        EXPLORATION,
        COMBAT,
        EVENT
    }
    public class GameManager : MonoBehaviour
    {
        public static Action<GameMode> OnGameModeChanged = (GameMode gameMode) => { };
        private GameMode currentGameMode = GameMode.NONE;
        public GameMode CurrentGameMode
        {
            get 
            {
                return currentGameMode;
            }
            set
            {
                if (value == currentGameMode)
                    return;
                currentGameMode = value;
                OnGameModeChanged?.Invoke(currentGameMode);
            }
        }

        public static GameManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            StaticGameStats.persistentDataPath = Application.persistentDataPath;
            new JsonDataService();
        }
    }
}