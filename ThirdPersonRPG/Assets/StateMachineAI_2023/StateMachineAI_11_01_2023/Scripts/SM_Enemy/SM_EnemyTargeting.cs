using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_EnemyTargeting : MonoBehaviour
{
    private SM_EnemyController SM_EnemyController;

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
        SM_EnemyController = GetComponent<SM_EnemyController>();
    }
    private void OnEnable()
    {
        SM_EnemyController.OnEnemyContentStateChanged += FaceTarget;
    }

    private void OnDisable()
    {
        SM_EnemyController.OnEnemyContentStateChanged -= FaceTarget;
    }

    public void AddTarget(GameObject _target)
    {
        if (targets.Contains(_target)) return;

        targets.Add(_target);

        currentTarget = targets[0];

        SM_EnemyController.InitializeCoroutine(ref targetInFollowingRangeCoroutine, CheckTargetInFollowingRange());

        SM_EnemyController.ChangeContentState(EnemyContentState.CHASE);
        SM_EnemyController.ChangeFlowState(EnemyFlowState.INCOMBAT);
    }

    public void RemoveTarget(GameObject _target)
    {
        if (!targets.Contains(_target)) return;

        targets.Remove(_target);

        if (targets.Count > 0)
            currentTarget = targets[0];
        else
        {
            SM_EnemyController.TerminateCoroutine(ref targetInFollowingRangeCoroutine, CheckTargetInFollowingRange());
            currentTarget = null;
            SM_EnemyController.ChangeFlowState(EnemyFlowState.PASSIVE);
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
                    SM_EnemyController.InitializeCoroutine(ref resetPhaseCoroutine, ResetPhase());
                }
            }
        }
    }

    private IEnumerator ResetPhase()
    {
        SM_EnemyController.FieldOfView.SetDetection(false);
        SM_EnemyController.ChangeContentState(EnemyContentState.RESETPHASE);
        SM_EnemyController.SetDestination(this.transform.position); // Prevents ai from moving cause destination target is his own position

        yield return new WaitForSeconds(resetPhaseDuration);
        RemoveTarget(currentTarget);
        SM_EnemyController.ChangeContentState(EnemyContentState.RETREATING); // Start the retreating phase - running back to origin
    }

    public void Retreating()
    {
        SM_EnemyMovement SM_EnemyMovement = SM_EnemyController.SM_EnemyMovement;

        if (SM_EnemyController.GetCurrentContentState() != EnemyContentState.RETREATING) return;

        SM_EnemyController.SetDestination(SM_EnemyMovement.spawnPos); // Make the AI return to its origin

        if (Vector3.Distance(this.transform.position, SM_EnemyMovement.spawnPos) > SM_EnemyController.NavMeshAgent.stoppingDistance * 1.2) return; // Have a check if the AI reached its origin 
                                                                                                                                                   // Distance check AI.StoppingDistance * 1.2 ensures the AI can reach its origin
        SM_EnemyController.ChangeContentState(EnemyContentState.PATROL);
        SM_EnemyController.FieldOfView.SetDetection(true); // Make the AI spot enemies after it has reached its origin
        SM_EnemyController.TerminateCoroutine(ref resetPhaseCoroutine, ResetPhase());
    }

    private void FaceTarget()
    {
        EnemyContentState currentState = SM_EnemyController.GetCurrentContentState();
        if (currentState == EnemyContentState.ATTACK || currentState == EnemyContentState.BLOCK) // only face the enemy when it is in range
            SM_EnemyController.InitializeCoroutine(ref faceTargetCoroutine, FaceTargetScan());

        if (currentState != EnemyContentState.ATTACK && currentState != EnemyContentState.BLOCK)
            SM_EnemyController.TerminateCoroutine(ref faceTargetCoroutine, FaceTargetScan());
    }

    private IEnumerator FaceTargetScan()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            Quaternion desiredRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, SM_EnemyController.NavMeshAgent.angularSpeed * Time.deltaTime);
        }
    }

}
