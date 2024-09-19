using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RobbieWagnerGames.AI
{
    public class AITest : MonoBehaviour
    {
        [SerializeField] private AIAgent testAgentPrefab;
        [SerializeField] private Vector3 spawnPos;

        [SerializeField] private List<AITarget> agentTargets;

        private void Awake()
        {
            StartCoroutine(RunTestCo());
        }

        private IEnumerator RunTestCo()
        {
            yield return null;

            AIAgent agent = AIManager.Instance.AddAgentToScene(testAgentPrefab, Vector3.zero, agentTargets);

            agent.MoveAgent(new Vector3(3, 1, 3));

            yield return new WaitForSeconds(30);

            agent.SetTargets(agentTargets, false, true);
        }
    }
}