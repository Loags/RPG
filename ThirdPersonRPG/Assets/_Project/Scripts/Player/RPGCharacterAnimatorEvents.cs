using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AnimatorMoveEvent : UnityEvent<Vector3, Quaternion>
{
}

namespace LB
{
    public class RPGCharacterAnimatorEvents : MonoBehaviour
    {
        // Event call functions for Animation events.
        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnShoot = new UnityEvent();
        public UnityEvent OnFootR = new UnityEvent();
        public UnityEvent OnFootL = new UnityEvent();
        public UnityEvent OnLand = new UnityEvent();
        public UnityEvent OnWeaponSwitch = new UnityEvent();

        public AnimatorMoveEvent OnMove = new AnimatorMoveEvent();

        // Components.
        private RPGCharacterController rpgCharacterController;
        private Animator animator;

        void Awake()
        {
            // Get the controller from the parent
            rpgCharacterController = GetComponentInParent<RPGCharacterController>();
            if (rpgCharacterController == null)
            {
                Debug.LogError("RPGCharacterAnimatorEvents could not find RPGCharacterController in parent.", this);
            }

            animator = GetComponent<Animator>();
        }

        public void Hit() => OnHit.Invoke();
        public void Shoot() => OnShoot.Invoke();
        public void FootR() => OnFootR.Invoke();
        public void FootL() => OnFootL.Invoke();
        public void Land() => OnLand.Invoke();

        public void WeaponSwitch() => OnWeaponSwitch.Invoke();

        /// <summary>
        /// Animation Event callback to signal the end of an attack animation.
        /// Tells the RPGCharacterController to end the Attack action.
        /// </summary>
        public void AttackEndEvent()
        {
            Debug.Log("RPGCharacterAnimatorEvents.AttackEndEvent Received!");
            if (rpgCharacterController != null)
            {
                // Tell the controller to end the Attack action
                rpgCharacterController.EndAction(HandlerTypes.Attack);
            }
            else
            {
                Debug.LogWarning("AttackEndEvent called but RPGCharacterController is null.", this);
            }
        }

        // Used for animations that contain root motion to drive the character's
        // position and rotation using the "Motion" node of the animation file.
        void OnAnimatorMove()
        {
            if (!animator)
            {
                return;
            }

            // Not used when using Navmesh Navigation.
            if (rpgCharacterController != null && rpgCharacterController.IsNavigating)
            {
                return;
            }

            OnMove.Invoke(animator.deltaPosition, animator.rootRotation);
        }
    }
}