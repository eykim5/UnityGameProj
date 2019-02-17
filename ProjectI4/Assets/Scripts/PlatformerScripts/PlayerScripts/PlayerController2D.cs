using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : RaycastController2D {

    public float maxSlopeAngle = 75f;

    [HideInInspector]
    public float crouchFactor = 1f;
    public ColInfo collisions;

    public override void Start() {
        base.Start();
        collisions.horiDirect = 1;
    }

    public void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        collisions.moveAmountOld = moveAmount;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            collisions.horiDirect = (int)Mathf.Sign(moveAmount.x);
        }

        HoriCollisions(ref moveAmount);
        VertCollisions(ref moveAmount);

        transform.Translate(moveAmount);
    }

    void HoriCollisions(ref Vector2 moveAmount)
    {
        float directX = collisions.horiDirect;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horiRayCount * crouchFactor; ++i)
        {
            Vector2 rayOrig = (directX == -1) ? rcOrigins.botLeft : rcOrigins.botRight;
            rayOrig += (Vector2.up * (horiRaySpace * i));

            RaycastHit2D hit = Physics2D.Raycast(rayOrig, Vector2.right * directX, rayLength, colMask);
            Debug.DrawRay(rayOrig, Vector2.right * directX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }

                    float distToSlope = 0;

                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distToSlope = hit.distance - skinWidth;
                        moveAmount.x -= distToSlope * directX;
                    }

                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distToSlope * directX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }
                }

                collisions.left = directX == -1;
                collisions.right = directX == 1;
            }
        }
    }

    void VertCollisions(ref Vector2 moveAmount)
    {
        float directY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < vertRayCount; ++i)
        {
            Vector2 rayOrig = (directY == -1) ? rcOrigins.botLeft : rcOrigins.topLeft;
            rayOrig += Vector2.right * (vertRaySpace * i + moveAmount.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrig, Vector2.up * directY, rayLength, colMask);
            Debug.DrawRay(rayOrig, Vector2.up * directY, Color.red);

            if (hit)
            {
                moveAmount.y = (hit.distance - skinWidth) * directY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.above = directY == 1;
                collisions.below = directY == -1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

            Vector2 rayOrig = ((directX == -1) ? rcOrigins.botLeft : rcOrigins.botRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrig, Vector2.right * directX, rayLength, colMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void ClimbSlope (ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDist = Mathf.Abs(moveAmount.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;

        if (moveAmount.y <= climbVelocityY)
        {
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(moveAmount.x);
            moveAmount.y = climbVelocityY;
            collisions.below = true;    
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(rcOrigins.botLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, colMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(rcOrigins.botRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, colMask);

        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }
        if (!collisions.slideMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? rcOrigins.botRight : rcOrigins.botLeft; 
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, colMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) 
                        {

                            float moveDist = Mathf.Abs(moveAmount.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = hit.normal.x * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slideMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    public struct ColInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope, descendingSlope, slideMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;

        public bool crouching;

        public int horiDirect;

        public Vector2 moveAmountOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;

            crouching = false;

            climbingSlope = false;
            descendingSlope = false;
            slideMaxSlope = false;
            slopeNormal = Vector2.zero;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
