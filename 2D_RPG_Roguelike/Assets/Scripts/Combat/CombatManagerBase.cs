using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
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
        protected List<Enemy> enemyInstances = new List<Enemy>();

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

        protected virtual IEnumerator HandleSelectionPhase()
        {
            //Debug.Log("Handle selection state");
            yield return new WaitForSeconds(2f);
            CurrentCombatPhase = CombatPhase.ACTION_EXECUTION;
        }

        protected virtual IEnumerator HandleExecutionPhase()
        {
            //Debug.Log("Handle execution state");
            yield return new WaitForSeconds(2f);
            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }

        protected virtual IEnumerator HandleWinState()
        {
            //Debug.Log("Handle Win State");
            yield return new WaitForSeconds(2f);
        }

        protected virtual IEnumerator HandleLoseState()
        {
            //Debug.Log("Handle Lose State");
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
    }
}