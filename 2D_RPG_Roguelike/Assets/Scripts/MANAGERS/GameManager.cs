using JetBrains.Annotations;
using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.Utilities.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<GameItem> initialInventory;

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

            LoadGameData();
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
        public void SaveGameData()
        {
            SavePlayerParty();
            SavePlayerInventory();
            //SavePlayerExplorationData();
        }

        private void SavePlayerExplorationData()
        {
            throw new NotImplementedException();
        }

        private void SavePlayerInventory()
        {
            JsonDataService.Instance.SaveData(StaticGameStats.inventorySavePath, Inventory.inventory.Select(i => i.Key.filePath).ToList());
        }

        private void SavePlayerParty()
        {
            JsonDataService.Instance.SaveData(StaticGameStats.partySavePath, Party.party);
        }

        public void LoadGameData()
        {
            LoadPlayerParty();
            LoadPlayerInventory();
            //LoadPlayerExplorationData();
        }

        private void LoadPlayerExplorationData()
        {
            throw new NotImplementedException();
        }

        private void LoadPlayerInventory()
        {
            List<string> gameItemFilePaths = JsonDataService.Instance.LoadDataRelative<List<string>>(StaticGameStats.inventorySavePath, new List<string>());

            List<GameItem> inventory = new List<GameItem>();

            foreach (string itemFilePath in gameItemFilePaths)
                inventory.Add(Resources.Load<GameItem>(itemFilePath));

            if (inventory.Any())
                Inventory.AddItemsToInventory(inventory);
            else
                Inventory.AddItemsToInventory(initialInventory);
        }

        private void LoadPlayerParty()
        {
            Party.party = JsonDataService.Instance.LoadDataRelative<List<SerializableAlly>>(StaticGameStats.partySavePath, null);
        }
        #endregion
    }
}