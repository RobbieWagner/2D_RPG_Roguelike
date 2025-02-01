using RobbieWagnerGames.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RobbieWagnerGames.TurnBasedCombat
{
    public enum MovementState
    {
        None = -1,
        Idle = 0,
        Moving = 1,
        Chasing = 2,
    }
    
    public class OverworldEnemy : MonoBehaviour
    {
        public Collider trigger;
        public static bool isCombatTriggered = false;
        public CombatConfiguration combatInfo;
        public Scene combatScene;

        public GameObject parentGameObject;

        [HideInInspector] public MovementState movementState = MovementState.Idle;
        private IEnumerator currentMovementStateCoroutine;

        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [SerializeField] private float noticeRange = 4;
        [SerializeField] private SphereCollider noticeTrigger;
        [SerializeField] private float noticeTime = .65f;
        [SerializeField] private Vector2 idleTimeRange;
        [SerializeField] private float walkRadius = 25f;
        [SerializeField] private Vector2 walkDistanceRange;

        #region combat
        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Player") && !isCombatTriggered && GameManager.Instance.CurrentGameMode == GameMode.EXPLORATION)
            {
                ChangeMovementState(MovementState.Idle);
                PlayerMovement.Instance.CeasePlayerMovement();
                StartCoroutine(GameManager.Instance.TriggerCombat(combatInfo));
                isCombatTriggered = true;

                StartCoroutine(DelayDestroy());
            }
        }
        #endregion

        #region ai navigation
        private void Awake()
        {
            //unitAnimator.ChangeAnimationState(UnitAnimationState.Idle);

            noticeTrigger.radius = noticeRange;
            currentMovementStateCoroutine = null;
            GameManager.OnGameModeChanged += CheckGameMode;
            CheckGameMode(GameManager.Instance.CurrentGameMode);
        }

        private void ResumeAgent()
        {
            movementState = MovementState.Idle;
            ChangeMovementCoroutine(Standby(Random.Range(idleTimeRange.x, idleTimeRange.y)));
            navMeshAgent.updateRotation = false;
        }

        private void CheckGameMode(GameMode mode)
        {
            if (mode == GameMode.EVENT)
                PauseAgent(false);
            else if (mode != GameMode.EXPLORATION)
                PauseAgent(true);
            else if (mode == GameMode.EXPLORATION)
                ResumeAgent();
        }

        private void PauseAgent(bool deleteifClose = false)
        {
            movementState = MovementState.Idle;
            if (currentMovementStateCoroutine != null)
                StopCoroutine(currentMovementStateCoroutine);
            navMeshAgent.SetDestination(transform.position);

            if (deleteifClose && Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) < noticeRange)
                OverworldEnemyManager.Instance.RemoveEnemy(this);
        }

        public void ChangeMovementState(MovementState state)
        {
            if (movementState != state)
            {
                switch (state)
                {
                    case MovementState.Idle:
                        //UnitAnimationState animationState = unitAnimator.GetAnimationState();
                        //if (animationState == UnitAnimationState.WalkForward)
                        //{
                        //    unitAnimator.ChangeAnimationState(UnitAnimationState.IdleForward);
                        //}
                        //else if (animationState == UnitAnimationState.WalkBack)
                        //{
                        //    unitAnimator.ChangeAnimationState(UnitAnimationState.Idle);
                        //}
                        //else if (animationState == UnitAnimationState.WalkLeft)
                        //{
                        //    unitAnimator.ChangeAnimationState(UnitAnimationState.IdleLeft);
                        //}
                        //else if (animationState == UnitAnimationState.WalkRight)
                        //{
                        //    unitAnimator.ChangeAnimationState(UnitAnimationState.IdleRight);
                        //}
                        ChangeMovementCoroutine(Standby(Random.Range(idleTimeRange.x, idleTimeRange.y)));
                        break;
                    case MovementState.Moving:
                        ChangeMovementCoroutine(MoveToNewSpot());
                        break;
                    case MovementState.Chasing:
                        ChangeMovementCoroutine(ChasePlayer());
                        break;
                }

                movementState = state;

                OnMovementStateChange?.Invoke(movementState);
            }
        }

        public delegate void OnMovementStateChangeDelegate(MovementState state);
        public event OnMovementStateChangeDelegate OnMovementStateChange;

        private void ChangeMovementCoroutine(IEnumerator newCoroutine)
        {
            if (newCoroutine != currentMovementStateCoroutine)
            {
                if (currentMovementStateCoroutine != null) 
                    StopCoroutine(currentMovementStateCoroutine);
                if (newCoroutine != null)
                {
                    currentMovementStateCoroutine = newCoroutine;
                    StartCoroutine(currentMovementStateCoroutine);
                }
            }
        }

        private IEnumerator Standby(float idleTime)
        {
            //Debug.Log("idle");
            if (navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }

            yield return new WaitForSeconds(idleTime);

            ChangeMovementState(MovementState.Moving);
        }

        private IEnumerator MoveToNewSpot()
        {
            //Debug.Log("moving");
            if (navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = false;
            }

            Vector3 targetPos = FindSpotOnNavMesh();

            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(targetPos, path);
            float walkDistance = CalculatePathDistance(path.corners);

            int limit = 0;
            while ((path.status == NavMeshPathStatus.PathPartial || walkDistance > walkDistanceRange.y || walkDistance < walkDistanceRange.x) && limit < 10)
            {
                targetPos = FindSpotOnNavMesh();
                navMeshAgent.CalculatePath(targetPos, path);
                walkDistance = CalculatePathDistance(path.corners);
                limit++;
            }

            if (limit == 10)
            {
                targetPos = transform.position;
            }

            navMeshAgent.SetDestination(targetPos);
            yield return new WaitForSeconds(1f);
            while (Vector3.Distance(navMeshAgent.destination, transform.position) > .75f && (Mathf.Abs(navMeshAgent.velocity.x) > .5f || Mathf.Abs(navMeshAgent.velocity.z) > .5f))
            {
                yield return null;
            }

            ChangeMovementState(MovementState.Idle);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ChangeMovementState(MovementState.Chasing);
            }
        }

        private IEnumerator ChasePlayer()
        {
            //Debug.Log("chasing");
            if (navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }
            yield return new WaitForSeconds(noticeTime);

            navMeshAgent.isStopped = false;
            while (NavMesh.SamplePosition(PlayerMovement.Instance.transform.position, out NavMeshHit hit, 1, NavMesh.AllAreas))
            {
                if (Vector3.Distance(hit.position, transform.position) > 10)
                    break;

                yield return null;
                navMeshAgent.SetDestination(hit.position);
            }
            ChangeMovementState(MovementState.Idle);
        }

        private Vector3 FindSpotOnNavMesh()
        {
            Vector3 position = transform.position;
            bool found = false;
            int limit = 0;
            while (!found && limit < 10)
            {
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                found = NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                if (found) position = hit.position;
                limit++;
            }

            return position;
        }

        private float CalculatePathDistance(Vector3[] points)
        {
            float distance = Vector3.Distance(transform.position, points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                distance += Vector3.Distance(points[i], points[i - 1]);
            }

            return distance;
        }
        #endregion

        #region destruction

        private IEnumerator DelayDestroy()
        {
            yield return new WaitUntil(() => GameManager.Instance.CurrentGameMode == GameMode.COMBAT);

            OverworldEnemyManager.Instance.RemoveEnemy(this);
        }

        private void OnDestroy() 
        {
            if (currentMovementStateCoroutine != null)
            {
                StopCoroutine(currentMovementStateCoroutine);
                currentMovementStateCoroutine = null;
            }
            GameManager.OnGameModeChanged -= CheckGameMode;
        }

        #endregion
    }
}