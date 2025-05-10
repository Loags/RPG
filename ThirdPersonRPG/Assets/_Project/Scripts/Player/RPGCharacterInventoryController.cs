using LB.Inventory;
using System.Collections;
using UnityEngine;

namespace LB
{
	public class RPGCharacterInventoryController : MonoBehaviour
	{
		private RPGCharacterController rpgCharacterController;
		public InventoryObject inventory;

		/// <summary>
		/// Returns whether the inventory is loaded or not
		/// </summary>
		public bool isInventoryLoaded => _isInventoryLoaded;

		private bool _isInventoryLoaded;


		private void Awake()
		{
			if (!TryGetComponent<RPGCharacterController>(out rpgCharacterController))
			{
				Debug.LogError("RPGCharacterInventoryController requires an RPGCharacterController component.", this);
				enabled = false;
				return;
			}
			StartCoroutine(LoadInventory());
		}

		private IEnumerator LoadInventory()
		{
			_isInventoryLoaded = false;
			if (inventory != null)
			{
				inventory.Load();
			}
			else
			{
				Debug.LogWarning("Inventory Object is not assigned in RPGCharacterInventoryController.", this);
			}
			yield return new WaitForSeconds(0.1f);
			_isInventoryLoaded = true;
		}

		/// <summary>
		/// Toggles the inventory state and corresponding character locks.
		/// </summary>
		public void ToggleInventory()
		{
			if (rpgCharacterController == null || rpgCharacterController.rpgCharacterGlobalInputBlocker) return;

			LockableStates movementAndActions = LockableStates.Movement | LockableStates.Actions;
			LockableStates cameraLock = LockableStates.Camera;
			LockableStates cursorUnlock = LockableStates.Cursor;

			if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INGAME)
			{
				StateManager.Instance.ChangeAppFlowState(AppStates.FlowStates.INVENTORY);
				rpgCharacterController.Lock(movementAndActions | cameraLock);
				rpgCharacterController.Unlock(cursorUnlock);
			}
			else if (StateManager.Instance.CurrentFlowState == AppStates.FlowStates.INVENTORY)
			{
				StateManager.Instance.ChangeAppFlowState(AppStates.FlowStates.INGAME);
				rpgCharacterController.Unlock(movementAndActions | cameraLock);
				rpgCharacterController.Lock(cursorUnlock);
			}
		}
	}
}