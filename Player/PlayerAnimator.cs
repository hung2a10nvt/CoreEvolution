using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController moveScript;

    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (moveScript == null) moveScript = GetComponent<PlayerController>();
    }

    void Update()
    {
        UpdateAnimationParameters();
        HandleFacingDirection();
    }

    private void UpdateAnimationParameters()
    {
        anim.SetFloat(MoveX, moveScript.moveInput.x);
        anim.SetFloat(MoveY, moveScript.moveInput.y);

        // Speed for Idle -> Move
        anim.SetFloat(Speed, moveScript.moveInput.sqrMagnitude);

        anim.SetBool(IsDashing, moveScript.isDashing);
    }

    private void HandleFacingDirection()
    {
        if (moveScript.moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveScript.moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}