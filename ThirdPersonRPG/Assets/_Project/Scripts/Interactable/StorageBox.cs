using LB.Inventory;
using UnityEngine;

namespace LB.Interactable
{
    public class StorageBox : Interactable
    {
        /// <summary>
        /// The inventory object associated with this storage box
        /// </summary>
        [Header("StorageBox Inventory")] 
        public InventoryObject inventory;

        /// <summary>
        /// Priority settings for this interactable type
        /// </summary>
        [Header("Interaction Settings")]
        [SerializeField] [Tooltip("Higher values take precedence over other interactables")] 
        private int storagePriority = 10;

        /// <summary>
        /// Gets the priority value for this storage box
        /// </summary>
        public override int InteractionPriority => storagePriority;

        /// <summary>
        /// Initializes the storage box and loads its inventory
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (inventory != null)
            {
                inventory.Load();
            }
            else
            {
                Debug.LogWarning("StorageBox is missing its InventoryObject reference.", this);
            }
        }

        /// <summary>
        /// Handles player interaction with the storage box, opening the storage UI.
        /// </summary>
        /// <param name="_rpgCharacterController">Reference to the player's character controller</param>
        public override void Interact(RPGCharacterController _rpgCharacterController)
        {
            if (_rpgCharacterController == null) return;

            base.Interact(_rpgCharacterController);

            if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INGAME)
            {
                StateManager.Instance.ChangeAppFlowState(AppStates.FlowStates.STORAGEBOX);

                // Define states for storage interaction
                LockableStates movementAndActions = LockableStates.Movement | LockableStates.Actions;
                LockableStates cameraLock = LockableStates.Camera;
                LockableStates cursorUnlock = LockableStates.Cursor;

                // Lock movement, actions, camera input; Unlock cursor
                _rpgCharacterController.Lock(movementAndActions | cameraLock);
                _rpgCharacterController.Unlock(cursorUnlock);
            }
            // The original commented-out block for closing with interaction key is preserved below and updated
            /*
             else if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.STORAGEBOX)
            {
                StateManager.Instance.ChangeAppFlowState(AppStates.FlowStates.INGAME);
                // Unlock movement, actions, camera input; Lock cursor
                _rpgCharacterController.Unlock(LockableStates.Movement | LockableStates.Actions | LockableStates.Camera);
                _rpgCharacterController.Lock(LockableStates.Cursor);
            }
            */
        }
    }
}