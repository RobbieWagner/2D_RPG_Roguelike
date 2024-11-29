using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public class RandomEncounterEnemy : MonoBehaviour
    {
        public Collider trigger;
        public CombatConfiguration combatInfo;
        public Scene combatScene;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
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