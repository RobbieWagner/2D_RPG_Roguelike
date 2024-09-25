using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.Utilities.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    /// <summary>
    /// Handles SETUP phase of combat
    /// </summary>
    public partial class CombatManagerBase : MonoBehaviour
    {
        [Space(10)]
        [Header("Combat Setup")]
        public List<Vector2> allyPositionOffsets;
        public List<Vector2> enemyPositionOffsets;
        public Ally baseAllyPrefab;

        // NOTE: ASSUMES ALLIES NEED TO BE INSTANTIATED
        protected virtual IEnumerator SetupCombat()
        {
            //Debug.Log("Setting up Combat");
            yield return null;

            if (currentCombat.pullAlliesFromSave)
                allyInstances = InstantiateAlliesFromSave();
            else
                allyInstances = InstantiateAllies(currentCombat.allyPrefabs);

            InstantiateEnemies(currentCombat.enemyPrefabs);

            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }

        private List<Ally> InstantiateAlliesFromSave()
        {
            List<SerializableAlly> savedAllies = JsonDataService.Instance.LoadData<List<SerializableAlly>>(StaticGameStats.partySavePath , null);
            if (savedAllies != null)
            {
                List<Ally> result = new List<Ally>();
                for(int i = 0; i < savedAllies.Count; i++)
                {
                    SerializableAlly serializableAlly = savedAllies[i];
                    Ally newAlly = Instantiate(baseAllyPrefab, transform);
                    serializableAlly.InitializeAlly(ref newAlly);
                    result.Add(newAlly);
                }
                return result;
            }
            else
                throw new NullReferenceException("Error starting combat: could not find party data!!");
        }

        private List<Ally> InstantiateAllies(List<Ally> allyPrefabs)
        {
            List<Ally> newAllies = new List<Ally>();
            for(int i = 0; i < allyPrefabs.Count; i++)
            {
                Ally allyInstance = Instantiate(allyPrefabs[i], transform);
                allyInstance.transform.position = allyPositionOffsets[i];
                newAllies.Add(allyInstance);
            }
            return newAllies;
        }

        private List<Enemy> InstantiateEnemies(List<Enemy> enemyPrefabs)
        {
            List<Enemy> newAllies = new List<Enemy>();
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                Enemy enemyInstance = Instantiate(enemyPrefabs[i], transform);
                enemyInstance.transform.position = enemyPositionOffsets[i];
                newAllies.Add(enemyInstance);
            }
            return newAllies;
        }
    }
}