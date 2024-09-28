using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum CombatPhase
    {
        NONE,
        SETUP,
        ACTION_SELECTION,
        ACTION_EXECUTION,
        WIN,
        LOSE
    }

    public partial class CombatManagerBase : MonoBehaviour
    {
        [SerializeField] private bool debugEnabled = false;
        [SerializeField] private DebugLogSequenceEvent debugSequenceEventPrefab;
        [SerializeField] private CombatConfiguration debugCombatConfiguration;

        protected List<Ally> allyInstances = new List<Ally>();
        protected List<Ally> activeAllies => allyInstances.Where(a => a.isUnitFighting).ToList();
        protected List<Enemy> enemyInstances = new List<Enemy>();
        protected List<Enemy> activeEnemies => enemyInstances.Where(a => a.isUnitFighting).ToList();
        protected bool IsUnitInCombatAlly(Unit unit) => unit.GetType() == typeof(Ally);
        protected bool IsUnitInCombatEnemy(Unit unit) => unit.GetType() == typeof(Enemy);

        public static Action<CombatPhase> OnCombatPhaseChange = (CombatPhase phase) => { };
        private Coroutine phaseChangeCoroutine;
        private CombatPhase currentCombatPhase = CombatPhase.NONE;
        public CombatPhase CurrentCombatPhase
        {
            get 
            {
                return currentCombatPhase;
            }
            set 
            {
                if(currentCombatPhase == value || phaseChangeCoroutine != null)
                    return;

                phaseChangeCoroutine = StartCoroutine(SetCombatPhaseCo(value));
            }
        }

        [HideInInspector] public CombatConfiguration currentCombat = null;

        public static CombatManagerBase Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            SetupCombatEventHandlers();

            if (debugEnabled)
                StartDebugCombat();
        }

        private void StartDebugCombat()
        {
            foreach (CombatEventTriggerType eventType in Enum.GetValues(typeof(CombatEventTriggerType)))
            {
                SubscribeEventToCombatEventHandler(Instantiate(debugSequenceEventPrefab, transform), eventType);
                debugSequenceEventPrefab.message = eventType.ToString();
            }

            StartCombat(debugCombatConfiguration);
        }

        public virtual bool StartCombat(CombatConfiguration combatConfiguration)
        {
            currentCombat = combatConfiguration;
            OnCombatPhaseChange += StartCombatPhase;
            CurrentCombatPhase = CombatPhase.SETUP;
            return true;
        }

        public virtual bool EndCombat()
        {
            currentCombat = null;
            OnCombatPhaseChange -= StartCombatPhase;
            CurrentCombatPhase = CombatPhase.NONE;
            return true;
        }

        protected virtual void StartCombatPhase(CombatPhase phase)
        {
            switch (phase)
            {
                case CombatPhase.SETUP:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.SETUP_STARTED, () => { StartCoroutine(SetupCombat()); }));
                    break;
                case CombatPhase.ACTION_SELECTION:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.SELECTION_PHASE_STARTED, () => { StartCoroutine(HandleSelectionPhase()); }));
                    break;
                case CombatPhase.ACTION_EXECUTION:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.EXECUTION_PHASE_STARTED, () => { StartCoroutine(HandleExecutionPhase()); }));
                    break;
                case CombatPhase.WIN:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.COMBAT_WON, () => { StartCoroutine(HandleWinState()); }));
                    break;
                case CombatPhase.LOSE:
                    StartCoroutine(WaitForCombatEvents(CombatEventTriggerType.COMBAT_LOST, () => { StartCoroutine(HandleLoseState()); }));
                    break;
                default:
                    throw new ArgumentException($"Invalid combat phase used {phase}");
            }
        }

        protected virtual IEnumerator HandleWinState()
        {
            Debug.Log("Handle Win State");
            yield return new WaitForSeconds(2f);
        }

        protected virtual IEnumerator HandleLoseState()
        {
            Debug.Log("Handle Lose State");
            yield return new WaitForSeconds(2f);
        }

        public IEnumerator SetCombatPhaseCo(CombatPhase phase)
        {
            // First, end the current phase of combat (Check for events that trigger)
            yield return StartCoroutine(EndCurrentPhase());

            // switch the phases
            currentCombatPhase = phase;
            OnCombatPhaseChange?.Invoke(currentCombatPhase);

            phaseChangeCoroutine = null;
        }

        protected virtual IEnumerator EndCurrentPhase()
        {
            switch (currentCombatPhase)
            {
                case CombatPhase.SETUP:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.SETUP_COMPLETE));
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.COMBAT_STARTED));
                    break;
                case CombatPhase.ACTION_SELECTION:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.SELECTION_PHASE_ENDED));
                    break;
                case CombatPhase.ACTION_EXECUTION:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.EXECUTION_PHASE_ENDED));
                    break;
                case CombatPhase.WIN:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.COMBAT_TERMINATED));
                    break;
                case CombatPhase.LOSE:
                    yield return StartCoroutine(RunCombatEvents(CombatEventTriggerType.COMBAT_TERMINATED));
                    break;
                case CombatPhase.NONE:
                    break;
                default:
                    throw new NotImplementedException($"Functionality for combat phase {currentCombatPhase} is not implemented for the current kind of combat!!");
            }
        }

        protected List<Unit> GetUnitsByInitiative()
        {
            List<Unit> result = new List<Unit>();
            result.AddRange(enemyInstances);
            result.AddRange(allyInstances);
            return result.OrderBy(x => x.GetInitiativeBoost()).ToList();
        }

        private List<Unit> GetAllCombatUnits()
        {
            List<Unit> units = new List<Unit>();
            units.AddRange(allyInstances);
            units.AddRange(enemyInstances);

            return units;
        }

        protected virtual bool CheckForCombatCompletion()
        { 
            foreach(Unit unit in GetAllCombatUnits())
            {
                if(unit.HP == 0)
                    unit.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
                else
                    unit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }

            if(!activeAllies.Any())
            {
                Debug.Log("combat lost, changing phase to LOSE");
                CurrentCombatPhase = CombatPhase.LOSE;
                return true;
            }
            if (!activeEnemies.Any())
            {
                Debug.Log("combat won, changing phase to WIN");
                CurrentCombatPhase = CombatPhase.WIN;
                return true;
            }

            return false;
        }
    }
}