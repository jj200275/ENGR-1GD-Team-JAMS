using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    // Initializers
    private Rigidbody2D rb;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float jumpHeight = 8f;
    private float movement = 0f;
    private bool canMove = true;  // turn off to disable player mvmt during end flower animation
    public float fallMultiplier = 1.5f;

    // Restart Level
    private float restartTimer = 0;
    private float restartCooldown = 3f;

    // Switching
    [SerializeField] GameObject presentDimension;
    [SerializeField] GameObject pastDimension;
    private bool allowDimSwitch = false;  // can change through triggers for certain areas on map - or when change scenes, etc so it remains unique to level
    private float timer;
    private float cooldown;
    private float switchTime;
    public bool present = true;

    // Audio
    public AudioSource audioSteps;  // both dim
    public AudioSource audioEerie;  // present dim - eerie
    public AudioSource audioBirds;  // past dimension "music" - birds
    private bool isMoving;

    // Jumping
    private bool isGrounded = false;
    private bool allowDoubleJump = false;  // can change through triggers for certain areas on map - or when change scenes, etc so it remains unique to level
    private int numJumps = 0;
    private bool jumpRelease = false;
    private bool canJump;

    // Dashing
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    private bool allowDash = false;    // can change through triggers for certain areas on map - or when change scenes, etc so it remains unique to level
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTimer;

    // Double-tap Dash controls
    private float tapLeft = -1f;
    private float tapRight = -1f;
    private float doubleTap = 0.25f;

    // Animator
    [SerializeField] CanvasScript CanvasScript;
    private Animator animator;
    private bool facingRight = true;

    // Level Switching
    private int index; // Level index in Build 
    private string levelName;  // Level/scene name 

    // Flower
    public Animator flowerAnimatorControl;


    //--------------------------------------------- Start ----------------------------------------------\\

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Switch
        timer = 0;
        cooldown = 0.3f;
        switchTime = 2.7f;

        // Initialize index - gets current level (when loads new scene)
        index = SceneManager.GetActiveScene().buildIndex;
        levelName = SceneManager.GetActiveScene().name;

        // Special Mvmts for Levels   -- >= in case of level bugs during gameplay        
        if (index >= 1) // enable dim. switch  - at level 2 (index is 1)
        {
            allowDimSwitch = true;
        }

        if (index >= 3) // enable double jump  - level 4 (index is 3)
        {
            allowDoubleJump = true;
        }

        if (index >= 4) // enable dash - level 5 (index is 4)
        {
            allowDash = true;
        }

        if (levelName == "Level End")
        {
            allowDimSwitch = false;
            // initializePastDim(index);    // DELETE - Only include if doing timer dim. switch on this level
        }

    }

    //--------------------------------------------- Updates ----------------------------------------------\\

    void Update()
    {
        // Restart Level
        if (restartTimer <= restartCooldown) { restartTimer += Time.deltaTime; }
        if (Input.GetKeyDown(KeyCode.R) && restartTimer >= restartCooldown)
        {
            // Reset all static instruction flags
            InstructionsScript.ADhasFadedOut = false;
            InstructionsScript.RhasFadedOut = false;
            InstructionsScript.ShasFadedOut = false;
            InstructionsScript.DubSpacehasFadedOut = false;
            InstructionsScript.DubDhasFadedOut = false;

            InstructionsScript.ADwaitForExit = false;
            InstructionsScript.RwaitForExit = false;
            InstructionsScript.SwaitForExit = false;
            InstructionsScript.DubSpacewaitForExit = false;
            InstructionsScript.DubDwaitForExit = false;

            SceneManager.LoadScene(index);  // reload current scene - note that it also reloads script
        }

        // Switch
        if (timer >= 0) { timer -= Time.deltaTime; }
        if (allowDimSwitch && Input.GetKeyDown(KeyCode.S) && timer <= 0)
        {
            switchDimension(present);
            timer = cooldown;
        }

        /*  DELETE - Only include if want timed dim. switch
        if (levelName == "Level End" && timer <= 0)
        {
            switchDimension(present);
            timer = switchTime;
        }
        */

        // Dashing
        CheckDoubleTap();

        // Audio - Player Footsteps
        if (!Mathf.Approximately(rb.linearVelocity.x, 0)) { isMoving = true; }
        else { isMoving = false; }

        if (isMoving && isGrounded)
        {
            if (!audioSteps.isPlaying) { audioSteps.Play(); }
        }
        else { audioSteps.Stop(); }

        // Animations
        animator.SetBool("Running", isMoving);
        animator.SetBool("Present", present);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            // Fast Fall - for falling/jumping
            if (!Mathf.Approximately(rb.linearVelocity.y, 0))  // when y-velocity NOT = 0 | note this includes when the player falls - not just when jump
            {
                rb.linearVelocity += Vector2.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;  // makes player fall faster (e.g. after jump)
            }

            // Jump
            if (canJump)
            {
                Jump();
                canJump = false;
            }

            if (jumpRelease && rb.linearVelocity.y > 0f)        // jump height if the button is released during the jump
            {
                jumpRelease = false;
            }

            if (isGrounded && Mathf.Approximately(rb.linearVelocity.y, 0))  // if player is on "Ground" AND has NO y-velocity (not touching side walls)
            {
                numJumps = 0;       // reset the number of jumps
            }

            // Dashing
            if (allowDash)
            {
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

                    Move(movement); // Normal move - is now here (used to be in Update()) | this is necessary for dash to work
                }
            }

            // Move (No dash)
            if (!allowDash)
            {
                Move(movement); // Normal move 
            }
        }

        else { Move(0); }  // if canMove = false, stop player mvmt
    }

    //--------------------------------------------- Move ----------------------------------------------\\

    private void Move(float x)
    {
        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);
    }

    void OnMove(InputValue value)
    {
        if (canMove)
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
    }

    void Flip()   // Flip function for animation when Move
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }


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
            CanvasScript.present = false;
        }
        else
        {
            presentDimension.SetActive(true);
            pastDimension.SetActive(false);
            audioBirds.Stop();  // stop past dim audio
            audioEerie.Play();  // play audio for present dim
            present = true;
            CanvasScript.present = true;
        }
    }

    //--------------------------------------------- Jump ----------------------------------------------\\

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }

    void OnJump(InputValue value)  // NOTE: to enable double jump, make sure allowDoubleJump is set to true!
    {
        // Double Jump
        if (allowDoubleJump)
        {
            if (value.isPressed && numJumps < 2)  // allows 2 jumps
            {
                canJump = true;
                numJumps++;
            }

            else if (!value.isPressed)
            {
                jumpRelease = true;     // jump button is released
            }
        }

        // Normal Jump
        else
        {
            if (value.isPressed && numJumps < 1)  // only allow 1 jump
            {
                canJump = true;
                numJumps++;
            }
        }
    }


    //--------------------------------------------- Dash ----------------------------------------------\\

    private void CheckDoubleTap()
    {
        if (canMove){
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                if (Time.time - tapLeft < doubleTap && canDash)
                {
                    Dash(-1);
                }
                tapLeft = Time.time;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                if (Time.time - tapRight < doubleTap && canDash)
                {
                    Dash(1);
                }
                tapRight = Time.time;
            }
        }
    }

    private void Dash(int direction)
    {
        isDashing = true;
        canDash = false;
        dashTime = dashDuration;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);
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

    void OnCollisionExit2D(Collision2D other)
    {
        isGrounded = false;
    }


    //--------------------------------------------- Triggers ----------------------------------------------\\
    void OnTriggerEnter2D(Collider2D other)
    {
        // Level Loader
        if (other.gameObject.CompareTag("LevelExit"))
        {
            SceneManager.LoadScene(index + 1);  // loads the next scene in Build Profile
        }

        // Flower Trigger
        if (other.gameObject.CompareTag("FlowerTrigger"))
        {
            switchDimension(present);
            canMove = false;  // stops player from being able to move
            flowerAnimatorControl.SetBool("isBloom", true);
            StartCoroutine("Bloom");    
        }
    }

            //----------------------- Past Initialization -------------------\\
            // Initialization for proper dimension - if level starts in past, set present to false
            // Call this function in Start() if needed
                void initializePastDim(int index)  // Note: level = index + 1
                {
                    present = false;

                    audioEerie.playOnAwake = false;
                    audioEerie.Stop();

                    audioBirds.playOnAwake = true;
                    audioBirds.Play();
                }

    //--------------------------------------------- Flower Trigger ----------------------------------------------\\

    IEnumerator Bloom()
    {
        yield return new WaitForSeconds(flowerAnimatorControl.GetCurrentAnimatorStateInfo(0).length + 3);
        GameObject endscript = GameObject.Find("end scripting");
        endscript.GetComponent<EndScript>().startEnd = true;
        // SceneManager.LoadScene("Level 1");  // CHANGE TO END SCENE ONCE GET! - Loads end scene
    }

}