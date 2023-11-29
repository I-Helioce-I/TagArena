using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    float maxSpeed = 5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", rb.velocity.magnitude / maxSpeed); 
    }

    public void SetBoolJump(bool isGrounded)
    {
        animator.SetBool("IsGrounded", !isGrounded); 
    }
}
