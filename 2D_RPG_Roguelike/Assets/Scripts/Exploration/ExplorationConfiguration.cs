using RobbieWagnerGames.Utilities.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat 
{
    [System.Serializable]
    public class ExplorationConfiguration
    {
        [Header("Scene and Position")]
        public string explorationSceneRef;
        public float playerPositionX;
        public float playerPositionY;
        public float playerPositionZ;

        [Header("Enemies")]
        public bool spawnEnemies = false;
        public Vector2Int enemyCountRange;
        public List<OverworldEnemy> enemyPrefabs;
    }

    public static class ExplorationData
    {
        private static ExplorationConfiguration explorationConfiguration = null;
        public static ExplorationConfiguration ExplorationConfiguration
        {
            get
            {
                if(explorationConfiguration == null)
                    explorationConfiguration = JsonDataService.Instance.LoadDataRelative<ExplorationConfiguration>(StaticGameStats.explorationDataSavePath, null);

                if(explorationConfiguration == null)
                    explorationConfiguration = GameManager.Instance.initialExplorationConfiguration;

                return explorationConfiguration;
            }
            set 
            {
                explorationConfiguration = value;
            }
        }
    }
}