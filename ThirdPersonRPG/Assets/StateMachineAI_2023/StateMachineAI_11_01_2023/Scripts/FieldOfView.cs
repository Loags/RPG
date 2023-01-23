using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private SM_EnemyController SM_EnemyController;

    [Header("FOV Settings")]
    public float fieldOfViewDegrees; // Has to be public for editor script
    public float fieldOfViewDegreesSmallArea; // Has to be public for editor script
    public float radius; // Has to be public for editor script
    public float radiusSmallArea; // Has to be public for editor script
    public LayerMask enemyMask;
    public LayerMask alliesMask;

    [HideInInspector] public bool canSeeEnemy;
    [HideInInspector] public Transform targetTransform;

    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float scanIterationTime;

    [Header("Enemy Detection Settings")]
    [SerializeField] private bool canDetectEnemies;

    private void Awake()
    {
        SM_EnemyController = GetComponent<SM_EnemyController>();
    }

    private void Start()
    {
        StartCoroutine(ScanForEnemies());
    }
    private void FieldOfViewScan(float _radius, float _fieldOfViewDegrees)
    {
        if (!canDetectEnemies) return; // If the AI is not allowed to detect enemies return

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, _radius, enemyMask);
        if (rangeChecks.Length != 0)
        {
            targetTransform = rangeChecks[0].transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < _fieldOfViewDegrees / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    // can see enemy
                    canSeeEnemy = true;

                    SM_EnemyController.SM_EnemyTargeting.AddTarget(targetTransform.root.gameObject);
                }
            }
            else
                canSeeEnemy = false;
        }
        else if (canSeeEnemy)
            canSeeEnemy = false;
    }

    IEnumerator ScanForEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(scanIterationTime);
            FieldOfViewScan(radius, fieldOfViewDegrees);
            FieldOfViewScan(radiusSmallArea, fieldOfViewDegreesSmallArea);
        }
    }

    public void SetDetection(bool _value) { canDetectEnemies = _value; }
}
