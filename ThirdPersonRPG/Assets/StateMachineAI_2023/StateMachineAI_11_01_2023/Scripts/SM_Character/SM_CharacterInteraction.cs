using System.Collections.Generic;
using UnityEngine;
using Manager_Coroutine;
using System.Collections;

public class SM_CharacterInteraction : MonoBehaviour
{
    private SM_CharacterController sm_CharacterController;

    public Interactable currentInteractable;
    [SerializeField] private List<Interactable> potentialInteractables = new();
    private Coroutine updateTargetCoroutine;

    private IEnumerator Start()
    {
        sm_CharacterController = GetComponentInParent<SM_CharacterController>();
        yield return new WaitForEndOfFrame();
        CoroutineManager.InitializeCoroutine(ref updateTargetCoroutine, UpdateCurrentTarget(), sm_CharacterController.myMonoBehaviour);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            currentInteractable.Interact();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
            if (!potentialInteractables.Contains(interactable))
                AddInteractable(interactable);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
            AddInteractable(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
            RemoveInteractable(interactable);
    }

    private void AddInteractable(Interactable _newInteractable)
    {
        if (_newInteractable == currentInteractable) return;

        if (currentInteractable != null)
            currentInteractable.OnDefocus(); // Defocus previous interactable

        if (!potentialInteractables.Contains(_newInteractable))
            potentialInteractables.Add(_newInteractable);

        currentInteractable = GetClosestInteractable(); // Check if there is a new currentInteractable

        if (currentInteractable != null)
            currentInteractable.OnFocus(transform.parent); // Focus potential new interactable
    }

    public void RemoveInteractable(Interactable _interactable)
    {
        if (_interactable == null) return;

        potentialInteractables.Remove(_interactable);

        if (_interactable == currentInteractable)
        {
            currentInteractable.OnDefocus();
            currentInteractable = null;
        }

        currentInteractable = GetClosestInteractable();

        if (currentInteractable != null)
            currentInteractable.OnFocus(transform.parent);

        _interactable.OnDefocus();
    }

    private Interactable GetClosestInteractable()
    {
        float dist = float.MaxValue;
        float previousDist = float.MaxValue;

        Interactable closestInteractable = null;

        foreach (Interactable interactable in potentialInteractables)
        {
            previousDist = dist;
            dist = Vector3.Distance(transform.position, interactable.transform.position);
            if (dist < previousDist)
            {
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }

    private IEnumerator UpdateCurrentTarget()
    {
        Interactable clostestInteractable = GetClosestInteractable();

        if (clostestInteractable == null || clostestInteractable == currentInteractable) yield break;

        yield return new WaitForSeconds(0.1f);

        if (currentInteractable != null)
            currentInteractable.OnDefocus();

        currentInteractable = clostestInteractable;
        currentInteractable.OnFocus(transform.parent);
    }
}
