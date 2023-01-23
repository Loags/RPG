using UnityEngine;

public class SM_CharacterAnimator : MonoBehaviour
{
    const float locomotionAnimationSmoothTime = 0.1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speedPercent = SM_CharacterController.playerInstance.SM_CharacterMovement.currentSpeed / SM_CharacterController.playerInstance.SM_CharacterMovement.maxSpeed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
    }
}
