using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

namespace RobbieWagnerGames.AI
{
    public enum AIState
    {
        NONE = -1,
        IDLE = 0,
        MOVING = 1,
        CHASING = 2
    }

    // DEFINES THE BASE AI AGENT AND HELPFUL METHODS
    // USEFUL FOR EASY, SIMPLE USES OF AI PATHFINDING, POTENTIALLY IF YOU HAVE A LOT OF AGENTS TO CONTROL
    // YOU CAN ALSO CREATE A CHILD CLASS OF THIS IF YOU'D LIKE TO CREATE ALTERNATE BEHAVIORS
    public class AIAgent : MonoBehaviour
    {
        public NavMeshAgent agent;

        [SerializeField] protected float idleWaitTime = 3f;
        protected float currentWaitTime;

        [SerializeField] protected float movementRange = 100f;

        protected List<AITarget> currentTargets = new List<AITarget>(); // Use [HideInInspector] if needed
        public AITarget chasingTarget { get; protected set; }

        protected AIState currentState = AIState.NONE;
        public AIState CurrentState
        {
            get 
            {
                return currentState;
            }
            set 
            {
                if(value == currentState)
                    return;
                currentState = value;
            }
        }
        //TODO: Add a delegate and event? Does Observer pattern need to be implemented?

        #region State Changing
        public virtual void GoIdle()
        {
            agent.isStopped = true;
            CurrentState = AIState.IDLE;
        }

        public virtual bool MoveAgent(Vector3 destination)
        {
            CurrentState = AIState.MOVING;
            bool success = SetDestination(destination);

            if (!success)
            {
                GoIdle();
                Debug.LogWarning("failed to move agent");
            }

            return success;
        }

        public bool ChaseNearestTarget()
        {
            Debug.Log("Chase Nearest Target.");
            AITarget closestTarget = null;
            float closestDistance = float.MaxValue;

            if (currentTargets == null || !currentTargets.Any())
            {
                Debug.LogWarning("Could not chase target: current targets list found empty.");
                GoIdle();
                return false;
            }

            foreach (AITarget target in currentTargets)
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(target.transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    float pathLength = AIManager.GetPathLength(path);

                    if (pathLength < closestDistance)
                    {
                        closestDistance = pathLength;
                        closestTarget = target;
                    }
                }
            }

            if (closestTarget != null)
            {
                CurrentState = AIState.CHASING;
                chasingTarget = closestTarget;
                return true;
            }

            return false;
        }
        #endregion

        #region States and Updates

        protected void Update()
        {
            switch (currentState) 
            {
                case AIState.IDLE:
                    UpdateIdleState();
                    break;
                case AIState.MOVING:
                    UpdateMovingState();
                    break;
                case AIState.CHASING:
                    UpdateChaseState();
                    break;
                default:
                    break;
            }
        }

        protected virtual void UpdateIdleState()
        {
            currentWaitTime += Time.deltaTime;
            if(currentWaitTime >= idleWaitTime)
            {
                currentWaitTime = 0;
                MoveToRandomSpot(movementRange);
            }
        }

        protected virtual void UpdateMovingState()
        {
            if (agent.destination == null || AIManager.GetPathLength(agent.path) < .05f)
                GoIdle();
        }

        protected virtual void UpdateChaseState()
        {
            SetDestination(chasingTarget.transform.position);

            if (agent.destination == null || chasingTarget == null ||  AIManager.GetPathLength(agent.path) < .05f)
                OnReachTarget(chasingTarget);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if(CurrentState == AIState.CHASING)
            {
                AITarget target = collision.gameObject.GetComponent<AITarget>();

                if(target != null && chasingTarget == target)
                    OnReachTarget(chasingTarget);
            }
        }

        protected virtual void OnReachTarget(AITarget target)
        {
            target.OnCaught(this);
            currentTargets.Remove(chasingTarget);
            ChaseNearestTarget();
        }
        #endregion

        #region Worldspace Movement

        public virtual bool SetDestination(Vector3 destination)
        {
            agent.isStopped = false;
            return agent.SetDestination(destination);
        }

        public virtual void MoveToRandomSpot(float range = 100f)
        {
            StartCoroutine(MoveToRandomSpotCo(transform.position, range, 10000));
        }

        public virtual IEnumerator MoveToRandomSpotCo(Vector3 offset, float range = 100f, int tryLimit = 10000, int triesBeforeYield = 25)
        { 
            int tries = 0;
            bool success = false;
            while (tries < tryLimit)
            {
                tries++;
                if(tries % triesBeforeYield == 0)
                    yield return null;
                
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range;
                randomDirection += offset;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
                {
                    MoveAgent(hit.position);
                    success = true;
                    yield break;
                }
            }
            
            if(!success)
                Debug.LogWarning($"Could not find a path after trying {tryLimit} times!");
        }
        #endregion

        #region AITarget Chasing
        public virtual void SetTargets(List<AITarget> targets, bool removeOldTargets = false, bool chaseNearestTarget = true)
        {
            if(removeOldTargets)
                currentTargets.Clear();
            
            currentTargets.AddRange(targets);

            if (chaseNearestTarget)
                ChaseNearestTarget();
        }
        #endregion
    }
}