using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    // Inputfields
    PlayerControls playerControls;
    InputAction move;

    // Movement fieldsCo
    Rigidbody rb;
    [SerializeField]
    float movementForce = 1f;
    [SerializeField]
    float jumpedForce = 5f;
    [SerializeField]
    float maxSpeed = 5f;
    Vector3 forceDirection = Vector3.zero;

    public bool isjumping;

    Animator animator;

    [SerializeField]
    Camera playerCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerControls = new PlayerControls();
        animator = GetComponent<Animator>();

        CursorIsVisible(false);
    }
    private void OnEnable()
    {
        playerControls.Player.Jump.started += DoJump;
        move = playerControls.Player.Move;
        playerControls.Player.Enable();

    }
    private void OnDisable()
    {
        playerControls.Player.Jump.started -= DoJump;
        playerControls.Player.Disable();
    }

    private void FixedUpdate()
    {

        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0.1f)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
            
        }

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0f;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsJumping", false);
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        

        LookAt();
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            isjumping = true;
            animator.SetBool("IsGrounded", false);
            animator.SetBool("IsJumping", true);
            forceDirection += Vector3.up * jumpedForce;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);


        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            return true;
        else
            return false;


    }

    private void CursorIsVisible(bool visible)
    {
        Cursor.visible = visible;

        if (!visible)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

    }

}
