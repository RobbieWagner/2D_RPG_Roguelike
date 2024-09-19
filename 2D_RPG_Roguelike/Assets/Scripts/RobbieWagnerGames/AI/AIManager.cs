using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.AI
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance { get; private set; }

        private List<AIAgent> activeAgents = new List<AIAgent>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void FreezeAI()
        {
            if(activeAgents.Any())
            {
                foreach(AIAgent agent in activeAgents) 
                    agent?.GoIdle();
            }
        }

        public AIAgent AddAgentToScene(AIAgent agentPrefab, Vector3 startingPos, List<AITarget> initialTargets)
        {
            AIAgent agent = Instantiate(agentPrefab);
            agent.transform.position = startingPos;
            agent.SetTargets(initialTargets);
            activeAgents.Add(agent);
            return agent;
        }

        public void DestroyAgent(AIAgent agent)
        {
            activeAgents.Remove(agent);
            Destroy(agent);
        }

        public static float GetPathLength(NavMeshPath path)
        {
            float length = 0.0f;

            if (path.corners.Length < 2)
                return length;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return length;
        }
    }
}