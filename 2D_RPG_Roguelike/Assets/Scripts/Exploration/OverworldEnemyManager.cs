using RobbieWagnerGames.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class OverworldEnemyManager : MonoBehaviourSingleton<ExplorationManager>
    {
        public Vector2 spawnDistance;
        public Coroutine spawnCoroutine = null;
        public List<OverworldEnemy> spawnedEnemies = new List<OverworldEnemy>();

        public IEnumerator SpawnLevelEnemies(int enemyCount)
        {
            int spawned = 0;

            while (spawned < enemyCount) 
            {
                yield return null;
                if (TrySpawnEnemy(null, out OverworldEnemy newInstance))
                {
                    spawnedEnemies.Add(newInstance);
                    spawned++;
                }
            }
        }

        private bool TrySpawnEnemy(Vector2? position, out OverworldEnemy newInstance)
        {
            if (!position.HasValue)
            {
                position = Random.insideUnitCircle * Random.Range(spawnDistance.x, spawnDistance.y);
            }

            List<OverworldEnemy> enemyOptions = ExplorationData.explorationConfiguration.enemyPrefabs;
            OverworldEnemy enemy = enemyOptions[Random.Range(0, enemyOptions.Count)];

            Vector3 spawnPosition = new Vector3(position.Value.x, 0, position.Value.y);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 5, NavMesh.AllAreas))
            {
                // If a valid position is found, spawn the AI agent at that position
                newInstance = Instantiate(enemy, hit.position, Quaternion.identity);
                return true;
            }

            newInstance = null;
            return false;
        }
    }
}