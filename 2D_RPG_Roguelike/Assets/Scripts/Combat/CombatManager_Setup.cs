using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using RobbieWagnerGames.Utilities.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] protected Image fightOverlay;
        [SerializeField] protected Animator fightOverlayAnimator;

        // NOTE: ASSUMES ALLIES NEED TO BE INSTANTIATED
        protected virtual IEnumerator SetupCombat()
        {
            //Debug.Log("Setting up Combat");
            yield return null;

            if (currentCombat.pullAlliesFromSave)
                allyInstances = InstantiatePlayerParty();
            else
                allyInstances = InstantiateAllies(currentCombat.allyPrefabs);

            enemyInstances = InstantiateEnemies(currentCombat.enemyPrefabs);

            HookupUnitGameplayEvents();
            AssignUnitCombatPositions();

            yield return StartCoroutine(DisplayFightOverlay());

            CompleteSetup();
        }

        private void AssignUnitCombatPositions()
        {
            //Assigns a combat position value to all units. Used in Selection phase to order the units properly
            int i = 0;
            foreach(Ally ally in allyInstances.OrderByDescending(a => a.transform.position.z))
            {
                ally.unitCombatPos = i;
                i++;
            }
            foreach (Enemy enemy in enemyInstances.OrderByDescending(a => a.transform.position.z))
            {
                enemy.unitCombatPos = i;
                i++;
            }
        }

        private void HookupUnitGameplayEvents()
        {
            // Logic to hookup events on units to UI and other methods
            foreach (Ally ally in allyInstances)
            {
                ally.OnHPLowered += CombatExecutionUI.Instance.DisplayHPChange;
                ally.OnHPRaised += CombatExecutionUI.Instance.DisplayHPChange;
                ally.OnStatChanged += CombatExecutionUI.Instance.DisplayEffectText;
            }
            foreach (Enemy enemy in enemyInstances)
            {
                enemy.OnHPLowered += CombatExecutionUI.Instance.DisplayHPChange;
                enemy.OnHPRaised += CombatExecutionUI.Instance.DisplayHPChange;
                enemy.OnStatChanged += CombatExecutionUI.Instance.DisplayEffectText;
            }
        }

        private void UnhookUnitGameplayEvents()
        {
            foreach (Ally ally in allyInstances)
            {
                ally.OnHPLowered -= CombatExecutionUI.Instance.DisplayHPChange;
                ally.OnHPRaised -= CombatExecutionUI.Instance.DisplayHPChange;
                ally.OnStatChanged -= CombatExecutionUI.Instance.DisplayEffectText;
            }
            foreach (Enemy enemy in enemyInstances)
            {
                enemy.OnHPLowered -= CombatExecutionUI.Instance.DisplayHPChange;
                enemy.OnHPRaised -= CombatExecutionUI.Instance.DisplayHPChange;
                enemy.OnStatChanged -= CombatExecutionUI.Instance.DisplayEffectText;
            }
        }

        private IEnumerator DisplayFightOverlay()
        {
            fightOverlayAnimator.SetTrigger("flash");
            yield return null;
            fightOverlay.enabled = true;

            while (!fightOverlayAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("idle", StringComparison.CurrentCultureIgnoreCase))
                yield return null;

            fightOverlay.enabled = false;
        }

        protected List<Ally> InstantiatePlayerParty()
        {
            if (Party.party != null && Party.party.Any())
            {
                List<Ally> result = new List<Ally>();
                for(int i = 0; i < Party.party.Count; i++)
                {
                    SerializableAlly serializableAlly = Party.party[i];
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
            for(int i = 0; i < allyPrefabs.Count && i < 3; i++)
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
            for (int i = 0; i < enemyPrefabs.Count && i < 3; i++)
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