using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.UI;
using RobbieWagnerGames.Utilities.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public static Action<GameMode, GameMode> OnGameModeEnded = (GameMode gameMode, GameMode nextGameMode) => { };
        private GameMode currentGameMode = GameMode.EXPLORATION; //GameMode.NONE //TODO: HAVE GAMEMODE INITIALIZED TO NONE
        private GameMode previousGameMode = GameMode.NONE;

        public List<Ally> initialParty;
        public List<GameItem> initialInventory;
        public ExplorationConfiguration initialExplorationConfiguration;
        public GameConfigurationData initialGameData;

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
                OnGameModeEnded?.Invoke(currentGameMode, value);
                previousGameMode = currentGameMode;
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

            OnGameModeEnded += OnEndGameMode;

            LoadGameData();
            StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            // Determines how to load the game correctly, and triggers the correct load
            yield return null;
            if (ExplorationManager.Instance != null)
                StartCoroutine(TriggerExploration(null));
            else if (GeneralGameData.gameData == null)
                StartCoroutine(StartNewGame());
            else if (ExplorationData.explorationConfiguration != null)
                StartCoroutine(TriggerExploration(ExplorationData.explorationConfiguration));
        }

        #region Game Modes
        private void OnEndGameMode(GameMode mode, GameMode nextGameMode)
        {
            if (mode == GameMode.EXPLORATION)
            {
                PlayerMovement.Instance.CeasePlayerMovement();
                ExplorationManager.Instance.IsObjectInteractionEnabled = false;
                
                if(nextGameMode != GameMode.EVENT)
                    PlayerMovement.Instance.spriteRenderer.enabled = false;
            }
            else if (mode == GameMode.COMBAT)
            {
                InputManager.Instance.gameControls.COMBAT.Disable();
            }
        }

        public IEnumerator TriggerCombat(CombatConfiguration combatInfo)
        {
            yield return null;
            yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn());

            SceneManager.LoadScene(combatInfo.combatSceneRef, LoadSceneMode.Additive);
            while (CombatBattlefield.Instance == null)
                yield return null;

            CombatManagerBase.Instance.StartCombat(combatInfo);
            CurrentGameMode = GameMode.COMBAT;
            OverworldEnemy.isCombatTriggered = false;
            CombatManagerBase.Instance.OnCombatEnded += TriggerPreviousGameMode;

            yield return StartCoroutine(ScreenCover.Instance.FadeCoverOut());
        }

        public IEnumerator TriggerExploration(ExplorationConfiguration explorationConfiguration, bool coverScreen = true)
        {
            yield return null;
            if(coverScreen)
                yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn());

            if(explorationConfiguration != null)
            {
                SceneManager.LoadScene(explorationConfiguration.explorationSceneRef, LoadSceneMode.Additive);
                while (ExplorationManager.Instance == null)
                    yield return null;
            }

            yield return StartCoroutine(ExplorationManager.Instance.StartExploration(explorationConfiguration));
            CurrentGameMode = GameMode.EXPLORATION;

            ExplorationManager.Instance.IsObjectInteractionEnabled = true;

            if(coverScreen)
                yield return StartCoroutine(ScreenCover.Instance.FadeCoverOut(.35f));
        }

        public void TriggerPreviousGameMode()
        {
            if (previousGameMode == GameMode.EXPLORATION)
            {
                if (currentGameMode == GameMode.COMBAT)
                    StartCoroutine(TriggerExploration(null));
                else if(currentGameMode == GameMode.EVENT)
                    StartCoroutine(TriggerExploration(null, false));
            }
        }

        #endregion

        #region Save/Load
        public void SaveGameData()
        {
            SavePlayerParty();
            SavePlayerInventory();
            SavePlayerExplorationData();

            JsonDataService.Instance.SaveData(StaticGameStats.gameDataSavePath, GeneralGameData.gameData);
        }

        private void SavePlayerExplorationData()
        {
            if (ExplorationManager.Instance != null)
            {
                ExplorationConfiguration explorationSaveData = new ExplorationConfiguration()
                {
                    explorationSceneRef = ExplorationManager.Instance.gameObject.scene.name,
                    playerPositionX = PlayerMovement.Instance.transform.position.x,
                    playerPositionY = PlayerMovement.Instance.transform.position.y,
                    playerPositionZ = PlayerMovement.Instance.transform.position.z
                };

                JsonDataService.Instance.SaveData(StaticGameStats.explorationDataSavePath, explorationSaveData);
            }
            else
                JsonDataService.Instance.SaveData<ExplorationConfiguration>(StaticGameStats.explorationDataSavePath, null);
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
            LoadPlayerExplorationData();

            GeneralGameData.gameData = JsonDataService.Instance.LoadDataRelative<GameConfigurationData>(StaticGameStats.gameDataSavePath, null);
        }

        private IEnumerator StartNewGame()
        {
            // TODO: Replace with logic that will start a new game !!!!!

            GeneralGameData.gameData = initialGameData;
            yield return StartCoroutine(TriggerExploration(initialExplorationConfiguration));
        }

        private void LoadPlayerExplorationData()
        {
            ExplorationConfiguration explorationData = JsonDataService.Instance.LoadDataRelative<ExplorationConfiguration>(StaticGameStats.explorationDataSavePath, null);
            ExplorationData.explorationConfiguration = explorationData;
        }

        private void LoadPlayerInventory()
        {
            List<string> gameItemFilePaths = JsonDataService.Instance.LoadDataRelative(StaticGameStats.inventorySavePath, new List<string>());

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