using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat 
{
    [System.Serializable]
    public class ExplorationConfiguration
    {
        public string explorationSceneRef;
        public float playerPositionX;
        public float playerPositionY;
        public float playerPositionZ;
    }

    public static class ExplorationData
    {
        public static ExplorationConfiguration explorationConfiguration;
    }
}