using RobbieWagnerGames.Utilities;
using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class EventSequenceManager : MonoBehaviourSingleton<EventSequenceManager>
    {
        [HideInInspector] public EventSequence currentSequence = null;

        public IEnumerator RunEventSequence(EventSequence eventSequence)
        {
            if (currentSequence == null)
            {
                currentSequence = eventSequence;
                yield return StartCoroutine(eventSequence.InvokeEvent());
                currentSequence = null;
            }
        }
    }
}