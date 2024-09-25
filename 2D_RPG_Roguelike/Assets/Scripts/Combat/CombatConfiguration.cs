using RobbieWagnerGames.StrategyCombat.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatConfiguration : MonoBehaviour
    {
        public bool pullAlliesFromSave;
        public List<Ally> allyPrefabs;
        public List<Enemy> enemyPrefabs;
    }
}