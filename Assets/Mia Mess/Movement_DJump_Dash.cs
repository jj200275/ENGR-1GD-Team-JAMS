using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement_DJump_Dash : MonoBehaviour
{

    private float horizontal;
    private float speed = 8f;
    private Vector2 movement = Vector2.zero;
    private float jumpingPower = 12f;
    private bool isJump;
    private bool jumpRelease;
    private bool isRight = true;
    private int numJumps;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // dashing

    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTimer;

    // double-tap dash controls

    private float tapLeft = -1f;
    private float tapRight = -1f;
    private float doubleTap = 0.25f;

    // animations

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponentInChildren<Animator>();
    }

    void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    void OnJump(InputValue value) 
    {
        if (value.isPressed && numJumps < 1)
        {
            isJump = true;          // initialize the number of jumps to 0, and check if numJumps < 1. if so, it allows one more jump (a double jump)
            numJumps++;             // increment the number of jumps
        }
        else if (!value.isPressed)
        {
            jumpRelease = true;     // jump button is released
        }
    }

    void Update()
    {
        // Move(movement.x, movement.y);
        horizontal = movement.x;
        Flip();

        // Dashing
        CheckDoubleTap();
    }

    private void FixedUpdate() 
    {

        // dashing

        if (isDashing)
        {
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false;
                // rb.gravityScale = 1f;
                dashCooldownTimer = dashCooldown;
            }
        }
        else
        {
            dashCooldownTimer -= Time.fixedDeltaTime;
            if (dashCooldownTimer <= 0f)
            {
                canDash = true;
            }

            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }

        if (IsGrounded())
        {
            numJumps = 0;       // if the player hits the ground, reset the number of jumps
        }
        if (isJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);     // jump height becomes the x value and the jump power
            isJump = false;
        }
        if (jumpRelease && rb.linearVelocity.y > 0f)        // jump height if the button is released during the jump
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            jumpRelease = false;
        }

        animator.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("isJumping", !IsGrounded());
    }

    private bool IsGrounded() 
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isRight && horizontal < 0f || !isRight && horizontal > 0f) 
        {
            isRight = !isRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void CheckDoubleTap()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            Debug.Log("A key pressed");
            if (Time.time - tapLeft < doubleTap && canDash)
            {
                Dash(-1);
            }
            tapLeft = Time.time;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Debug.Log("D key pressed");
            if (Time.time - tapRight < doubleTap && canDash)
            {
                Dash(1);
            }
            tapRight = Time.time;
        }
    }

    private void Dash(int direction)
    {
        isDashing = true;
        canDash = false;
        dashTime = dashDuration;

        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);
        // rb.gravityScale = 0f;

        Debug.Log("Dash started! Direction: " + direction);
    }
}
