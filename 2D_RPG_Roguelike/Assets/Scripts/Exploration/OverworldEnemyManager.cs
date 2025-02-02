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
        public float maxEnemyDistance = 100f;
        public Coroutine spawnCoroutine = null;
        private List<OverworldEnemy> spawnedEnemies = new List<OverworldEnemy>();

        protected override void Awake()
        {
            base.Awake();

            ExplorationManager.Instance.OnSetCurrentInteractable += HandlePlayerInteraction;
        }

        #region Spawning
        public IEnumerator SpawnLevelEnemies(int maxTries = 1000)
        {
            int minEnemiesToSpawn = ExplorationData.ExplorationConfiguration.enemyCountRange.x - spawnedEnemies.Count;
            int maxEnemiesToSpawn = ExplorationData.ExplorationConfiguration.enemyCountRange.y - spawnedEnemies.Count;

            int enemiesToSpawn = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn);

            yield return StartCoroutine(TrySpawnEnemies(enemiesToSpawn, maxTries));
        }

        public IEnumerator TrySpawnEnemies(int spawnCount, int maxTries = 1000)
        {
            int spawned = 0;
            int spawnTries = 0;

            while (spawned < spawnCount && spawnTries < maxTries)
            {
                spawnTries++;
                yield return null;
                if (TrySpawnEnemy(out OverworldEnemy newInstance))
                {
                    spawnedEnemies.Add(newInstance);
                    spawned++;
                    spawnTries = 0;
                }
            }

            if (spawned < spawnCount)
                Debug.LogWarning($"OverworldEnemyManager could only spawn {spawned}/{spawnCount} enemies");
        }

        public IEnumerator RefreshEnemies()
        {
            List<OverworldEnemy> removedEnemies = new List<OverworldEnemy>();
        
            foreach(OverworldEnemy enemy in spawnedEnemies)
            {
                //Debug.Log($"distance {Vector3.Distance(enemy.transform.position, PlayerMovement.Instance.transform.position)}");
                if(Vector3.Distance(enemy.transform.position, PlayerMovement.Instance.transform.position) > maxEnemyDistance)
                {
                    removedEnemies.Add(enemy);
                }
            }    

            foreach(OverworldEnemy enemy in removedEnemies)
            {
                Destroy(enemy.gameObject);
                spawnedEnemies.Remove(enemy);
            }

            yield return StartCoroutine(SpawnLevelEnemies());
        }

        private bool TrySpawnEnemy(out OverworldEnemy newInstance)
        {
            Vector2 position = Random.insideUnitCircle * Random.Range(spawnDistanceRange.x, spawnDistanceRange.y);
            return TrySpawnEnemy(out newInstance, position);
        }

        private bool TrySpawnEnemy(out OverworldEnemy newInstance, Vector2 position)
        {
            List<OverworldEnemy> enemyOptions = ExplorationData.ExplorationConfiguration.enemyPrefabs;
            OverworldEnemy enemy = enemyOptions[Random.Range(0, enemyOptions.Count)];

            Vector3 spawnPosition = new Vector3(position.x, 0, position.y);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 5, NavMesh.AllAreas))
            {
                newInstance = Instantiate(enemy, transform);
                NavMeshAgent agent = newInstance.GetComponentInChildren<NavMeshAgent>();
                agent.isStopped = true;
                agent.Warp(hit.position);
                //Debug.Log(hit.position);
                return true;
            }

            newInstance = null;
            return false;
        }

        public void RemoveEnemy(OverworldEnemy enemy)
        {
            if(spawnedEnemies.Contains(enemy))
            {
                Destroy(enemy.gameObject);
                spawnedEnemies.Remove(enemy);
            }
        }
        #endregion

        #region Instance Updates
        private void HandlePlayerInteraction(Interactable interactable)
        {
            if (interactable != null)
                PauseAllAgents();
            else
                ResumeAllAgents();
        }

        private void PauseAllAgents()
        {
            foreach (OverworldEnemy enemy in spawnedEnemies)
                enemy.PauseAgent();
        }

        public void ResumeAllAgents()
        {
            foreach (OverworldEnemy enemy in spawnedEnemies)
                enemy.ResumeAgent();
        }
        #endregion
    }
}