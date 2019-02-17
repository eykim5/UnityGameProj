using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : BaseBehavior
{
    public float timer;
    public float minTime;
    public float maxTime;

    private Transform playerPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        playerPos = GameObject.Find("Player").GetComponent<Transform>();
        timer = Random.Range(minTime, maxTime);

        Debug.Log("Enemy is Idle.");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timer <= 0)
        {
            float rand = Random.Range(0, 2);

            /*if (rand == 0)
            {
                animator.SetTrigger("dash");
            }
            else
            {
                animator.SetTrigger("jump");
            }*/
            animator.SetTrigger("jump");

        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (playerPos.position.x < animator.transform.position.x )
        {
            if (enemy.faceRight)
            {
                enemy.Flip();
            }
        }
        else if (playerPos.position.x > animator.transform.position.x)
        {
            if (!enemy.faceRight)
            {
                enemy.Flip();
            }
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //      
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
