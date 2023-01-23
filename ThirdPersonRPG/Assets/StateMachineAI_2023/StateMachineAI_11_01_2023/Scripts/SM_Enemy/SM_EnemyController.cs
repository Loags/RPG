using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Manager_Coroutine;

public enum EnemyContentState
{
    INITIALIZATION,
    IDLE,
    PATROL,
    CHASE,
    RESETPHASE,
    RETREATING,
    ATTACK,
    BLOCK
}

public enum EnemyFlowState
{
    NONE,
    PASSIVE,
    INCOMBAT
}

public class SM_EnemyController : MonoBehaviour
{
    [HideInInspector] public SM_EnemyMovement SM_EnemyMovement;
    [HideInInspector] public SM_EnemyCombat SM_EnemyCombat;
    [HideInInspector] public SM_EnemyTargeting SM_EnemyTargeting;
    [HideInInspector] public SM_EnemyHitBox SM_EnemyHitBox;
    [HideInInspector] public FieldOfView FieldOfView;

    [HideInInspector] public MonoBehaviour myMonoBehaviour;

    public NavMeshAgent NavMeshAgent;

    public delegate void EnemyContentStateChanged();
    public event EnemyContentStateChanged OnEnemyContentStateChanged;

    public delegate void EnemyFlowStateChanged();
    public event EnemyFlowStateChanged OnEnemyFlowStateChanged;

    [SerializeField] private EnemyContentState currentContentState;
    [SerializeField] private EnemyContentState previousContentState;

    [SerializeField] private EnemyFlowState currentFlowState;
    [SerializeField] private EnemyFlowState previousFlowState;

    private void Awake()
    {
        SM_EnemyMovement = GetComponent<SM_EnemyMovement>();
        SM_EnemyCombat = GetComponent<SM_EnemyCombat>();
        SM_EnemyTargeting = GetComponent<SM_EnemyTargeting>();
        SM_EnemyHitBox = GetComponentInChildren<SM_EnemyHitBox>();
        FieldOfView = GetComponent<FieldOfView>();
        myMonoBehaviour = GetComponent<MonoBehaviour>();
    }

    private void Update()
    {
        switch (currentContentState)
        {
            case EnemyContentState.IDLE:
                break;
            case EnemyContentState.PATROL:
                SM_EnemyMovement.Patrol();
                break;
            case EnemyContentState.CHASE:
                SM_EnemyMovement.ChaseEnemy();
                break;
            case EnemyContentState.RESETPHASE:
                break;
            case EnemyContentState.RETREATING:
                SM_EnemyTargeting.Retreating();
                break;
            case EnemyContentState.ATTACK:
                break;
            case EnemyContentState.BLOCK:
                break;
            default:
                break;
        }

        switch (currentFlowState)
        {
            case EnemyFlowState.NONE:
                break;
            case EnemyFlowState.PASSIVE:
                break;
            case EnemyFlowState.INCOMBAT:
                break;
            default:
                break;
        }

    }

    public void SetDestination(Vector3 _destination)
    {
        if (NavMeshAgent.destination == _destination) return;

        NavMeshAgent.destination = _destination;
    }

    public bool CheckDestinationValid(Vector3 _destination)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(_destination, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    public void InitializeCoroutine(ref Coroutine _coroutine, IEnumerator _enumerator, MonoBehaviour _behaviour = null)
    {
        _behaviour = myMonoBehaviour;
        CoroutineManager.InitializeCoroutine(ref _coroutine, _enumerator, _behaviour);
    }

    public void TerminateCoroutine(ref Coroutine _coroutine, IEnumerator _enumerator, MonoBehaviour _behaviour = null)
    {
        _behaviour = myMonoBehaviour;
        CoroutineManager.TerminateCoroutine(ref _coroutine, _enumerator, _behaviour);
    }

    public void ChangeContentState(EnemyContentState _newState)
    {
        if (_newState == currentContentState) return;

        previousContentState = currentContentState;
        currentContentState = _newState;

        OnEnemyContentStateChanged?.Invoke();
        //Debug.Log("[State Changed] \n" + previousContentState + "  --->  " + currentContentState);
    }

    public void ChangeFlowState(EnemyFlowState _newState)
    {
        if (_newState == currentFlowState) return;

        previousFlowState = currentFlowState;
        currentFlowState = _newState;

        OnEnemyFlowStateChanged?.Invoke();
        //Debug.Log("[State Changed] \n" + previousFlowState + "  --->  " + currentFlowState);
    }
    public EnemyContentState GetCurrentContentState() { return currentContentState; }
    public EnemyContentState GetPreviousContentState() { return previousContentState; }
    public EnemyFlowState GetCurrentFlowState() { return currentFlowState; }
    public EnemyFlowState GetPreviousFlowState() { return previousFlowState; }
}
