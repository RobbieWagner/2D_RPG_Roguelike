using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class DebugLogSequenceEvent : CombatEvent
    {
        public string message;

        public override IEnumerator InvokeCombatEvent()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log(message);
        }
    }
}