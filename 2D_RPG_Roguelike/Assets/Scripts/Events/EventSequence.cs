using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class EventSequence : MonoBehaviour
    {
        public bool repeatable;
        [HideInInspector] public int timesTriggered;
        [Header("Events")]
        [SerializeField] protected List<SequenceEvent> eventSequence;

        public virtual IEnumerator InvokeEvent(bool setToEventGameMode = true)
        {
            timesTriggered++;

            if (setToEventGameMode && GameManager.Instance != null)
                GameManager.Instance.CurrentGameMode = GameMode.EVENT;

            foreach (SequenceEvent sequenceEvent in eventSequence)
                yield return StartCoroutine(sequenceEvent.InvokeSequenceEvent());

            if (setToEventGameMode && GameManager.Instance != null)
                GameManager.Instance.TriggerPreviousGameMode();

            OnCompleteEventInvocation?.Invoke();
        }

        public delegate void OnCompleteEventInvocationDelegate();
        public event OnCompleteEventInvocationDelegate OnCompleteEventInvocation;

        public bool CanTrigger()
        {
            return repeatable || timesTriggered == 0;
        }
    }
}