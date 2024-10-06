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
        public static Action<GameMode> OnGameModeEnded = (GameMode gameMode) => { };
        private GameMode currentGameMode = GameMode.EXPLORATION; //GameMode.NONE //TODO: HAVE GAMEMODE INITIALIZED TO NONE
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
                OnGameModeEnded?.Invoke(currentGameMode);
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

            OnGameModeChanged += OnChangeGameMode;
            OnGameModeEnded += OnEndGameMode;
        }

        private void OnEndGameMode(GameMode mode)
        {
            if(mode == GameMode.EXPLORATION)
            {
                CharacterMovement2D.Instance.enabled = false;
                CharacterMovement2D.Instance.spriteRenderer.enabled = false;
            }
        }

        private void OnChangeGameMode(GameMode mode)
        {
            if(mode == GameMode.EXPLORATION)
            {
                CharacterMovement2D.Instance.enabled = true;
                CharacterMovement2D.Instance.spriteRenderer.enabled = true;
            }
        }
    }
}