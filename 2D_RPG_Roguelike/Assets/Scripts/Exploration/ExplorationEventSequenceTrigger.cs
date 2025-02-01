using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ExplorationEventSequenceTrigger : MonoBehaviour
    {
        [SerializeField] private EventSequence eventSequence;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && eventSequence.CanTrigger())
                StartCoroutine(EventSequenceManager.Instance.RunEventSequence(eventSequence));
        }
    }
}