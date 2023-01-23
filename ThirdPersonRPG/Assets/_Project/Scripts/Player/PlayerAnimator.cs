using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    const float locomotionAnimationSmoothTime = 0.1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float speedPercent = PlayerController.instance.PlayerMovement.currentSpeed / PlayerController.instance.PlayerMovement.maxSpeed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);
    }
}
