using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController2D))]
public class Player : MonoBehaviour {
    // This class is where all the active processes occur related to the player.

    // CharStats to get baseVals for stats.
    BasePlayerStats baseStats;

    // Current, ingame variables.
    [HideInInspector]
    public float HP;
    [HideInInspector]
    public float SPEED;

    // Variables for jump physics.
    public const float maxJumpHeight = 4f;
    public const float minJumpHeight = 1f;
    public const float jumpTimeApex = .4f;

    // Variables for air and ground acceleration.
    public const float accelTimeAir = .2f;
    public const float accelTimeGround = .1f;

    // Variables for general movement.
    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private Vector2 velocity;
    private float velocityXSmoothing;

    // Variables for speed and hitboxes.
    private float moveSpeed;
    private float standSpeed;
    private float crouchSpeed;
    private float standSizeY;
    private float standOffsetY;
    private float crouchSizeY;
    private float crouchOffsetY;

    public float grabDist = 1.5f;
    public float holdDist = 2f;
    public float throwForce = 3f;

    // Variables for relevant foreign objects.
    private PlayerController2D controller;
    public GameObject hCObj;
    private HeadCheck headCheck;

    public Vector2 directInput;

    private bool faceRight = true;
    public bool crouching = false;
    public bool grabbing;
    public bool inAir;
    private bool dropping = false;
    private bool isStill;


    private RaycastHit2D hit;

    public Transform grabPoint;

	void Start () {
        baseStats = GetComponent<BasePlayerStats>();

        // Initializes stats to baseVals.
        HP = baseStats.HP.Value;
        SPEED = baseStats.SPEED.Value;
        standSpeed = SPEED;
        crouchSpeed = SPEED / 3.2f;


        // Initializes some variables, mostly for foreign hitboxes.
        controller = GetComponent<PlayerController2D>();
        headCheck = hCObj.GetComponent<HeadCheck>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(jumpTimeApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * jumpTimeApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        standSizeY = controller.boxCol2D.size.y;
        standOffsetY = controller.boxCol2D.offset.y;
        crouchSizeY = controller.boxCol2D.size.y / 2;
        crouchOffsetY = -.5f;
        moveSpeed = standSpeed;
        grabbing = false;
	}

    void Update()
    {
        // Anything related to INPUT goes HERE!
        if (Input.GetButton("Fire3") && controller.collisions.below)
        {
            velocity.x = 0;
            isStill = true;
        }
        else {
            isStill = false;
        }

        directInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpDown();
        }
        if (Input.GetButtonUp("Jump"))
        {
            OnJumpUp();
        }

        if (Input.GetButtonDown("Fire1")) {
            Grab();
        }

        if (Input.GetButtonDown("Fire2") && grabbing)
        {
            dropping = true;
            Grab();
        }


        if (HP <= 0)
        {
            Death();
        }
    }

    void FixedUpdate () {
        // Anything related to MOVEMENT/PHYSICS goes HERE!

        CalcVelocity();
        Crouch();
        
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

        if (directInput.x > 0 && !faceRight)
        {
            Flip();
        }
        else if (directInput.x < 0 && faceRight)
        {
            Flip();
        }

        if (grabbing)
        {
            Hold();
        }

        inAir = !controller.collisions.below;   // This is here for animation purposes.

	}

    public void OnJumpDown()
    {
        if (controller.collisions.below && !crouching)
        {
            if (controller.collisions.slideMaxSlope)
            {
                if (directInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void Grab()
    {
        Debug.DrawRay(transform.position, Vector2.right * transform.localScale.x, Color.green);
        if (!grabbing)
        {
            Physics2D.queriesStartInColliders = false;
            hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, grabDist);

            if (hit.collider != null && hit.collider.tag == "Grabbable")
            {
                grabbing = true;
                if (hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
                    hit.collider.gameObject.layer = 12;
                }
            }
        }
        else
        {
            // This is where throw potentially goes.
            grabbing = false;
            Rigidbody2D objRigBody2D = hit.collider.gameObject.GetComponent<Rigidbody2D>();

            if (objRigBody2D != null)
            {
                objRigBody2D.gravityScale = 1.5f;
                hit.collider.gameObject.layer = 10;

                Vector2 objVelo;
                float grabAngle = Mathf.Atan2(directInput.y, directInput.x) * Mathf.Rad2Deg;
                float objVeloX = Mathf.Cos(grabAngle * Mathf.Deg2Rad) * holdDist;
                float objVeloY = Mathf.Sin(grabAngle * Mathf.Deg2Rad) * holdDist;

                //Clean these up later.
                if (dropping)
                {
                    objVelo = new Vector2(0f, -9.81f);
                    dropping = false;
                }
                else
                {
                    if (directInput == Vector2.zero || directInput.y < 0)
                    {
                        if (faceRight)
                        {
                            objVelo = new Vector2(throwForce, 0f);
                        }
                        else
                        {
                            objVelo = new Vector2(-throwForce, 0f);
                        }
                    }
                    else
                    {
                        objVelo = new Vector2(objVeloX * throwForce, objVeloY * throwForce); // Determines direction and velocity of object thrown. Current equations are placeholders.
                    }
                }

                objRigBody2D.AddForce(objVelo);
                //objRigBody2D.velocity = objVelo;
            }
        }
    }

    public void Hold()
    {
        GameObject grabObj = hit.collider.gameObject;

        if (directInput != Vector2.zero && !crouching)
        {
            float grabAngle = Mathf.Atan2(directInput.y, directInput.x) * Mathf.Rad2Deg;

            float posX, posY;

            if (grabAngle == 0)
            {
                posX = holdDist;
                posY = 0f;
            }
            else if (grabAngle == 90)
            {
                posX = 0f;
                posY = holdDist;
            }
            else if (grabAngle == 180)
            {
                posX = -holdDist;
                posY = 0f;
            }
            else
            {
                posX = Mathf.Cos(grabAngle * Mathf.Deg2Rad) * holdDist;
                posY = Mathf.Sin(grabAngle * Mathf.Deg2Rad) * holdDist;
                if (posY < 0)
                {
                    if (directInput.x != 0)
                    {
                        posX = holdDist * Mathf.Sign(directInput.x);
                    }
                    else
                    {
                        posX = holdDist * Mathf.Sign(transform.localScale.x);
                    }
                    posY = 0f;
                }
            }

            if (Mathf.Sign(grabAngle) >= 0)
            {
                grabObj.transform.position = new Vector2(transform.position.x + posX, transform.position.y + posY + .5f);   // Determines position of object when aiming. (note the .5f compensates for the player's hitbox).
                grabObj.transform.eulerAngles = new Vector3(grabObj.transform.eulerAngles.x, grabObj.transform.eulerAngles.y, grabAngle);   // Rotates object as player aims it.
            }
            else
            {
                float holdAngle;
                if (grabAngle >= -90)
                {
                    holdAngle = 0f;
                }
                else
                {
                    holdAngle = 180f;
                }
                grabObj.transform.position = new Vector2(transform.position.x + posX, transform.position.y + posY - .5f);
                grabObj.transform.eulerAngles = new Vector3(grabObj.transform.eulerAngles.x, grabObj.transform.eulerAngles.y, holdAngle);
            }
        }
        else
        {
            if (crouching)
            {
                grabObj.transform.position = new Vector2(grabPoint.position.x, grabPoint.position.y - 1f);
            }
            else
            {
                grabObj.transform.position = grabPoint.position;    // Default positional and rotational values if player is not aiming.
            }
            grabObj.transform.eulerAngles = new Vector3(grabObj.transform.eulerAngles.x, grabObj.transform.eulerAngles.y, 0f);
        }
    }

    void CalcVelocity()
    {
        float targetVelocityX;

        if (!isStill) {
            targetVelocityX = directInput.x * moveSpeed;
        }
        else
        {
            targetVelocityX = 0;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGround : accelTimeAir);
        velocity.y += gravity * Time.deltaTime;
    }

    void Crouch()
    {
        if ((controller.collisions.below && directInput.y < -.3f) || headCheck.getTrigger())
        {
            crouching = true;
            hCObj.SetActive(true);
            controller.boxCol2D.size = new Vector2(controller.boxCol2D.size.x, crouchSizeY);
            controller.boxCol2D.offset = new Vector2(controller.boxCol2D.offset.x, crouchOffsetY);
            controller.crouchFactor = 0.5f;
            moveSpeed = crouchSpeed;
        }
        else
        {
            crouching = false;
            hCObj.SetActive(false);
            controller.boxCol2D.offset = new Vector2(controller.boxCol2D.offset.x, standOffsetY);
            controller.boxCol2D.size = new Vector2(controller.boxCol2D.size.x, standSizeY);
            controller.crouchFactor = 1f;
            moveSpeed = standSpeed;
        }

    }

    void Flip()
    {
        faceRight = !faceRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Death()
    {
        Debug.Log("Player is DEAD.");
    }
}
