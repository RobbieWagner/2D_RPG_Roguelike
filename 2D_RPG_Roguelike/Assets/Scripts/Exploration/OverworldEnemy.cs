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
        Noticing = 2,
        Chasing = 3,
    }
    
    public class OverworldEnemy : MonoBehaviour
    {
        public Collider trigger;
        public static bool isCombatTriggered = false;
        public CombatConfiguration combatInfo;
        public Scene combatScene;

        private bool doUpdate = true;

        public GameObject parentGameObject;

        [HideInInspector] public MovementState movementState = MovementState.None;
        private IEnumerator currentMovementStateCoroutine;

        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [SerializeField] private float noticeRange = 4;
        [SerializeField] private SphereCollider noticeTrigger;
        [SerializeField] private float noticeTime = .65f;
        [SerializeField] private Vector2 idleTimeRange;
        private float idleTime = 0;
        private float currentIdleTimer = 0;
        [SerializeField] private float walkRadius = 25f;
        [SerializeField] private float maxWalkTime = 10f;
        private float currentWalkTimer = 0;
        [SerializeField] private Vector2 walkDistanceRange;

        #region collision
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ChangeMovementState(MovementState.Noticing);
            }
        }

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

        #region agent
        private void Awake()
        {
            //unitAnimator.ChangeAnimationState(UnitAnimationState.Idle);

            noticeTrigger.radius = noticeRange;
            currentMovementStateCoroutine = null;
            GameManager.OnGameModeChanged += CheckGameMode;
            CheckGameMode(GameManager.Instance.CurrentGameMode);
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

        public void PauseAgent(bool deleteifClose = false)
        {
            navMeshAgent.isStopped = true;
            doUpdate = false;

            if (deleteifClose && Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) < noticeRange)
                OverworldEnemyManager.Instance.RemoveEnemy(this);
        }

        public void ResumeAgent()
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.updateRotation = false;
            doUpdate = true;

            if(movementState == MovementState.None)
                ChangeMovementState(MovementState.Idle);
        }
        #endregion

        #region movement states
        public void ChangeMovementState(MovementState state)
        {
            if (movementState != state)
            {
                movementState = state;
                OnMovementStateChange?.Invoke(movementState);
            }

            switch(movementState)
            {
                case MovementState.Idle:
                    StandIdle();
                    break;
                case MovementState.Moving:
                    StartMoving();
                    break;
                case MovementState.Noticing:
                    StartCoroutine(NoticePlayer());
                    break;
                case MovementState.Chasing:
                    StartChase();
                    break;
                case MovementState.None:
                    break;
                default:
                    break;
            }
                
        }

        public delegate void OnMovementStateChangeDelegate(MovementState state);
        public event OnMovementStateChangeDelegate OnMovementStateChange;

        private void StandIdle()
        {
            idleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
            currentIdleTimer = 0;
            navMeshAgent.isStopped = true;
        }

        private void StartMoving()
        {
            if (navMeshAgent.enabled)
                navMeshAgent.isStopped = false;

            currentWalkTimer = 0;

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
                ChangeMovementState(MovementState.Idle);

            navMeshAgent.SetDestination(targetPos);
        }

        private IEnumerator NoticePlayer()
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }

            yield return new WaitForSeconds(noticeTime);
            ChangeMovementState(MovementState.Chasing);
        }

        private void StartChase()
        {
            navMeshAgent.isStopped = false;
        }
        #endregion

        #region ai navigation
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

        #region movement state updates
        private void Update()
        {
            if (doUpdate)
            { 
                switch (movementState)
                {
                    case MovementState.Idle:
                        UpdateStandby();
                        break;
                    case MovementState.Moving:
                        UpdateMovement();
                        break;
                    case MovementState.Chasing:
                        UpdateChase();
                        break;
                    default: break;
                }
            }
        }

        private void UpdateStandby()
        {
            currentIdleTimer += Time.deltaTime;

            if(currentIdleTimer >= idleTime)
                ChangeMovementState(MovementState.Moving);
        }

        private void UpdateMovement()
        {
            currentWalkTimer += Time.deltaTime;

            if (Vector3.Distance(navMeshAgent.destination, transform.position) < .75f || currentWalkTimer >= maxWalkTime) //&& (Mathf.Abs(navMeshAgent.velocity.x) > .5f || Mathf.Abs(navMeshAgent.velocity.z) > .5f))
            {
                ChangeMovementState(MovementState.Idle);
            }
        }

        private void UpdateChase()
        {
            if(NavMesh.SamplePosition(PlayerMovement.Instance.transform.position, out NavMeshHit hit, 1, NavMesh.AllAreas) && Vector3.Distance(hit.position, transform.position) < 10)
                navMeshAgent.SetDestination(hit.position);
            else
                ChangeMovementState(MovementState.Idle);
        }

        private void UpdateAnimation()
        {
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