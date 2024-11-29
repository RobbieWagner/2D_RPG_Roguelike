using RobbieWagnerGames.StrategyCombat.Units;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    [Serializable]
    public class CombatConfiguration
    {
        public bool pullAlliesFromSave;
        public List<Ally> allyPrefabs;
        public List<Enemy> enemyPrefabs;
        public string combatSceneRef;
    }
}