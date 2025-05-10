namespace LB
{
    public class Idle : MovementActionHandler<EmptyContext>
    {
        public Idle(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        {
            if (controller.IsMoving || controller.IsSprinting)
            {
                return controller.MoveInput.magnitude < 0.2f;
            }

            return controller.MaintainingGround || controller.AcquiringGround;
        }

        protected override void _StartAction(RPGCharacterController controller, EmptyContext context)
        {
            movement.currentState = CharacterState.Idle;
        }

        public override bool IsActive()
        {
            return movement.currentState != null && (CharacterState)movement.currentState == CharacterState.Idle;
        }
    }
}