using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum CombatEventTriggerType
    {
        NONE = -1,

        SETUP_STARTED = 1,
        SETUP_COMPLETE = 2,

        COMBAT_STARTED = 3,

        SELECTION_PHASE_STARTED = 4,
        SELECTION_PHASE_ENDED = 5,

        EXECUTION_PHASE_STARTED = 6,
        EXECUTION_PHASE_ENDED = 7,

        COMBAT_WON = 9, // IF EVENT NEEDS TO TRIGGER NO MATTER HOW COMBAT ENDS, USE COMBAT TERMINATED/RESOLVED INSTEAD
        COMBAT_LOST = 10, // IF EVENT NEEDS TO TRIGGER NO MATTER HOW COMBAT ENDS, USE COMBAT TERMINATED/RESOLVED INSTEAD
        COMBAT_TERMINATED = 11,
    }

    /// <summary>
    /// Handles events that will interupt the flow of combat
    /// </summary>
    public partial class CombatManagerBase : MonoBehaviour
    {
        [Space(10)]
        [Header("Combat Events")]
        private Dictionary<CombatEventTriggerType, CombatEventHandler> combatEventHandlers = new Dictionary<CombatEventTriggerType, CombatEventHandler>();
        [SerializeField] private CombatEventHandler combatEventHandlerPrefab;
        [SerializeField] private Transform eventHandlerParent;
        [Space(10)]

        //private bool isInterrupted = false;
        private Coroutine currentInterruptionCoroutine;
        public delegate IEnumerator CombatCoroutineEventHandler();

        private void SetupCombatEventHandlers()
        {
            foreach (CombatEventTriggerType eventType in Enum.GetValues(typeof(CombatEventTriggerType)))
                combatEventHandlers.Add(eventType, Instantiate(combatEventHandlerPrefab, eventHandlerParent));
        }

        public void SubscribeEventToCombatEventHandler(CombatEvent combatEvent, CombatEventTriggerType triggerType)
        {
           // Debug.Log("attempt to subscribe");
            if (combatEventHandlers.Keys.Contains(triggerType))
                combatEventHandlers[triggerType].Subscribe(combatEvent, combatEvent.priority);
            else Debug.LogWarning($"Trigger type {triggerType} not found, please ensure that trigger type is valid for combat event");
        }

        public void UnsubscribeEventFromCombatEventHandler(CombatEvent combatEvent, CombatEventTriggerType triggerType)
        {
            if (combatEventHandlers.Keys.Contains(triggerType))
                combatEventHandlers[triggerType].Unsubscribe(combatEvent);
            else Debug.LogWarning($"Trigger type {triggerType} not found, please ensure that trigger type is valid for combat event");
        }

        public IEnumerator WaitForCombatEvents(CombatEventTriggerType triggerType, Action callback)
        {
            yield return StartCoroutine(RunCombatEvents(triggerType));
            callback?.Invoke();
        }

        public IEnumerator RunCombatEvents(CombatEventTriggerType triggerType)
        {
            if (combatEventHandlers.Keys.Contains(triggerType))
                yield return StartCoroutine(combatEventHandlers[triggerType].Invoke());
            
        }

        protected virtual IEnumerator InvokeCombatEvent(CombatCoroutineEventHandler handler, bool yield = true)
        {
            if (handler != null)
            {
                if (yield) foreach (CombatCoroutineEventHandler invocation in handler?.GetInvocationList()) yield return StartCoroutine(invocation?.Invoke());
                else foreach (CombatCoroutineEventHandler invocation in handler?.GetInvocationList()) StartCoroutine(invocation?.Invoke());
            }
        }
    }
}