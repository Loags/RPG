using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LB.Utilities.DeveloperConsole;
using LB.Inventory;
using System;

namespace LB
{
	/// <summary>
	/// Defines the states that can be locked on the character controller.
	/// Used as bit flags to allow combining multiple states.
	/// </summary>
	[Flags]
	public enum LockableStates
	{
		None = 0,
		Movement = 1 << 0, // 1
		Actions = 1 << 1,  // 2
		Cursor = 1 << 2,   // 4
		Camera = 1 << 3,   // 8
		All = ~0           // -1 (all bits set)
	}

	/// <summary>
	/// RPGCharacterController is the main entry point for triggering animations and holds all the
	/// state related to a character. It is the core component, no other controller
	/// will run without it.
	/// </summary>
	public class RPGCharacterController : MonoBehaviour
	{
		/// <summary>
		/// Static reference to it self.
		/// Only use with caution!
		/// </summary>
		public static RPGCharacterController Instance;

		/// <summary>
		/// Event called when load on spawn is done.
		/// </summary>
		public event Action OnPreLoad = delegate { };

		/// <summary>
		/// Event called when actions are locked by an animation or state change.
		/// </summary>
		public event Action OnLockActions = delegate { };

		/// <summary>
		/// Event called when actions are unlocked.
		/// </summary>
		public event Action OnUnlockActions = delegate { };

		/// <summary>
		/// Event called when movement is locked by an animation or state change.
		/// </summary>
		public event Action OnLockMovement = delegate { };

		/// <summary>
		/// Event called when movement is unlocked.
		/// </summary>
		public event Action OnUnlockMovement = delegate { };

		/// <summary>
		/// Event called when the cursor is locked (hidden).
		/// </summary>
		public event Action OnLockCursor = delegate { };

		/// <summary>
		/// Event called when the cursor is unlocked (visible).
		/// </summary>
		public event Action OnUnLockCursor = delegate { };

		/// <summary>
		/// Event called when camera input is locked.
		/// </summary>
		public event Action OnLockCamera = delegate { };

		/// <summary>
		/// Event called when camera input is unlocked.
		/// </summary>
		public event Action OnUnLockCamera = delegate { };

		/// <summary>
		/// RPGCharacterInputSystemController component
		/// </summary>
		[HideInInspector] public RPGCharacterInputSystemController rpgCharacterInputSystemController;

		/// <summary>
		/// RPGCharacterInventoryController component
		/// </summary>
		[HideInInspector] public RPGCharacterInventoryController rpgCharacterInventoryController;

		/// <summary>
		/// RPGCharacterEquipmentController component
		/// </summary>
		[HideInInspector] public RPGCharacterEquipmentController rpgCharacterEquipmentController;

		/// <summary>
		/// RPGCharacterStats component
		/// </summary>
		[HideInInspector] public RPGCharacterStats rpgCharacterStats;

		/// <summary>
		/// Bool that controls each Character script if the global Input is being blocked or not by a higher force
		/// </summary>
		[HideInInspector] public bool rpgCharacterGlobalInputBlocker;

		/// <summary>
		/// Unity Animator component.
		/// </summary>
		[HideInInspector] public Animator animator;

		/// <summary>
		/// Animation speed control. Doesn't affect lock timing.
		/// </summary>
		public float animationSpeed = 1;

		/// <summary>
		/// IKHands component.
		/// </summary>
		[HideInInspector] public IKHands ikHands;

		/// <summary>
		/// Target for Aiming/Strafing.
		/// </summary>
		public Transform target;

		/// <summary>
		/// Returns whether the character can take actions. Considers locked state and navigation.
		/// </summary>
		public bool CanAction => !IsLocked(LockableStates.Actions) && !IsNavigating;

		/// <summary>
		/// Returns whether the character can face.
		/// </summary>
		public bool CanFace => _canFace;

		private bool _canFace = true;

		/// <summary>
		/// Returns whether the character can move.
		/// </summary>
		public bool CanMove => !IsLocked(LockableStates.Movement);

		/// <summary>
		/// Returns whether the character can sprint. Currently tied to CanMove.
		/// </summary>
		public bool CanSprint => CanMove;

		/// <summary>
		/// Returns whether the character can strafe.
		/// </summary>
		public bool CanStrafe => _canStrafe;

		private bool _canStrafe = true;

		/// <summary>
		/// Returns whether the AcquiringGround action is active, signifying that the character is
		/// landing on the ground. AcquiringGround is added by RPGCharacterMovementController.
		/// </summary>
		public bool AcquiringGround => TryGetHandlerActive(HandlerTypes.AcquiringGround);

		/// <summary>
		/// Returns whether the Aim action is active.
		/// </summary>
		public bool IsAiming => TryGetHandlerActive(HandlerTypes.Aim);

		/// <summary>
		/// Returns whether the Attack action is active.
		/// </summary>
		public bool IsAttacking => _isAttacking;

		private bool _isAttacking;

		/// <summary>
		/// Returns whether the Facing action is active.
		/// </summary>
		public bool IsFacing => TryGetHandlerActive(HandlerTypes.Face);

		/// <summary>
		/// Returns whether the Fall action is active. Fall is added by
		/// RPGCharacterMovementController.
		/// </summary>
		public bool IsFalling => TryGetHandlerActive(HandlerTypes.Fall);

		/// <summary>
		/// Returns whether the Idle action is active. Idle is added by
		/// RPGCharacterMovementController.
		/// </summary>
		public bool IsIdle => TryGetHandlerActive(HandlerTypes.Idle);

		/// <summary>
		/// Returns whether the Move action is active. Idle is added by
		/// RPGCharacterMovementController.
		/// </summary>
		public bool IsMoving => TryGetHandlerActive(HandlerTypes.Move);

		/// <summary>
		/// Returns whether the Sprint action is active.
		/// </summary>
		public bool IsSprinting => TryGetHandlerActive(HandlerTypes.Sprint);

		/// <summary>
		/// Returns whether the Navigation action is active. Navigation is added by
		/// RPGCharacterNavigationController.
		/// </summary>
		public bool IsNavigating => TryGetHandlerActive(HandlerTypes.Navigation);

		/// <summary>
		/// Returns whether the Roll action is active. Roll is added by
		/// RPGCharacterMovementController.
		/// </summary>
		public bool IsRolling => TryGetHandlerActive(HandlerTypes.Roll);

		/// <summary>
		/// Returns whether the Roll action is active. Roll is added by
		/// RPGCharacterMovementController.
		/// </summary>
		public bool IsKnockback => TryGetHandlerActive(HandlerTypes.Knockback);

		/// <summary>
		/// Returns whether the Roll action is active. Roll is added by
		/// RPGCharacterMovementController.
		/// </summary>
		public bool IsKnockdown => TryGetHandlerActive(HandlerTypes.Knockdown);

		/// <summary>
		/// Returns whether the Strafe action is active.
		/// </summary>
		public bool IsStrafing => TryGetHandlerActive(HandlerTypes.Strafe);

		/// <summary>
		/// Returns whether the MaintainingGround action is active, signifying that the character
		/// is on the ground. MaintainingGround is added by RPGCharacterMovementController. If the
		/// action does not exist, this defaults to true.
		/// </summary>
		public bool MaintainingGround => TryGetHandlerActive(HandlerTypes.MaintainingGround);

		/// <summary>
		/// Vector3 for move input. Use SetMoveInput to change this.
		/// </summary>
		public Vector3 MoveInput => _moveInput;

		private Vector3 _moveInput;

		/// <summary>
		/// Vector3 for aim input. Use SetAimInput to change this.
		/// </summary>
		public Vector3 AimInput => _aimInput;

		private Vector3 _aimInput;

		/// <summary>
		/// Vector3 for facing. Use SetFaceInput to change this.
		/// </summary>
		public Vector3 FaceInput => _faceInput;

		private Vector3 _faceInput;

		/// <summary>
		/// Vector3 for jump input. Use SetJumpInput to change this.
		/// </summary>
		public Vector3 JumpInput => _jumpInput;

		private Vector3 _jumpInput;

		/// <summary>
		/// Camera relative input in the XZ plane. This is calculated when SetMoveInput is called.
		/// </summary>
		public Vector3 CameraRelativeInput => _cameraRelativeInput;

		private Vector3 _cameraRelativeInput;

		/// <summary>
		/// Returns whether the cursor is visible or not
		/// True = invisible
		/// False = visible 
		/// </summary>
		public bool IsCursorLocked => IsLocked(LockableStates.Cursor);

		/// <summary>
		/// Returns whether the camera input via mouse movement is locked or not
		/// </summary>
		public bool IsCameraLocked => IsLocked(LockableStates.Camera);

		/// <summary>
		/// Returns whether the preload has been finished or not
		/// </summary>
		public bool IsPreLoadFinished => _isPreLoadFinished;

		private bool _isPreLoadFinished;

		/// <summary>
		/// Integer weapon number for the single equipped weapon slot. See the Weapon enum in Inventory namespace.
		/// </summary>
		[HideInInspector] public Weapon equippedWeapon = Weapon.Unarmed;

		/// <summary>
		/// Returns whether the currently equipped weapon is the primary melee weapon type.
		/// Uses the WeaponExtensions helper.
		/// </summary>
		public bool IsMeleeWeaponEquipped => equippedWeapon.IsMeleeWeapon();

		/// <summary>
		/// Returns whether the character is in the Unarmed state.
		/// </summary>
		public bool HasNoWeapon => equippedWeapon.HasNoWeapon();

		private Dictionary<string, IActionHandler> actionHandlers = new();

		/// <summary>
		/// Tracks the currently locked states using bit flags.
		/// </summary>
		private LockableStates _lockedStates = LockableStates.None;
		private LockableStates _prevLockedStatesForConsole = LockableStates.None; // Used by ConsoleFeedback

		/// <summary>
		/// Coroutine handle for the currently active timed lock, if any.
		/// </summary>
		private Coroutine _currentTimedLockCoroutine = null;

		#region Initialization

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != this)
			{
				Debug.LogWarning($"Duplicate RPGCharacterController instance found on {gameObject.name}. Destroying self.");
				Destroy(gameObject);
				return;
			}

			rpgCharacterInputSystemController = GetComponent<RPGCharacterInputSystemController>();
			rpgCharacterInventoryController = GetComponent<RPGCharacterInventoryController>();
			rpgCharacterEquipmentController = GetComponent<RPGCharacterEquipmentController>();
			rpgCharacterStats = GetComponent<RPGCharacterStats>();
			animator = GetComponentInChildren<Animator>();
			ikHands = GetComponentInChildren<IKHands>();

			if (!animator)
			{
				Debug.LogError("ERROR: THERE IS NO ANIMATOR COMPONENT ON CHILD OF CHARACTER.");
				Debug.Break();
			}

			animator.gameObject.AddComponent<RPGCharacterAnimatorEvents>();
			animator.updateMode = AnimatorUpdateMode.Normal;
			animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			animator.SetInteger(AnimationParameters.Weapon, 0);
			animator.SetInteger(AnimationParameters.WeaponSwitch, 0);

			SetHandler(HandlerTypes.Attack, new Attack());
			SetHandler(HandlerTypes.Face, new SimpleActionHandler(StartFace, EndFace));
			SetHandler(HandlerTypes.Null, new Null());
			SetHandler(HandlerTypes.SlowTime, new SlowTime());
			SetHandler(HandlerTypes.Strafe, new SimpleActionHandler(StartStrafe, EndStrafe));

			_lockedStates = LockableStates.Cursor;
			if(Cursor.visible) Cursor.visible = false;
			if(Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;

			StartCoroutine(nameof(PreLoad));

			if (target != null)
			{
				SetAimInput(target.transform.position);
			}
			else
			{
				Debug.LogWarning("RPGCharacterController: Target for aiming is not assigned.", this);
				SetAimInput(transform.position + transform.forward);
			}
		}

		private void OnEnable()
		{
			DeveloperConsoleBehaviour.instance.OnConsoleToggled += ConsoleFeedback;
		}

		private void OnDisable()
		{
			DeveloperConsoleBehaviour.instance.OnConsoleToggled -= ConsoleFeedback;

			if (_currentTimedLockCoroutine != null)
			{
				StopCoroutine(_currentTimedLockCoroutine);
				_currentTimedLockCoroutine = null;
			}
		}

		#endregion

		#region Actions

		/// <summary>
		/// Set an action handler.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		/// <param name="handler">The handler associated with this action.</param>
		public void SetHandler(string action, IActionHandler handler)
		{
			actionHandlers[action] = handler;
		}

		/// <summary>
		/// Get an action handler by name. If it doesn't exist, return the Null handler.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		public IActionHandler GetHandler(string action)
		{
			if (HandlerExists(action))
			{
				return actionHandlers[action];
			}

			Debug.LogError("RPGCharacterController: No handler for action \"" + action + "\"");
			return actionHandlers[HandlerTypes.Null];
		}

		/// <summary>
		/// Check if a handler exists.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		/// <returns>Whether or not that action exists on this controller.</returns>
		public bool HandlerExists(string action)
		{
			return actionHandlers.ContainsKey(action);
		}

		public bool TryGetHandlerActive(string action)
		{
			return HandlerExists(action) && IsActive(action);
		}

		/// <summary>
		/// Check if an action is active.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		/// <returns>Whether the action is active. If the action does not exist, returns false.</returns>
		public bool IsActive(string action)
		{
			return GetHandler(action).IsActive();
		}

		/// <summary>
		/// Check if an action can be started.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		/// <returns>Whether the action can be started. If the action does not exist, returns false.</returns>
		public bool CanStartAction(string action)
		{
			return GetHandler(action).CanStartAction(this);
		}

		public bool TryStartAction(string action, object context = null)
		{
			if (!CanStartAction(action))
			{
				return false;
			}

			StartAction(action, context);
			return true;
		}

		public bool TryEndAction(string action)
		{
			if (!CanEndAction(action))
			{
				return false;
			}

			EndAction(action);
			return true;
		}

		/// <summary>
		/// Check if an action can be ended.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		/// <returns>Whether the action can be ended. If the action does not exist, returns false.</returns>
		public bool CanEndAction(string action)
		{
			return GetHandler(action).CanEndAction(this);
		}

		/// <summary>
		/// Start the action with the specified context. If the action does not exist, there is no effect.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		/// <param name="context">Contextual object used by this action. Leave blank if none is required.</param>
		public void StartAction(string action, object context = null)
		{
			GetHandler(action).StartAction(this, context);
		}

		/// <summary>
		/// End the action. If the action does not exist, there is no effect.
		/// </summary>
		/// <param name="action">Name of the action.</param>
		public void EndAction(string action)
		{
			GetHandler(action).EndAction(this);
		}

		#endregion

		#region Updates

		private void LateUpdate()
		{
			animator.SetFloat(AnimationParameters.AnimationSpeed, animationSpeed);
		}

		#endregion

		#region Input

		/// <summary>
		/// Set move input. This method expects the x-axis to be left-right input and the
		/// y-axis to be up-down input.
		///
		/// The z-axis is ignored, but the type is a Vector3 in case you wish to use the z-axis.
		///
		/// This method computes CameraRelativeInput using the x and y axis of the move input
		/// and the main camera, producing a normalized Vector3 in the XZ plane.
		/// </summary>
		/// <param name="_moveInput">Vector3 move input</param>
		public void SetMoveInput(Vector3 newMoveInput)
		{
			this._moveInput = newMoveInput;

			if (Camera.main == null)
			{
				Debug.LogError("SetMoveInput: Camera.main is null. Cannot calculate camera relative input.");
				_cameraRelativeInput = Vector3.zero;
				return;
			}

			Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;

			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			Vector3 relativeVelocity = _moveInput.x * right + _moveInput.y * forward;

			if (relativeVelocity.magnitude > 1.0f)
			{
				relativeVelocity.Normalize();
			}

			_cameraRelativeInput = relativeVelocity;
		}

		/// <summary>
		/// Set facing input. This is a position in world space of the object that the character
		/// is facing towards.
		/// </summary>
		/// <param name="_faceInput">Vector3 face input.</param>
		public void SetFaceInput(Vector3 newFaceInput)
		{
			this._faceInput = newFaceInput;
		}

		/// <summary>
		/// Set aim input. This is a position in world space of the object that the character
		/// is aiming at, so that you can easily lock on to a moving target.
		/// </summary>
		/// <param name="_aimInput">Vector3 aim input.</param>
		public void SetAimInput(Vector3 newAimInput)
		{
			this._aimInput = newAimInput;
		}

		/// <summary>
		/// Set jump input. Use this with Vector3.up and Vector3.down (y-axis).
		///
		/// The X and Z axes are  ignored, but the type is a Vector3 in case you wish to
		/// use the X and Z axes for other actions.
		/// </summary>
		/// <param name="_jumpInput">Vector3 jump input.</param>
		public void SetJumpInput(Vector3 newJumpInput)
		{
			this._jumpInput = newJumpInput;
		}

		#endregion

		#region Movement

		/// <summary>
		/// Dive Roll. Locks movement and actions for the duration.
		/// </summary>
		/// <param name="rollType">1- Forward.</param>
		public void DiveRoll(DiveRollType rollType)
		{
			if (!CanAction) return;

			animator.TriggerDiveRoll(rollType);
			Lock(LockableStates.Movement | LockableStates.Actions, duration: 1.0f);
			SetIKPause(1.05f);
		}

		/// <summary>
		/// Knockback in the specified direction. Locks movement and actions.
		/// </summary>
		/// <param name="direction">1- Backwards, 2- Backward version2.</param>
		public void Knockback(KnockbackType direction)
		{
			animator.TriggerKnockback(direction);
			float lockDuration = 1.0f;
			float ikPause = 1.125f;

			if (direction == KnockbackType.Knockback2)
			{
				lockDuration = 0.8f;
				ikPause = 1.0f;
			}

			Lock(LockableStates.Movement | LockableStates.Actions, duration: lockDuration);
			SetIKPause(ikPause);
		}

		/// <summary>
		/// Knockdown in the specified direction. Locks movement and actions for a longer duration.
		/// </summary>
		/// <param name="direction">1- Backwards.</param>
		public void Knockdown(KnockdownType direction)
		{
			animator.TriggerKnockdown(direction);
			Lock(LockableStates.Movement | LockableStates.Actions, duration: 5.25f);
			SetIKPause(5.25f);
		}

		#endregion

		#region Combat

		/// <summary>
		/// Trigger an attack animation. Locks movement and actions for the specified duration.
		/// </summary>
		/// <param name="attackNumber">Animation number to play.</param>
		/// <param name="duration">Duration in seconds that movement/actions are locked.</param>
		public void Attack(int attackNumber, float duration)
		{
			Debug.Log($"RPGCharacterController.Attack: Triggering Attack Animation Number: {attackNumber}");
			_isAttacking = true;
			Lock(LockableStates.Movement | LockableStates.Actions, duration: duration);
			AnimatorTrigger attackTriggerType = AnimatorTrigger.AttackTrigger;
			animator.SetActionTrigger(attackTriggerType, attackNumber);
		}

		/// <summary>
		/// Trigger the running attack animation.
		/// Simplified: Assumes a single weapon and no specific 'Side'.
		/// Movement/action lock should be handled by the calling Action Handler if needed for running attacks.
		/// </summary>
		public void RunningAttack()
		{
			int attackNumber = 1;
			if (HasNoWeapon) attackNumber = 1;
			else attackNumber = 4;
			animator.SetActionTrigger(AnimatorTrigger.AttackTrigger, attackNumber);
		}

		/// <summary>
		/// Placeholder for starting facing behavior (part of the Face Action Handler).
		/// </summary>
		public void StartFace() { /* Logic handled by Facing Action Handler if needed */ }

		/// <summary>
		/// Placeholder for ending facing behavior (part of the Face Action Handler).
		/// </summary>
		public void EndFace() { /* Logic handled by Facing Action Handler if needed */ }

		/// <summary>
		/// Placeholder for starting strafe behavior (part of the Strafe Action Handler).
		/// </summary>
		public void StartStrafe() { /* Logic handled by Strafe Action Handler if needed */ }

		/// <summary>
		/// Placeholder for ending strafe behavior (part of the Strafe Action Handler).
		/// </summary>
		public void EndStrafe() { /* Logic handled by Strafe Action Handler if needed */ }

		/// <summary>
		/// Trigger the GetHit animation. Locks movement and actions briefly.
		/// </summary>
		public void GetHit(int hitNumber)
		{
			animator.TriggerGettingHit(hitNumber);
			Lock(LockableStates.Movement | LockableStates.Actions, duration: 0.4f, delay: 0.1f);
			SetIKPause(0.6f);
		}

		#endregion

		#region Misc

		/// <summary>
		/// Gets the GameObject containing the Animator component.
		/// </summary>
		/// <returns>The Animator's GameObject, or null if animator is not assigned.</returns>
		public GameObject GetAnimatorTarget()
		{
			return animator != null ? animator.gameObject : null;
		}

		/// <summary>
		/// Returns the length of the current animation clip on the specified layer.
		/// </summary>
		/// <param name="animationLayer">The index of the animation layer.</param>
		/// <returns>The length of the current clip in seconds, or 0 if animator/clip info is unavailable.</returns>
		private float CurrentAnimationLength(int animationLayer)
		{
			if (animator == null || animationLayer < 0 || animationLayer >= animator.layerCount)
			{
				return 0f;
			}

			AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(animationLayer);
			if (clipInfo.Length > 0 && clipInfo[0].clip != null)
			{
				return clipInfo[0].clip.length;
			}

			return 0f;
		}

		/// <summary>
		/// Turns IK weights to 0 instantly.
		/// </summary>
		public void SetIKOff()
		{
			if (ikHands == null) return;
			ikHands.BlendIK(false, 0f, 0f, Weapon.Unarmed);
		}

		/// <summary>
		/// Turns IK weights to 1 instantly or smoothly based on BlendIK.
		/// </summary>
		public void SetIKOn()
		{
			if (ikHands != null)
			{
				ikHands.BlendIK(true, 0f, 0f, equippedWeapon);
			}
		}

		/// <summary>
		/// Pauses IK updates for a specified duration using the IKHands component.
		/// </summary>
		/// <param name="pauseTime">Duration in seconds to pause IK updates.</param>
		public void SetIKPause(float pauseTime)
		{
			if (ikHands != null && ikHands.isUsed)
			{
				ikHands.SetIKPause(pauseTime);
			}
		}

		/// <summary>
		/// Coroutine to handle pre-loading necessary components like inventory and equipment.
		/// Invokes OnPreLoad event before and after loading.
		/// </summary>
		private IEnumerator PreLoad()
		{
			_isPreLoadFinished = false;
			OnPreLoad();
			yield return new WaitUntil(() =>
				(rpgCharacterInventoryController != null && rpgCharacterInventoryController.isInventoryLoaded) &&
				(rpgCharacterEquipmentController != null && rpgCharacterEquipmentController.isEquipmentLoaded)
			);
			_isPreLoadFinished = true;
			OnPreLoad();
		}

		#endregion

		#region Console Feedback

		/// <summary>
		/// Handles locking/unlocking character states when the developer console is toggled.
		/// </summary>
		/// <param name="isConsoleOpen">True if the console is opening, false if closing.</param>
		private void ConsoleFeedback(bool isConsoleOpen)
		{
			if (isConsoleOpen)
			{
				_prevLockedStatesForConsole = _lockedStates;

				LockableStates statesToLockForConsole = LockableStates.Movement | LockableStates.Actions | LockableStates.Camera;
				LockableStates statesToUnlockForConsole = LockableStates.Cursor;

				ApplyLock(statesToLockForConsole);
				ApplyUnlock(statesToUnlockForConsole);

				rpgCharacterGlobalInputBlocker = true;
			}
			else
			{
				LockableStates currentStateInConsole = _lockedStates;
				LockableStates targetState = _prevLockedStatesForConsole;

				LockableStates statesToReLock = targetState & ~currentStateInConsole;
				LockableStates statesToReUnlock = currentStateInConsole & ~targetState;

				ApplyLock(statesToReLock);
				ApplyUnlock(statesToReUnlock);

				rpgCharacterGlobalInputBlocker = false;
			}
		}

		#endregion

		#region Lock System Refactor

		/// <summary>
		/// Checks if one or more states are currently locked.
		/// </summary>
		/// <param name="statesToCheck">The state(s) to check using bit flags.</param>
		/// <returns>True if any of the specified states are locked, false otherwise.</returns>
		public bool IsLocked(LockableStates statesToCheck)
		{
			return (_lockedStates & statesToCheck) != 0;
		}

		/// <summary>
		/// Locks the specified character states. Can be timed with an optional delay.
		/// </summary>
		/// <param name="statesToLock">The state(s) to lock (e.g., LockableStates.Movement | LockableStates.Actions).</param>
		/// <param name="duration">If > 0, the lock will automatically be released after this duration (in seconds).</param>
		/// <param name="delay">If > 0, the lock will be applied after this delay (in seconds).</param>
		public void Lock(LockableStates statesToLock, float duration = 0f, float delay = 0f)
		{
			if (_currentTimedLockCoroutine != null)
			{
				StopCoroutine(_currentTimedLockCoroutine);
				_currentTimedLockCoroutine = null;
			}

			if (delay > 0f || duration > 0f)
			{
				_currentTimedLockCoroutine = StartCoroutine(_DelayedLockUnlock(statesToLock, duration, delay));
			}
			else
			{
				ApplyLock(statesToLock);
			}
		}

		/// <summary>
		/// Unlocks the specified character states.
		/// </summary>
		/// <param name="statesToUnlock">The state(s) to unlock (e.g., LockableStates.Movement | LockableStates.Actions).</param>
		public void Unlock(LockableStates statesToUnlock)
		{
			ApplyUnlock(statesToUnlock);
		}

		/// <summary>
		/// Coroutine to handle delayed and timed locks/unlocks.
		/// </summary>
		private IEnumerator _DelayedLockUnlock(LockableStates statesToLock, float duration, float delay)
		{
			if (delay > 0f)
			{
				yield return new WaitForSeconds(delay);
			}

			ApplyLock(statesToLock);

			if (duration > 0f)
			{
				yield return new WaitForSeconds(duration);

				if (_currentTimedLockCoroutine != null && ReferenceEquals(_currentTimedLockCoroutine, GetRunningCoroutine()))
				{
					ApplyUnlock(statesToLock);
					_currentTimedLockCoroutine = null;
				}
			}
			else
			{
				_currentTimedLockCoroutine = null;
			}
		}

		private Coroutine GetRunningCoroutine() => _currentTimedLockCoroutine;

		/// <summary>
		/// Internal method to apply locks, update state, and trigger events.
		/// </summary>
		private void ApplyLock(LockableStates statesToLock)
		{
			LockableStates previousStates = _lockedStates;
			_lockedStates |= statesToLock;

			LockableStates newlyLocked = _lockedStates & ~previousStates;

			if ((newlyLocked & LockableStates.Movement) != 0) OnLockMovement();
			if ((newlyLocked & LockableStates.Actions) != 0) OnLockActions();
			if ((newlyLocked & LockableStates.Cursor) != 0)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				OnLockCursor();
			}
			if ((newlyLocked & LockableStates.Camera) != 0) OnLockCamera();
		}

		/// <summary>
		/// Internal method to apply unlocks, update state, and trigger events.
		/// </summary>
		private void ApplyUnlock(LockableStates statesToUnlock)
		{
			LockableStates previousStates = _lockedStates;
			_lockedStates &= ~statesToUnlock;

			LockableStates newlyUnlocked = previousStates & ~_lockedStates;

			if ((newlyUnlocked & LockableStates.Movement) != 0) OnUnlockMovement();
			if ((newlyUnlocked & LockableStates.Actions) != 0)
			{
				if (_isAttacking) _isAttacking = false;
				OnUnlockActions();
			}
			if ((newlyUnlocked & LockableStates.Cursor) != 0)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				OnUnLockCursor();
			}
			if ((newlyUnlocked & LockableStates.Camera) != 0) OnUnLockCamera();
		}

		#endregion
	}
}