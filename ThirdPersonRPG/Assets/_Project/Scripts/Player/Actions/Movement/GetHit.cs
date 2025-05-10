// Hit from front1 - 1
// Hit from front2 - 2
// Hit from back - 3
// Hit from left - 4
// Hit from right - 5


using UnityEngine;

namespace LB
{
    public class GetHit : MovementActionHandler<HitContext>
    {
        public GetHit(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        {
            return !controller.IsKnockback && !controller.IsKnockdown;
        }

        protected override void _StartAction(RPGCharacterController controller, HitContext context)
        {
            int hitNumber = context.number;
            Vector3 direction = context.direction;
            float force = context.force;
            float variableForce = context.variableForce;

            if (hitNumber == -1)
            {
                hitNumber = (int)AnimationVariations.Hits.TakeRandom();
                direction = AnimationData.HitDirection((HitType)hitNumber);
                direction = controller.transform.rotation * direction;
            }
            else
            {
                if (context.relative)
                {
                    direction = controller.transform.rotation * direction;
                }
            }

            controller.GetHit(hitNumber);
            movement.KnockbackForce(direction, force, variableForce);
        }
    }
}