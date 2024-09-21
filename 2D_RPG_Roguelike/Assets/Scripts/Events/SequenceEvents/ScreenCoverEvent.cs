using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class ScreenCoverEvent : SequenceEvent
    {
        public override IEnumerator InvokeSequenceEvent()
        {
            yield return null;
            //yield return StartCoroutine(SceneTransitionController.Instance.FadeScreenOut());
        }
    }
}