using JetBrains.Annotations;
using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.Utilities.SaveData;
using System;
using System.Collections.Generic;
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
        public List<Ally> initialParty;

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

            if(JsonDataService.Instance == null)
                new JsonDataService();

            OnGameModeChanged += OnChangeGameMode;
            OnGameModeEnded += OnEndGameMode;
        }

        private void OnEndGameMode(GameMode mode)
        {
            if(mode == GameMode.EXPLORATION)
            {
                PlayerMovement.Instance.CeasePlayerMovement();
                PlayerMovement.Instance.spriteRenderer.enabled = false;
            }
        }

        private void OnChangeGameMode(GameMode mode)
        {
            if(mode == GameMode.EXPLORATION)
                PlayerMovement.Instance.CanMove = true;
        }

        #region Save/Load
        public void SaveGame()
        {
            SavePlayerParty();
            SavePlayerInventory();
            SavePlayerExplorationData();
        }

        private void SavePlayerExplorationData()
        {
            throw new NotImplementedException();
        }

        private void SavePlayerInventory()
        {
            throw new NotImplementedException();
        }

        private void SavePlayerParty()
        {
            throw new NotImplementedException();
        }

        public void LoadGame()
        {
            LoadPlayerParty();
            LoadPlayerInventory();
            LoadPlayerExplorationData();
        }

        private void LoadPlayerExplorationData()
        {
            throw new NotImplementedException();
        }

        private void LoadPlayerInventory()
        {
            throw new NotImplementedException();
        }

        private void LoadPlayerParty()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}