using UnityEngine;

namespace LB.Interactable
{
    public interface IInteractable
    {
        /// <summary>
        /// Performs the interaction behavior when player interacts with this object
        /// </summary>
        /// <param name="_rpgCharacterController">Reference to the player's character controller</param>
        public void Interact(RPGCharacterController _rpgCharacterController);

        /// <summary>
        /// Shows the interaction popup or UI element
        /// </summary>
        public void ShowPopUp();

        /// <summary>
        /// Hides the interaction popup or UI element
        /// </summary>
        public void HidePopUp();

        /// <summary>
        /// Gets the world position of the interactable
        /// </summary>
        /// <returns>World position of the interactable</returns>
        public Vector3 Position();

        /// <summary>
        /// Gets the priority value of this interactable
        /// </summary>
        /// <remarks>
        /// Higher values indicate higher priority. This is used for determining
        /// which interactable should be selected when multiple are in range.
        /// </remarks>
        public int InteractionPriority { get; }
    }
}