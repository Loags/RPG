using UnityEngine;

namespace LB
{
    public class Sprint : MovementActionHandler<EmptyContext>
    {
        public Sprint(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        {
            return controller.CanMove
                   && controller.CanSprint
                   && controller.rpgCharacterInputSystemController.HasSprintInput()
                   && controller.MoveInput.sqrMagnitude > 0.1f
                   && controller.MaintainingGround;
        }

        protected override void _StartAction(RPGCharacterController controller, EmptyContext context)
        {
            Debug.Log("Start Sprint Action");
            movement.currentState = CharacterState.Sprint;
        }

        public override bool IsActive()
        {
            return movement.currentState != null && (CharacterState)movement.currentState == CharacterState.Sprint;
        }
    }
}