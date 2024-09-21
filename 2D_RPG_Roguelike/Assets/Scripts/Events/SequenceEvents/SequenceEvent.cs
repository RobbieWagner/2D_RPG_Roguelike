using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class SequenceEvent : MonoBehaviour
    {
        public virtual IEnumerator InvokeSequenceEvent()
        {
            yield return null;
        }
    }
}