using LB.Interactable;
using LB.Utilities;
using LB_Manager_Coroutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LB
{
    public class RPGCharacterInteractionController : MonoBehaviour
    {
        /// <summary>
        /// Reference to the RPGCharacterController component.
        /// </summary>
        private RPGCharacterController rpgCharacterController;

        /// <summary>
        /// LayerMask for detecting interactable objects.
        /// </summary>
        [SerializeField] private LayerMask interactableLayerMask;

        /// <summary>
        /// Radius within which interactable objects can be detected.
        /// </summary>
        [SerializeField] private float interactionRadius = 0.2f;

        /// <summary>
        /// Threshold distance to prioritize very close interactables.
        /// </summary>
        [SerializeField] private float closeDistanceThreshold = 0.5f;

        /// <summary>
        /// When true, extremely close interactables will override priority settings
        /// </summary>
        [SerializeField] [Tooltip("If enabled, very close interactables can override priority settings")]
        private bool closeDistanceOverridesPriority = false;

        /// <summary>
        /// Currently prioritized interactable object.
        /// </summary>
        [SerializeField][ReadOnly] private IInteractable currentInteractable;

        /// <summary>
        /// Interval for updating interactables pop-up.
        /// </summary>
        [SerializeField] private float updateInterval = 0.2f;

        /// <summary>
        /// Coroutine for updating interactables pop-up.
        /// </summary>
        private Coroutine updateInteractablesPopUp;

        /// <summary>
        /// List of interactables currently within range of the player.
        /// </summary>
        private readonly List<IInteractable> interactablesInRange = new List<IInteractable>();

        /// <summary>
        /// Pre-allocated buffer for physics queries to avoid garbage collection.
        /// </summary>
        private readonly Collider[] colliderBuffer = new Collider[10];

        /// <summary>
        /// Initializes the component by getting the required references.
        /// </summary>
        private void Awake()
        {
            rpgCharacterController = GetComponent<RPGCharacterController>();
        }

        /// <summary>
        /// Starts the coroutine for updating interactable pop-ups.
        /// </summary>
        private void Start()
        {
            CoroutineManager.InitializeCoroutine(ref updateInteractablesPopUp, UpdateInteractablePopUp(), this);
        }

        /// <summary>
        /// Handles interaction with the currently prioritized interactable object.
        /// </summary>
        public void Interaction()
        {
            if (!rpgCharacterController.CanAction) return;

            if (currentInteractable != null)
            {
                currentInteractable.Interact(rpgCharacterController);
                currentInteractable = null;
            }
        }

        /// <summary>
        /// Coroutine that periodically updates the interactables pop-up based on the update interval.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator UpdateInteractablePopUp()
        {
            while (true)
            {
                if (rpgCharacterController.CanAction)
                {
                    UpdateCurrentInteractable();
                }
                else if (currentInteractable != null)
                {
                    currentInteractable.HidePopUp();
                    currentInteractable = null;
                }

                yield return new WaitForSeconds(updateInterval);
            }
        }

        /// <summary>
        /// Updates the currently prioritized interactable object based on distance and direction.
        /// </summary>
        private void UpdateCurrentInteractable()
        {
            try
            {
                // Find all interactables in range using pre-allocated buffer
                int hitCount = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, colliderBuffer, interactableLayerMask);

                // Clear previous list but don't hide popups yet
                interactablesInRange.Clear();

                // Collect all valid interactables
                for (int i = 0; i < hitCount; i++)
                {
                    if (colliderBuffer[i].TryGetComponent(out IInteractable interactable))
                    {
                        interactablesInRange.Add(interactable);
                    }
                }

                if (interactablesInRange.Count > 0)
                {
                    IInteractable highestPriority = GetHighestPriorityInteractable(interactablesInRange);

                    // Only update if the highest priority interactable has changed
                    if (currentInteractable != highestPriority)
                    {
                        // Hide the previous interactable's popup if it exists
                        if (currentInteractable != null)
                        {
                            currentInteractable.HidePopUp();
                        }

                        // Update current interactable and show its popup
                        currentInteractable = highestPriority;
                        if (currentInteractable != null)
                        {
                            currentInteractable.ShowPopUp();
                        }
                    }
                }
                else
                {
                    // No interactables in range, hide current if exists
                    if (currentInteractable != null)
                    {
                        currentInteractable.HidePopUp();
                        currentInteractable = null;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in UpdateCurrentInteractable: {e.Message}");
                if (currentInteractable != null)
                {
                    currentInteractable.HidePopUp();
                    currentInteractable = null;
                }
            }
        }

        /// <summary>
        /// Determines the highest priority interactable from a list based on interaction priority and proximity
        /// </summary>
        /// <param name="interactables">List of interactables to evaluate.</param>
        /// <returns>The interactable with the highest priority.</returns>
        private IInteractable GetHighestPriorityInteractable(List<IInteractable> interactables)
        {
            try
            {
                if (interactables == null || interactables.Count == 0)
                    return null;
                
                Vector3 playerPosition = transform.position;
                Vector3 playerForward = transform.forward;

                // First, find any interactables within close distance threshold
                List<IInteractable> closeInteractables = new List<IInteractable>();
                foreach (IInteractable interactable in interactables)
                {
                    if (interactable == null) continue;
                    
                    float distance = Vector3.Distance(playerPosition, interactable.Position());
                    if (distance <= closeDistanceThreshold)
                    {
                        closeInteractables.Add(interactable);
                    }
                }

                // If we have close interactables and close distance override is enabled
                if (closeInteractables.Count > 0 && closeDistanceOverridesPriority)
                {
                    // Find the closest one among the close interactables
                    IInteractable closestInteractable = null;
                    float closestDistance = float.MaxValue;

                    foreach (IInteractable interactable in closeInteractables)
                    {
                        float distance = Vector3.Distance(playerPosition, interactable.Position());
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestInteractable = interactable;
                        }
                    }

                    if (closestInteractable != null)
                    {
                        //Debug.Log($"Selected close interactable at distance {closestDistance:F2}");
                        return closestInteractable;
                    }
                }

                // If no close interactables or close distance override is disabled,
                // proceed with normal priority-based selection
                IInteractable highestPriorityInteractable = null;
                int highestPriority = int.MinValue;
                List<IInteractable> highestPriorityInteractables = new List<IInteractable>();

                // First pass: find highest priority value
                foreach (IInteractable interactable in interactables)
                {
                    if (interactable == null) continue;
                    
                    int priority = interactable.InteractionPriority;
                    
                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        highestPriorityInteractable = interactable;
                        highestPriorityInteractables.Clear();
                        highestPriorityInteractables.Add(interactable);
                    }
                    else if (priority == highestPriority)
                    {
                        highestPriorityInteractables.Add(interactable);
                    }
                }

                // If only one highest priority interactable found, return it
                if (highestPriorityInteractables.Count == 1)
                {
                    Debug.Log($"Selected highest priority interactable with priority {highestPriority}");
                    return highestPriorityInteractable;
                }

                // If multiple interactables have the same highest priority, choose based on angle and distance
                IInteractable bestInteractable = null;
                float bestScore = float.MaxValue;

                foreach (IInteractable interactable in highestPriorityInteractables)
                {
                    Vector3 directionToInteractable = (interactable.Position() - playerPosition).normalized;
                    float angle = Vector3.Angle(playerForward, directionToInteractable);
                    float distance = Vector3.Distance(playerPosition, interactable.Position());
                    
                    // Combined score: angle * 0.7 + distance * 0.3
                    float score = (angle * 0.7f) + (distance * 0.3f);
                    
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestInteractable = interactable;
                    }
                }

                Debug.Log($"Selected best interactable with score {bestScore:F2}");
                return bestInteractable;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in GetHighestPriorityInteractable: {e.Message}");
                return null;
            }
        }
    }
}