using UnityEngine;

namespace LB{
    public static class ControllerExtensions
    {
        public static void DebugController(this RPGCharacterController controller)
        {
            Debug.Log("CONTROLLER SETTINGS---------------------------");
            Debug.Log("AnimationSpeed: " + controller.animationSpeed);
            Debug.Log("canAction: " + controller.CanAction);
            Debug.Log("canFace: " + controller.CanFace);
            Debug.Log("canMove: " + controller.CanMove);
            Debug.Log("canSprint: " + controller.CanSprint);
            Debug.Log("canStrafe: " + controller.CanStrafe);
            Debug.Log("acquiringGround: " + controller.AcquiringGround);
            Debug.Log("maintainingGround: " + controller.MaintainingGround);
            Debug.Log("isAttacking: " + controller.IsAttacking);
            Debug.Log("isFacing: " + controller.IsFacing);
            Debug.Log("isFalling: " + controller.IsFalling);
            Debug.Log("isIdle: " + controller.IsIdle);
            Debug.Log("isMoving: " + controller.IsMoving);
            Debug.Log("isSprinting: " + controller.IsSprinting);
            Debug.Log("isNavigating: " + controller.IsNavigating);
            Debug.Log("isRolling: " + controller.IsRolling);
            Debug.Log("isKnockback: " + controller.IsKnockback);
            Debug.Log("isKnockdown: " + controller.IsKnockdown);
            Debug.Log("isStrafing: " + controller.IsStrafing);
            Debug.Log("moveInput: " + controller.MoveInput);
            Debug.Log("aimInput: " + controller.AimInput);
            Debug.Log("jumpInput: " + controller.JumpInput);
            Debug.Log("cameraRelativeInput: " + controller.CameraRelativeInput);
            Debug.Log("equippedWeapon: " + controller.equippedWeapon);
            Debug.Log("isMeleeWeaponEquipped: " + controller.IsMeleeWeaponEquipped);
        }

        /// <summary>
        /// Returns true if the character is moving.
        /// </summary>
        /// <param name="controller">Controller.</param>
        public static bool IsMoving(this RPGCharacterController controller)
        {
            return controller.MoveInput.magnitude > 0.1f;
        }

        /// <summary>
        /// Returns true if character is moving fast.
        /// </summary>
        /// <param name="controller">Controller.</param>
        public static bool IsMovingFast(this RPGCharacterController controller)
        {
            return controller.MoveInput.magnitude > 0.9f;
        }

        /// <summary>
        /// Returns true if the animator is in the state "Idle".
        /// </summary>
        /// <param name="controller">Controller.</param>
        public static bool IsAnimatorIdle(this RPGCharacterController controller)
        {
            return controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        }

        /// <summary>
        /// Returns true if the character has a weapon assigned.
        /// </summary>
        public static bool HasWeapon(this RPGCharacterController controller)
        {
            // Check the single equipped weapon
            return controller.equippedWeapon != LB.Inventory.Weapon.Unarmed; 
        }
    }
}