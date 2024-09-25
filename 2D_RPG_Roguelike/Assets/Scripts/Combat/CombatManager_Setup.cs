using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    /// <summary>
    /// Handles SETUP phase of combat
    /// </summary>
    public partial class CombatManagerBase : MonoBehaviour
    {
        [Space(10)]
        [Header("Combat Setup")]
        public List<Vector2> allyPositionOffsets;
        public List<Vector2> enemyPositionOffsets;

        // NOTE: ASSUMES ALLIES NEED TO BE INSTANTIATED
        protected virtual IEnumerator SetupCombat()
        {
            //Debug.Log("Setting up Combat");
            yield return null;

            if (currentCombat.pullAlliesFromSave)
                InstantiateAllies(PullAlliesFromSave(), allyPositionOffsets);
            else
                InstantiateAllies(currentCombat.allyPrefabs, allyPositionOffsets);

            InstantiateEnemies(currentCombat.enemyPrefabs, enemyPositionOffsets);

            CurrentCombatPhase = CombatPhase.ACTION_SELECTION;
        }

        private List<Ally> PullAlliesFromSave()
        {
            throw new NotImplementedException();
        }

        private void InstantiateAllies(List<Ally> allyPrefabs, List<Vector2> allyPositionOffsets)
        {
            for(int i = 0; i < allyPrefabs.Count; i++)
            {

            }
        }

        private void InstantiateEnemies(List<Enemy> enemyPrefabs, List<Vector2> enemyPositionOffsets)
        {
            throw new NotImplementedException();
        }
    }
}