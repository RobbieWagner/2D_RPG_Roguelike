using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.AI
{
    public class AITarget : MonoBehaviour
    {
        public virtual void OnCaught(AIAgent agent)
        {
            Debug.Log($"{agent.gameObject.name} caught {gameObject.name}");
        }
    }
}