using UnityEngine;

namespace LB
{
    public class BackButtonController : MonoBehaviour
    {
        /// <summary>
        /// Handles the action when a generic 'back' or 'close' button is pressed in a UI.
        /// Transitions state and adjusts character locks accordingly.
        /// </summary>
        public void CloseButtonFlow()
        {
            // Ensure RPGCharacterController instance exists before trying to use it
            var playerController = RPGCharacterController.Instance;
            if (playerController == null)
            {
                Debug.LogError("BackButtonController cannot find RPGCharacterController.Instance!", this);
                // Potentially transition to a safe state like MainMenu if possible
                StateManager.Instance.ChangeAppFlowState(AppStates.FlowStates.MAINMENU);
                return;
            }

            // Define common lock states for returning to INGAME
            LockableStates ingameLocks = LockableStates.Cursor; // Cursor locked
            LockableStates ingameUnlocks = LockableStates.Movement | LockableStates.Actions | LockableStates.Camera; // Movement, Actions, Camera unlocked


            AppStates.FlowStates targetState = AppStates.FlowStates.MAINMENU; // Default target

            switch (StateManager.Instance.CurrentFlowState) // Check CURRENT state to decide where to go back TO
            {
                // Cases where 'Back' likely means returning to INGAME from a menu opened during gameplay
                case AppStates.FlowStates.INVENTORY:
                case AppStates.FlowStates.STORAGEBOX:
                case AppStates.FlowStates.PAUSEMENU: // Assuming PauseMenu also returns to InGame
                // Add other potential in-game menus here (e.g., Dialogue, Quest Log)
                    targetState = AppStates.FlowStates.INGAME;
                    // Restore InGame lock states: Unlock Movement/Actions/Camera, Lock Cursor
                    playerController.Unlock(ingameUnlocks);
                    playerController.Lock(ingameLocks);
                    break;

                // Cases where 'Back' likely means returning to MAINMENU
                case AppStates.FlowStates.SETTINGS:
                case AppStates.FlowStates.CREDTIS: // Corrected typo from original code
                case AppStates.FlowStates.CHARACTERSELECT: // Assuming back from char select goes to main menu
                    targetState = AppStates.FlowStates.MAINMENU;
                    // Adjust locks for main menu if necessary (e.g., unlock cursor)
                    // Assuming main menu needs unlocked cursor, unlocked actions/movement/camera:
                     playerController.Unlock(LockableStates.All); // Unlock everything
                     // playerController.Lock(LockableStates.None); // Equivalent
                    break;

                // Add specific logic for other states if needed
                // case AppStates.FlowStates.LOADINGSCREEN: // Cannot go back from loading?
                // case AppStates.FlowStates.INITIALIZATION: // Cannot go back from init?
                //     break;

                // Default case: Go to MainMenu (or handle error)
                default:
                    Debug.LogWarning($"BackButtonController: Unhandled current state {StateManager.Instance.CurrentFlowState}. Returning to Main Menu.");
                    targetState = AppStates.FlowStates.MAINMENU;
                     playerController.Unlock(LockableStates.All); // Ensure unlocked state for main menu
                    break;
            }

            // Perform the state change
            StateManager.Instance.ChangeAppFlowState(targetState);

        }
    }
}