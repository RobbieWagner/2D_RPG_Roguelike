using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class RandomEncounterEnemy : MonoBehaviour
    {
        public Collider trigger;
        public CombatConfiguration combatInfo;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CombatManagerBase.Instance.transform.position = new Vector3(transform.position.x, CombatManagerBase.Instance.transform.position.y, transform.position.z);
                PlayerMovement.Instance.CeasePlayerMovement();
                StartCoroutine(CombatManagerBase.Instance.StartCombat(combatInfo));
                trigger.enabled = false;
                CombatManagerBase.OnCombatPhaseChange += CheckFoOverworldDestruction;
            }
        }

        private void CheckFoOverworldDestruction(CombatPhase phase)
        {
            if (phase == CombatPhase.SETUP)
            {
                CombatManagerBase.OnCombatPhaseChange -= CheckFoOverworldDestruction;
                Destroy(gameObject);
            }
        }
    }
}