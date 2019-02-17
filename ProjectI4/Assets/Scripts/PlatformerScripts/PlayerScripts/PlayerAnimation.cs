using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Player player;

    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player.directInput.x != 0f)
        {
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }

        if (Input.GetButtonDown("Fire1") || player.grabbing)
        {
            animator.SetBool("grabbing", true);
        }
        else
        {
            animator.SetBool("grabbing", false);
        }
        animator.SetBool("inAir", player.inAir);
        animator.SetBool("crouching", player.crouching);
    }
}
