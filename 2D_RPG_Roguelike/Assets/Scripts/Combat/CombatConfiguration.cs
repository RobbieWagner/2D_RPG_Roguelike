using RobbieWagnerGames.StrategyCombat.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class CombatConfiguration : MonoBehaviour
    {
        [SerializeField] private List<Ally> allies;
        [SerializeField] private List<Enemy> enemies;

        public virtual List<Ally> GetAllies()
        {
            return allies;
        }

        public virtual List<Enemy> GetEnemies()
        {
            return enemies;
        }
    }
}