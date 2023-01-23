using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargeting : MonoBehaviour
{
    private EnemyController EnemyController;

    [Space(15)]
    [Header("Targeting")]
    public GameObject currentTarget;
    [HideInInspector] public List<GameObject> targets = new();

    // In Range Check - Distance based
    [SerializeField] private float followingRange;
    [SerializeField] private float checkFollowingRangeIteration;
    [SerializeField] private bool targetInFollowingRange;
    private Coroutine targetInFollowingRangeCoroutine;
    private Coroutine faceTargetCoroutine;

    // In Sight Check - FOV based
    private Coroutine targetOutOfSightCoroutine;

    [Space(15)]
    [Header("Reset Phase")]
    [SerializeField] private float resetPhaseDuration;
    private Coroutine resetPhaseCoroutine;

    private void Awake()
    {
        EnemyController = GetComponent<EnemyController>();
    }
    private void OnEnable()
    {
        EnemyController.OnEnemyContentStateChanged += FaceTarget;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyContentStateChanged -= FaceTarget;
    }

    public void AddTarget(GameObject _target)
    {
        if (targets.Contains(_target)) return;

        targets.Add(_target);

        currentTarget = targets[0];

        EnemyController.InitializeCoroutine(ref targetInFollowingRangeCoroutine, CheckTargetInFollowingRange());

        EnemyController.ChangeContentState(EnemyContentState.CHASE);
        EnemyController.ChangeFlowState(EnemyFlowState.INCOMBAT);
    }

    public void RemoveTarget(GameObject _target)
    {
        if (!targets.Contains(_target)) return;

        targets.Remove(_target);

        if (targets.Count > 0)
            currentTarget = targets[0];
        else
        {
            EnemyController.TerminateCoroutine(ref targetInFollowingRangeCoroutine, CheckTargetInFollowingRange());
            currentTarget = null;
            EnemyController.ChangeFlowState(EnemyFlowState.PASSIVE);
        }
    }

    private IEnumerator CheckTargetInFollowingRange()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkFollowingRangeIteration);
            float distance = float.MaxValue;
            if (currentTarget != null) // Null Ref if currentTarget is already deleted
                distance = Vector3.Distance(this.transform.position, currentTarget.transform.position);
            if (distance < followingRange)
            {
                if (!targetInFollowingRange)
                    targetInFollowingRange = true;
            }
            else
            {
                if (targetInFollowingRange)
                {
                    targetInFollowingRange = false;
                    EnemyController.InitializeCoroutine(ref resetPhaseCoroutine, ResetPhase());
                }
            }
        }
    }

    private IEnumerator ResetPhase()
    {
        EnemyController.FieldOfView.SetDetection(false);
        EnemyController.ChangeContentState(EnemyContentState.RESETPHASE);
        EnemyController.SetDestination(this.transform.position); // Prevents ai from moving cause destination target is his own position

        yield return new WaitForSeconds(resetPhaseDuration);
        RemoveTarget(currentTarget);
        EnemyController.ChangeContentState(EnemyContentState.RETREATING); // Start the retreating phase - running back to origin
    }

    public void Retreating()
    {
        EnemyMovement SM_EnemyMovement = EnemyController.SM_EnemyMovement;

        if (EnemyController.GetCurrentContentState() != EnemyContentState.RETREATING) return;

        EnemyController.SetDestination(SM_EnemyMovement.spawnPos); // Make the AI return to its origin

        if (Vector3.Distance(this.transform.position, SM_EnemyMovement.spawnPos) > EnemyController.NavMeshAgent.stoppingDistance * 1.2) return; // Have a check if the AI reached its origin 
                                                                                                                                                   // Distance check AI.StoppingDistance * 1.2 ensures the AI can reach its origin
        EnemyController.ChangeContentState(EnemyContentState.PATROL);
        EnemyController.FieldOfView.SetDetection(true); // Make the AI spot enemies after it has reached its origin
        EnemyController.TerminateCoroutine(ref resetPhaseCoroutine, ResetPhase());
    }

    private void FaceTarget()
    {
        EnemyContentState currentState = EnemyController.GetCurrentContentState();
        if (currentState == EnemyContentState.ATTACK || currentState == EnemyContentState.BLOCK) // only face the enemy when it is in range
            EnemyController.InitializeCoroutine(ref faceTargetCoroutine, FaceTargetScan());

        if (currentState != EnemyContentState.ATTACK && currentState != EnemyContentState.BLOCK)
            EnemyController.TerminateCoroutine(ref faceTargetCoroutine, FaceTargetScan());
    }

    private IEnumerator FaceTargetScan()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            Quaternion desiredRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, EnemyController.NavMeshAgent.angularSpeed * Time.deltaTime);
        }
    }

}
