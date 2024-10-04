using RobbieWagnerGames.UI;
using RobbieWagnerGames.StrategyCombat;
using RobbieWagnerGames.StrategyCombat.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public partial class CombatManagerBase
    {
        protected virtual IEnumerator HandleWinState()
        {
            Debug.Log("Handle Win State");
            yield return new WaitForSeconds(2f);

            StartCoroutine(EndCombat());
        }

        protected virtual IEnumerator HandleLoseState()
        {
            Debug.Log("Handle Lose State");
            yield return new WaitForSeconds(2f);

            StartCoroutine(EndCombat());
        }

        public virtual IEnumerator EndCombat()
        {
            yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn());

            foreach (Ally ally in allyInstances)
                Destroy(ally.gameObject);
            allyInstances.Clear();

            foreach (Enemy enemy in enemyInstances)
                Destroy(enemy.gameObject);
            enemyInstances.Clear();

            CurrentCombatPhase = CombatPhase.NONE;
            OnCombatPhaseChange -= StartCombatPhase;
            currentCombat = null;
        }

        protected virtual bool CheckForCombatCompletion()
        {
            foreach (Unit unit in GetAllCombatUnits())
            {
                if (unit.HP == 0)
                    unit.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
                else
                    unit.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }

            if (!activeAllies.Any())
            {
                //Debug.Log("combat lost, changing phase to LOSE");
                CurrentCombatPhase = CombatPhase.LOSE;
                return true;
            }
            if (!activeEnemies.Any())
            {
                //Debug.Log("combat won, changing phase to WIN");
                CurrentCombatPhase = CombatPhase.WIN;
                return true;
            }

            return false;
        }
    }
}
