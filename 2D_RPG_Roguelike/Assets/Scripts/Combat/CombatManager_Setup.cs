using RobbieWagnerGames.StrategyCombat;
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
        public List<Vector3> allyPositionOffsets;
        public List<Vector3> enemyPositionOffsets;
        public Ally baseAllyPrefab;

        [SerializeField] protected UnitUI unitUIPrefab;
        protected List<UnitUI> allyUIInstances = new List<UnitUI>();

        // NOTE: ASSUMES ALLIES NEED TO BE INSTANTIATED
        protected virtual IEnumerator SetupCombat()
        {
            //Debug.Log("Setting up Combat");
            yield return null;

            PlayerMovement.Instance.CanMove = false;

            if (currentCombat.pullAlliesFromSave)
                allyInstances = InstantiateAlliesFromSave();
            else
                allyInstances = InstantiateAllies(currentCombat.allyPrefabs);

            enemyInstances = InstantiateEnemies(currentCombat.enemyPrefabs);

            CompleteSetup();
        }

        protected List<Ally> InstantiateAlliesFromSave()
        {
            List<SerializableAlly> savedAllies = JsonDataService.Instance.LoadDataRelative<List<SerializableAlly>>(StaticGameStats.partySavePath , null);
            if (savedAllies != null)
            {
                List<Ally> result = new List<Ally>();
                for(int i = 0; i < savedAllies.Count; i++)
                {
                    SerializableAlly serializableAlly = savedAllies[i];
                    Ally newAlly = Instantiate(baseAllyPrefab, transform);
                    serializableAlly.InitializeAlly(ref newAlly);
                    result.Add(newAlly);

                    PlaceUnit(i, newAlly);
                }
                return result;
            }
            else
            {
                Debug.Log("loading first time party data");

                return InstantiateAllies(GameManager.Instance.initialParty);
            }
        }

        protected List<Ally> InstantiateAllies(List<Ally> allyPrefabs)
        {
            List<Ally> newAllies = new List<Ally>();
            for(int i = 0; i < allyPrefabs.Count; i++)
            {
                Ally allyInstance = Instantiate(allyPrefabs[i], transform);
                allyInstance.name = $"ally_{i}";
                allyInstance.unitName += $"_{i}";
                //allyInstance.HP = allyInstance.GetMaxHP();
                newAllies.Add(allyInstance);
                
                PlaceUnit(i, allyInstance);
            }
            return newAllies;
        }

        private void PlaceUnit(int i, Ally newAlly)
        {
            newAlly.transform.localPosition = allyPositionOffsets[i];
            UnitUI unitUI = Instantiate(unitUIPrefab, unitUIParent.transform);
            unitUI.Unit = newAlly;
            allyUIInstances.Add(unitUI);
        }

        protected List<Enemy> InstantiateEnemies(List<Enemy> enemyPrefabs)
        {
            List<Enemy> newEnemies = new List<Enemy>();
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                Enemy enemyInstance = Instantiate(enemyPrefabs[i], transform);
                enemyInstance.name = $"enemy_{i}";
                enemyInstance.unitName += $"_{i}";
                enemyInstance.HP = enemyInstance.GetMaxHP();
                enemyInstance.transform.localPosition = enemyPositionOffsets[i];
                newEnemies.Add(enemyInstance);
            }
            return newEnemies;
        }

        protected virtual void CompleteSetup()
        {
            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }
    }
}