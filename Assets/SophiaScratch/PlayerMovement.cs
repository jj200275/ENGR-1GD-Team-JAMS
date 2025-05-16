using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerInput : MonoBehaviour
{
    // Initializers
    private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 8f;
    public float fallMultiplier = 1.5f;
    private float movement = 0f;

<<<<<<< Updated upstream
=======
    // Switching
    [SerializeField] GameObject presentDimension;
    [SerializeField] GameObject pastDimension;
    public bool present = true;
    public float timer;
    public float cooldown;

    // Audio
    public AudioSource audioSteps;  // both dim
    public AudioSource audioEerie;  // present dim - eerie
    public AudioSource audioBirds;  // past dimension "music" - birds
    private bool isMoving;

    // Jumping
    private bool isGrounded = false;
    private bool allowDoubleJump = true;  // can change through triggers for certain areas on map - or when change scenes, etc so it remains unique to level
    private int numJumps = 0;
    private bool jumpRelease = false;
    private bool canJump;
    
    // Dashing
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTimer;

    // Double-tap Dash controls
    private float tapLeft = -1f;
    private float tapRight = -1f;
    private float doubleTap = 0.25f;
    
    // Animator
    //[SerializeField] private Animator animator;
    private Animator animator;
    private bool facingRight = true; 

    
//--------------------------------------------- Start ----------------------------------------------\\

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
//--------------------------------------------- Updates ----------------------------------------------\\

    void Update()
    {
        Move(movement);
        animator.SetFloat("Speed", Mathf.Abs(speed * movement));
        
        // Dashing
        CheckDoubleTap();

        // Audio - Player Footsteps
        if (!Mathf.Approximately(rb.linearVelocity.x, 0)) {isMoving = true;} 
        else {isMoving = false;} 

        if (isMoving && isGrounded)
        {
            if (!audioSteps.isPlaying) {audioSteps.Play();}
        }
        else {audioSteps.Stop();}

    }

    private void FixedUpdate()
    {
        // Falling/Jumping
        if (!Mathf.Approximately(rb.linearVelocity.y, 0))  // when y-velocity NOT = 0 | note this includes when the player falls - not just when jump
        {
            rb.linearVelocity += Vector2.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime ;  // makes player fall faster (e.g. after jump)
        }

        // Double Jump
        if (allowDoubleJump)
        {
            if (isGrounded && Mathf.Approximately(rb.linearVelocity.y, 0))  // if player is on "Ground" AND has NO y-velocity (not touching side walls)
            {
                numJumps = 0;       // reset the number of jumps
            }
            
            if (canJump)
            {
                Jump();
                canJump = false;
            }
            
            if (jumpRelease && rb.linearVelocity.y > 0f)        // jump height if the button is released during the jump
            {
                jumpRelease = false;
            }
        }

        // Dashing
        if (isDashing)
        {
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false;
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

            rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);
        }
    }
    
//--------------------------------------------- Move ----------------------------------------------\\

    private void Move(float x)
    {   
        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);
    }

    void OnMove(InputValue value)
    {
        movement = value.Get<float>();

        // flips player animation
        if (movement < 0 && facingRight == true)
        {
            Flip();
        }

        if (movement > 0 && facingRight == false)
        {
            Flip();
        }  
    }

    void Flip()   // Flip function for animation when Move
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

//--------------------------------------------- Audio ----------------------------------------------\\


//--------------------------------------------- Switch ----------------------------------------------\\

 void switchDimension(bool current)
    {
        if (current)
        {
            presentDimension.SetActive(false);
            pastDimension.SetActive(true);
            audioEerie.Stop();  // stop present dim audio
            audioBirds.Play();  // play audio for past dim
            present = false;
        }
        else
        {
            presentDimension.SetActive(true);
            pastDimension.SetActive(false);
            audioBirds.Stop();  // stop past dim audio
            audioEerie.Play();  // play audio for present dim
            present = true;
        }
    }

//--------------------------------------------- Jump ----------------------------------------------\\

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }

    void OnJump(InputValue value)  // Note: to enable double jump, make sure allowDoubleJump is set to true!
    {   
        if (allowDoubleJump)  // if double jump is enabled
        {
            if (value.isPressed && numJumps < 1)
            {
                canJump = true;          // initialize the number of jumps to 0, and check if numJumps < 1. if so, it allows one more jump (a double jump)
                numJumps++;             // increment the number of jumps
            }
            
            else if (!value.isPressed)
            {
                jumpRelease = true;     // jump button is released
            }
        }

        else  // Normal Jump
        {
            if (value.isPressed && isGrounded && Mathf.Approximately(rb.linearVelocity.y, 0))  // if player is on "Ground" AND has NO y-velocity (not touching side walls)
            {
                Jump();
            }
        }
    }


//--------------------------------------------- Dash ----------------------------------------------\\

    private void CheckDoubleTap()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            //Debug.Log("A key pressed");
            if (Time.time - tapLeft < doubleTap && canDash)
            {
                Dash(-1);
            }
            tapLeft = Time.time;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            //Debug.Log("D key pressed");
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
        Debug.Log("Dash started! Direction: " + direction);
    }


//--------------------------------------------- Collisions ----------------------------------------------\\

    // Ground Check 
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

/* NOTE: may be issue with switch dimensions - not include for now
    void OnCollisionStay2D(Collision2D other)  // while IN collider - to make sure don't get weird bug for jump
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
*/

    void OnCollisionExit2D(Collision2D other)
    {
        isGrounded = false;
    }
    
}