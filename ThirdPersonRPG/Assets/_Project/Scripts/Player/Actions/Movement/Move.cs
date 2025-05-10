using UnityEngine;

namespace LB
{
    public class Move : MovementActionHandler<EmptyContext>
    {
        public Move(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        {
            return controller.CanMove
                   && !controller.rpgCharacterInputSystemController.HasSprintInput()
                   && controller.MoveInput.sqrMagnitude > 0.1f
                   && controller.MaintainingGround;
        }

        protected override void _StartAction(RPGCharacterController controller, EmptyContext context)
        {
            Debug.Log("Start Move Action");
            movement.currentState = CharacterState.Move;
        }

        public override bool IsActive()
        {
            return movement.currentState != null && (CharacterState)movement.currentState == CharacterState.Move;
        }
    }
}