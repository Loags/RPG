using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private EnemyController EnemyController;
    [SerializeField] private string tagToCheck;
    private List<GameObject> hitObjects = new(); // Objects inside the HitBox

    private void Awake()
    {
        EnemyController = GetComponentInParent<EnemyController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(tagToCheck)) return; // Skip the rest if the tag doesn't match tagToCheck

        if (hitObjects.Count <= 0 || hitObjects.Contains(other.gameObject)) return; // Skip when the GameObject is already in the list

        hitObjects.Add(other.transform.root.gameObject); // Add the GameObject to the List
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagToCheck)) return; // Skip the rest if the tag doesn't match tagToCheck

        if (hitObjects.Count <= 0 || !hitObjects.Contains(other.gameObject)) return; // Skip when the GameObject is not in the list

        hitObjects.Remove(other.transform.root.gameObject); // Remove the GameObject from the List
    }


    public List<GameObject> GetHitObject() { return hitObjects; } // Make the List accessable to other scripts
    public bool CurrentTargetInList() // Check if the current target is in the hit object list
    {
        GameObject currentTarget = EnemyController.SM_EnemyTargeting.currentTarget;
        if (currentTarget == null) return false;
        return hitObjects.Contains(EnemyController.SM_EnemyTargeting.currentTarget);
    }
}
