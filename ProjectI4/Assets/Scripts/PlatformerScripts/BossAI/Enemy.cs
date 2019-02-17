using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    BaseEnemyStats baseStats;
    private EnemyController2D controller;

    [HideInInspector]
    public float HP;
    [HideInInspector]
    public float ATK;
    [HideInInspector]
    public float SPEED;

    public bool canJump;
    private int jumpCounter;
    private float jumpTimer;
    private bool jumpDown;

    // Variables for jump physics.
    public const float maxJumpHeight = 8f;
    public const float minJumpHeight = 1f;
    public const float jumpTimeApex = .6f;

    // Variables for air and ground acceleration.
    public const float accelTimeAir = .1f;
    public const float accelTimeGround = -.3f;

    // Variables for general movement.
    private float gravity;
    private float origGrav;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector2 velocity;
    private float velocityXSmoothing;

    // Variables for speed and hitboxes.
    private float moveSpeed;

    public bool faceRight = true;

    void Start()
    {
        baseStats = GetComponent<BaseEnemyStats>();
        HP = baseStats.HP.Value;
        ATK = baseStats.ATK.Value;
        SPEED = baseStats.SPEED.Value;

        //moveSpeed = SPEED;
        controller = GetComponent<EnemyController2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(jumpTimeApex, 2);
        origGrav = gravity;
        maxJumpVelocity = Mathf.Abs(gravity) * jumpTimeApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        canJump = true;
        jumpTimer = 1.3f;
        jumpDown = false;
    }

    void Update()
    {
        if (HP <= 0)
        {
            Death();
        }
    }

    void FixedUpdate()
    {
        CalcVelocity();

        controller.Move(velocity * Time.deltaTime);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slideMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    void CalcVelocity()
    {
        float targetVelocityX = moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGround : accelTimeAir);
        velocity.y += gravity * Time.deltaTime;
    }

    public void Flip()
    {
        faceRight = !faceRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ResetJumpCounter()
    {
        if (jumpCounter == -100) { 
            jumpCounter = 3;
        }
        canJump = true;
        jumpTimer = 1.3f;
    }

    public void DecreaseJump()
    {
        jumpCounter--;
        ResetJumpCounter();
    }

    public void Jump(Transform playerPos)
    {
        if (jumpTimer < 0 && !controller.collisions.below)
        {
            velocity = new Vector2(0f, -75f);
            gravity = origGrav;
            jumpDown = false;
        }
        else if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (controller.collisions.below)    // Initial jump. If jumpCount is 0, then the enemy will not do another jump.
        {
            if (jumpCounter > 0)
            {
                velocity.y = maxJumpVelocity;
            }
            else
            {
                moveSpeed = 0f;
            }
            Invoke("DecreaseJump", 1.0f);
            canJump = false;
        }

        if (velocity.y < 0f && jumpTimer > 0)    //  Lets the enemy stay in the air until they slam down.
        {
            velocity.y = 0f;
            gravity = 0f;
        }
        
        if (!controller.collisions.below)   //Directions in the air.
        {
            if (Mathf.Abs(playerPos.position.x - transform.position.x) >= 2f && !jumpDown)
            {
                moveSpeed = 15f * transform.localScale.x;
            }
            else
            {
                moveSpeed = 0f;
                jumpDown = true;
            }
        }
    }

    void Death()
    {
        Debug.Log(transform.name + " is DEAD.");
        Destroy(gameObject);
    }
}
