using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SM_EnemyMovement : MonoBehaviour
{
    private SM_EnemyController SM_EnemyController;

    public float walkSpeed;
    public float sprintSpeed;

    [Header("Patrol Points")]
    [Tooltip("Set this bool to true if you want to use randomly generated patrol points")]
    [SerializeField] private bool useRandomPatrolPoints;
    [Tooltip("Only use this float when you want to generate patrol points")]
    [SerializeField] private float radiusPatrolPoints;
    [Tooltip("Set the min distance that should be between each patrol point")]
    [SerializeField] private float distanceBetweenPoints;
    [Tooltip("How long the AI should wait at a patrol point")]
    [SerializeField] private float timeAtPatrolPoint;
    [Tooltip("How many patrolpoints the ai should generate")]
    [SerializeField] private int amountPatrolPoints;
    [Tooltip("Set your patrolpoints if want to have fixed positions")]
    [SerializeField] private List<Vector3> patrolPoints = new();

    private Vector3 previousTargetPoint = new(-1f, -1f, -1f);
    private Vector3 targetPoint = new(0f, 0f, 0f);
    [HideInInspector] public Vector3 spawnPos;
    private Coroutine idleAtPatrolPoint;
    private Coroutine setPatrolPointCoroutine;

    private void Awake() { SM_EnemyController = GetComponent<SM_EnemyController>(); }

    private void OnEnable()
    {
        SM_EnemyController.OnEnemyContentStateChanged += AdjustMovementSpeed;
        SM_EnemyController.OnEnemyContentStateChanged += ResetIdleCoroutine;
    }

    private void OnDisable()
    {
        SM_EnemyController.OnEnemyContentStateChanged -= AdjustMovementSpeed;
        SM_EnemyController.OnEnemyContentStateChanged -= ResetIdleCoroutine;
    }

    private void Start()
    {
        spawnPos = this.gameObject.transform.position;
        if (useRandomPatrolPoints)
        {
            for (int i = 0; i < amountPatrolPoints; i++)
                patrolPoints.Add(Vector3.zero);
            SM_EnemyController.InitializeCoroutine(ref setPatrolPointCoroutine, SetPatrolPoints());
        }
        SM_EnemyController.InitializeCoroutine(ref idleAtPatrolPoint, IdleAtPatrolPoint());
    }

    public void ChaseEnemy()
    {
        Vector3 enemyPos = SM_EnemyController.SM_EnemyTargeting.currentTarget.transform.position;
        SM_EnemyController.SetDestination(enemyPos);

        if (Vector3.Distance(this.transform.position, enemyPos) > SM_EnemyController.NavMeshAgent.stoppingDistance) return;

        if (SM_EnemyController.GetCurrentContentState() != EnemyContentState.ATTACK)
            SM_EnemyController.ChangeContentState(EnemyContentState.ATTACK);

    }

    public void Patrol()
    {
        SM_EnemyController.SetDestination(targetPoint);
        float distanceToTargetPoint = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(targetPoint.x, targetPoint.z));
        if (distanceToTargetPoint <= SM_EnemyController.NavMeshAgent.stoppingDistance)
        {
            if (idleAtPatrolPoint != null) return; // return when the Coroutine has already started

            SM_EnemyController.InitializeCoroutine(ref idleAtPatrolPoint, IdleAtPatrolPoint());
        }
    }

    private IEnumerator IdleAtPatrolPoint()
    {
        SM_EnemyController.ChangeContentState(EnemyContentState.IDLE);
        yield return new WaitForSeconds(timeAtPatrolPoint);
        ChooseTargetPoint();
        idleAtPatrolPoint = null;
    }

    private IEnumerator SetPatrolPoints()
    {
        int length = patrolPoints.Count;
        for (int i = 0; i < length; i++)
        {
            Vector3 randomPos = GetRandomPoint(i);

            tempPoint = Vector3.zero; // Workaround

            yield return new WaitForEndOfFrame();
            patrolPoints[i] = randomPos;

            Debug.DrawRay(randomPos, Vector3.up * 10, Color.blue, float.MaxValue);
        }

        CleanUpPatrolPoints();
        SM_EnemyController.TerminateCoroutine(ref setPatrolPointCoroutine, SetPatrolPoints());
    }

    // Workaround
    private void CleanUpPatrolPoints()
    {
        bool removed = false;
        for (int i = patrolPoints.Count - 1; i >= 0; i--)
            if (patrolPoints[i] == Vector3.zero)
            {
                patrolPoints.Remove(patrolPoints[i]);
                removed = true;
            }

        if (removed)
            print("* Removed Patrol Points due to too many iterations * \n" +
                    "---> Solution: Lower distanceBetweenPoints or amountPatrolPoints");
    }
    private Vector3 tempPoint = new();
    private int fallBackCounter = 0;
    public Vector3 GetRandomPoint(int _index)
    {
        fallBackCounter += 1;
        if (fallBackCounter > 30) // Prevents the game from crashing --> StackOverflow
        {
            fallBackCounter = 0;
            return Vector3.zero;
        }

        NavMeshHit hit = new();

        while (true) // Search until it has found a point on the nav mesh
        {
            if (NavMesh.SamplePosition(spawnPos + Random.insideUnitSphere * radiusPatrolPoints, out hit, 1.0f, NavMesh.AllAreas))
                break;
        }

        if (!IsPatrolPointValid(hit.position, _index)) // Search until it has found a point on the nav mesh and with its distance to the nearest point
            GetRandomPoint(_index);
        else
            tempPoint = hit.position;

        return tempPoint;
    }

    private bool IsPatrolPointValid(Vector3 _position, int _index)
    {
        if (!SM_EnemyController.CheckDestinationValid(_position)) return false;

        bool isValid = true;
        foreach (Vector3 vector3 in patrolPoints) // Check if the points have their min distance
                                                  // ( CAREFULL: can take long or forever, when it's used on a small space with many patrolpoints ) <---------
        {
            float dist = Vector3.Distance(_position, vector3);
            if (dist < distanceBetweenPoints)
            {
                isValid = false;
            }
        }

        return isValid;
    }

    private void ChooseTargetPoint()
    {
        int randomIndex = Random.Range(0, patrolPoints.Count - 1);
        if (previousTargetPoint == patrolPoints[randomIndex])
            ChooseTargetPoint(); // Restart func until u have a different target point

        SM_EnemyController.ChangeContentState(EnemyContentState.PATROL);
        previousTargetPoint = targetPoint;
        targetPoint = patrolPoints[randomIndex];
    }

    private Vector3 SetPatrolPointYAxis(Vector3 _position)
    {
        Vector3 newPos = _position;

        if (Physics.Raycast(new Ray(_position, Vector3.up), out RaycastHit hitUp, Mathf.Infinity, LayerMask.GetMask("Ground"))) // Get exact y-position on ground where the PatrolPoint should spawn
        {
            if (hitUp.collider != null)
            {
                newPos.y = hitUp.point.y;
                return newPos;
            }
        }

        return new Vector3(-100f, -100f, -100f);
    }

    private void ResetIdleCoroutine()
    {
        if (SM_EnemyController.GetPreviousContentState() == EnemyContentState.IDLE && SM_EnemyController.GetCurrentContentState() != EnemyContentState.IDLE)
            SM_EnemyController.TerminateCoroutine(ref idleAtPatrolPoint, IdleAtPatrolPoint());
    }

    #region EffectPublicVars
    private void AdjustMovementSpeed()
    {
        switch (SM_EnemyController.GetCurrentContentState())
        {
            case EnemyContentState.IDLE:
                SetMovementSpeed(walkSpeed);
                break;
            case EnemyContentState.PATROL:
                SetMovementSpeed(walkSpeed);
                break;
            case EnemyContentState.CHASE:
                SetMovementSpeed(sprintSpeed);
                break;
            default:
                break;
        }
    }
    public void SetMovementSpeed(float _value)
    {
        walkSpeed = _value;
        SM_EnemyController.NavMeshAgent.speed = walkSpeed;
    }
    #endregion EffectPublicVars

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusPatrolPoints);
    }
}

