using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 50f; 
    public float deacceleration = 50f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.8f;

    private Rigidbody2D rb;
    private PlayerStats playerStats;

    public Vector2 moveInput;
    private Vector2 lastMoveDirection;
    private Vector2 dashDirection;

    public bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>(); 

        if (playerStats == null || playerStats.characterData == null)
        {
            return;
        }
    }

    void Update()
    {
        if (isDashing) return;

        var keyboard = Keyboard.current;

        moveInput = Vector2.zero;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y = 1;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y = -1;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x = -1;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x = 1;

        if (moveInput != Vector2.zero)
        {
            moveInput.Normalize();
            lastMoveDirection = moveInput;
        }

        // Dash Cooldown
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        if (keyboard.spaceKey.wasPressedThisFrame && dashCooldownTimer <= 0)
        {
            StartDash();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            ApplyDash();
            return;
        }

        ApplyMovement();
    }

    void ApplyMovement()
    {
        float currentMoveSpeed = playerStats.CurrentMoveSpeed;
        float targetSpeedX = moveInput.x * currentMoveSpeed;
        float targetSpeedY = moveInput.y * currentMoveSpeed;

        float accelX = (Mathf.Abs(targetSpeedX) > 0.01f) ? acceleration : deacceleration;
        float accelY = (Mathf.Abs(targetSpeedY) > 0.01f) ? acceleration : deacceleration;

        float moveX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeedX, accelX * Time.fixedDeltaTime);
        float moveY = Mathf.MoveTowards(rb.linearVelocity.y, targetSpeedY, accelY * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(moveX, moveY);
    }

    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        float currentDashSpeed = playerStats.CurrentMoveSpeed * 3f;

        dashDirection = moveInput != Vector2.zero ? moveInput : lastMoveDirection;
        rb.linearVelocity = dashDirection * currentDashSpeed;
    }

    void ApplyDash()
    {
        if (dashTimeLeft > 0)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            dashTimeLeft -= Time.fixedDeltaTime;
        }
        else
        {
            isDashing = false;
            rb.linearVelocity = Vector2.zero; // Stop after dashing
        }
    }
}