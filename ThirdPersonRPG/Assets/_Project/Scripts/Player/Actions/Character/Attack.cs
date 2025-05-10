using LB.Inventory;
using UnityEngine;

namespace LB
{
    public class Attack : BaseActionHandler<AttackContext>
    {
        public override bool CanStartAction(RPGCharacterController controller)
        {
            // Prevent starting attack if not grounded or if already active or cannot perform actions
            bool canStart = !active && controller.CanAction && controller.MaintainingGround;
            // Debug.Log($"Attack.CanStartAction: !active={!active}, controller.CanAction={controller.CanAction}, controller.MaintainingGround={controller.MaintainingGround}, Result={canStart}"); // Optional detailed log
            return canStart;
        }

        public override bool CanEndAction(RPGCharacterController controller)
        {
            return active;
        }

        protected override void _StartAction(RPGCharacterController controller, AttackContext context)
        {
            Debug.Log("Attack._StartAction called. Setting active = true.");
            if (!controller.TryGetComponent<RPGCharacterWeaponController>(out var rpgCharacterWeaponController))
            {
                Debug.LogError("Attack action requires RPGCharacterWeaponController.", controller.gameObject);
                return; 
            }

            int attackNumber = context.number; 
            string contextType = context.type;
            Weapon weaponType = controller.equippedWeapon;
            float duration = 0f;

            if (attackNumber == -1)
            {
                switch (contextType)
                {
                    case "Attack":
                        attackNumber = AnimationData.RandomAttackNumber(weaponType); 
                        break;
                    case "Special":
                        attackNumber = 1;
                        break;
                    default:
                        Debug.LogWarning($"Unhandled attack context type: {contextType}. Defaulting to random attack.");
                        attackNumber = AnimationData.RandomAttackNumber(weaponType);
                        break;
                }
            }

            duration = AnimationData.AttackDuration(weaponType, attackNumber);
            if (duration <= 0) {
                Debug.LogWarning($"Attack duration is zero or negative for {weaponType}, attack {attackNumber}. Check AnimationData.");
                duration = 1f;
            }

            controller.Attack(attackNumber, duration);

            float timeStampOne = duration * 0.2f; 
            float timeStampTwo = duration * 0.8f; 

            rpgCharacterWeaponController.ToggleWeaponDamageCollider(timeStampOne, timeStampTwo - timeStampOne);
        }

        protected override void _EndAction(RPGCharacterController controller)
        {
            Debug.Log("Attack._EndAction called. active should be set to false by base class.");
            if (controller.TryGetComponent<RPGCharacterWeaponController>(out var rpgCharacterWeaponController))
            {
                rpgCharacterWeaponController.TerminateWeaponDamageColliderCoroutine(); 
            }
        }
    }
}