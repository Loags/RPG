using TMPro;
using UnityEngine;

namespace LB.Interactable
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Canvas object containing the interaction UI elements
        /// </summary>
        [SerializeField] private GameObject canvasObject;

        /// <summary>
        /// Base priority value for this interactable
        /// </summary>
        [SerializeField]
        [Tooltip("Higher values give this interactable higher precedence")]
        protected int basePriority = 0;

        /// <summary>
        /// Text component used to display interaction description
        /// </summary>
        private TMP_Text interactionDescriptionDisplay;

        /// <summary>
        /// Gets the priority of this interactable
        /// </summary>
        /// <remarks>
        /// Override this in derived classes to provide custom priority logic
        /// Higher values indicate higher priority
        /// </remarks>
        public virtual int InteractionPriority => basePriority;

        /// <summary>
        /// Initializes the component
        /// </summary>
        protected virtual void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Performs initial setup for the interactable
        /// </summary>
        protected virtual void Initialize()
        {
            HidePopUp();
        }

        /// <summary>
        /// Deactivates the interactable and hides its UI
        /// </summary>
        protected virtual void Deactivate()
        {
            gameObject.SetActive(false);
            HidePopUp();
        }

        /// <summary>
        /// Handles interaction with the player
        /// </summary>
        /// <param name="_rpgCharacterController">Reference to the character controller</param>
        public virtual void Interact(RPGCharacterController _rpgCharacterController)
        {
        }

        /// <summary>
        /// Shows the interaction UI
        /// </summary>
        public virtual void ShowPopUp()
        {
            if (canvasObject != null)
                canvasObject.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the interaction UI
        /// </summary>
        public virtual void HidePopUp()
        {
            if (canvasObject != null)
                canvasObject.gameObject.SetActive(false);
        }

        /// <summary>
        /// Gets the world position of this interactable
        /// </summary>
        /// <returns>World position of the interactable</returns>
        public Vector3 Position()
        {
            return transform.position;
        }
    }
}