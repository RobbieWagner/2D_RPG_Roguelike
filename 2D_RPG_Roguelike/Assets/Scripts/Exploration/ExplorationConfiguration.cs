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
        public static ExplorationConfiguration explorationConfiguration;
    }
}