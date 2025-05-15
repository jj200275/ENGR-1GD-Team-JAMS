using UnityEngine;

public class movementScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D myRigidBody;
    [SerializeField] Collider2D myCollider;
    bool isGrounded;
    [SerializeField] int maxJumps;
    public int jumpsLeft;
    [SerializeField] float gravityMult;
    [SerializeField] float jumpStrength;
    void Start()
    {
        
    }

    void Update()
    {
        myRigidBody.linearVelocityX = 0;
        if (Input.GetKey(KeyCode.A))
        {
            myRigidBody.linearVelocityX -= 5;
        }
        if (Input.GetKey(KeyCode.D))
        {
            myRigidBody.linearVelocityX += 5;
        }
        if (Input.GetKeyDown(KeyCode.W) && jumpsLeft > 0)
        {
            jumpsLeft -= 1;
            myRigidBody.linearVelocityY = jumpStrength;
        }
        if (myRigidBody.linearVelocityY < 0)
        {
            myRigidBody.gravityScale = gravityMult;
        }
        else
        {
            myRigidBody.gravityScale = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        jumpsLeft = maxJumps;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
