using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames
{
    [System.Serializable]
    public class GameConfigurationData
    {
        public List<string> triggeredEvents;
    }

    public static class GeneralGameData
    {
        public static GameConfigurationData gameData;
    }
}