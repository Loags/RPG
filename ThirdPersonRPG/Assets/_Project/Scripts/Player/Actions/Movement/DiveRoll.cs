using UnityEngine;

namespace LB
{
    public class DiveRoll : MovementActionHandler<DiveRollType>
    {
        public DiveRoll(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        {
            Debug.Log("Can Start Dive Roll Action");
            return controller.CanAction;
        }

        protected override void _StartAction(RPGCharacterController controller, DiveRollType rollType)
        {
            controller.DiveRoll(rollType);
            movement.currentState = CharacterState.DiveRoll;
        }

        public override bool IsActive()
        {
            return movement.currentState != null && (CharacterState)movement.currentState == CharacterState.DiveRoll;
        }
    }
}