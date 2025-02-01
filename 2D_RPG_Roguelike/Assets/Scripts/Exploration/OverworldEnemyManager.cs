using RobbieWagnerGames.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class OverworldEnemyManager : MonoBehaviourSingleton<OverworldEnemyManager>
    {
        public Vector2 spawnDistanceRange;
        public Coroutine spawnCoroutine = null;
        public List<OverworldEnemy> spawnedEnemies = new List<OverworldEnemy>();

        public IEnumerator SpawnLevelEnemies()
        {
            int spawned = 0;
            int enemiesToSpawn = Random.Range(ExplorationData.explorationConfiguration.enemyCountRange.x, ExplorationData.explorationConfiguration.enemyCountRange.y);

            while (spawned < enemiesToSpawn) 
            {
                yield return null;
                if (TrySpawnEnemy(out OverworldEnemy newInstance))
                {
                    spawnedEnemies.Add(newInstance);
                    spawned++;
                }
            }
        }

        private bool TrySpawnEnemy(out OverworldEnemy newInstance)
        {
            Vector2 position = Random.insideUnitCircle * Random.Range(spawnDistanceRange.x, spawnDistanceRange.y);
            return TrySpawnEnemy(out newInstance, position);
        }

        private bool TrySpawnEnemy(out OverworldEnemy newInstance, Vector2 position)
        {
            List<OverworldEnemy> enemyOptions = ExplorationData.explorationConfiguration.enemyPrefabs;
            OverworldEnemy enemy = enemyOptions[Random.Range(0, enemyOptions.Count)];

            Vector3 spawnPosition = new Vector3(position.x, 0, position.y);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 5, NavMesh.AllAreas))
            {
                newInstance = Instantiate(enemy, transform);
                NavMeshAgent agent = newInstance.GetComponentInChildren<NavMeshAgent>();
                agent.isStopped = true;
                agent.Warp(hit.position);
                return true;
            }

            newInstance = null;
            return false;
        }
    }
}