
namespace LB{
    public class Knockback : MovementActionHandler<HitContext>
    {
        public Knockback(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        { return controller.CanAction; }

        protected override void _StartAction(RPGCharacterController controller, HitContext context)
        {
            int hitNumber = context.number;
            UnityEngine.Vector3 direction = context.direction;
            float force = context.force;
            float variableForce = context.variableForce;

            if (hitNumber == -1) {
                hitNumber = (int)AnimationVariations.Knockbacks.TakeRandom();
                direction = AnimationData.HitDirection((KnockbackType)hitNumber);
                direction = controller.transform.rotation * direction;
            }
			else {
                if (context.relative) { direction = controller.transform.rotation * direction; }
            }

            controller.Knockback((KnockbackType)hitNumber);
            movement.KnockbackForce(direction, force, variableForce);
            movement.currentState = CharacterState.Knockback;
        }

        public override bool IsActive()
        { return movement.currentState != null && (CharacterState)movement.currentState == CharacterState.Knockback; }
    }
}