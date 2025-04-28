using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //general
    private Rigidbody _rb;
    private Vector2 _movementInput;
    
    //stats
    public float health;
    public float maxHealth;
    
    //movement
    private bool isSprinting = false;
    public float speed = 5f;
    public float sprintMultiplier = 3f;
    
    //dash
    private bool canDash = true;
    public float dashForce = 50f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 1f;
    public GameObject dashBar;
    private Vector3 _transformLocalScale;
    
    //jump
    private bool canJump = true;
    public float jumpForce = 5f;
    
    //ground check
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        maxHealth = 100f;
        health = maxHealth;
    }

    // This is the Movement message called by PlayerInput
    public void OnMovement(InputAction.CallbackContext context)
    {
        Debug.Log("Move");
        _movementInput = context.ReadValue<Vector2>();
    }

    // This is the Sprint message called by PlayerInput
    public void OnSprint(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint");
        isSprinting = context.performed;
    }

    // This is the Jump message called by PlayerInput
    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (canJump && context.performed)
        {
            Debug.Log("Jump");
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }

    // This is the Dash message called by PlayerInput
    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash && context.performed)
        {
            canDash = false;
            Vector3 dashDirection = new Vector3(_movementInput.x, 0.0f, _movementInput.y).normalized;
            StartCoroutine(DashCoroutine(dashDirection));
            lerpDashCooldown();
        }
    }

    //This is the dash coroutine that controls the main execution of the dash feature
    private IEnumerator DashCoroutine(Vector3 dashDirection)
    {
        Debug.Log("Dash");
        _rb.useGravity = false;  // Disable gravity
        _rb.velocity = dashDirection * dashForce;  // Set the velocity for dash
        yield return new WaitForSeconds(dashDuration);  // Wait for the dash duration
        _rb.velocity = Vector3.zero;  // Stop the dash
        _rb.useGravity = true;  // Re-enable gravity
        yield return new WaitForSeconds(dashCooldown - dashDuration);  // Wait for the remainder of the cooldown duration
        canDash = true;  // Re-enable dashing
    }

    public void lerpDashCooldown()
    {
        if (!canDash)
        {
            StartCoroutine(LerpDashBar());
        }
    }

    private IEnumerator LerpDashBar()
    {
        float elapsedTime = 0f;
        float duration = dashCooldown - dashDuration;
        //set x scale to zero
        Vector3 startingScale = new Vector3(0, dashBar.transform.localScale.y, dashBar.transform.localScale.z);
        //lerp back to original scale x value
        Vector3 endingScale = new Vector3(dashBar.transform.localScale.x, dashBar.transform.localScale.y, dashBar.transform.localScale.z);

        dashBar.transform.localScale = startingScale;  // Set the initial scale to 0

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 newScale = new Vector3(Mathf.Lerp(startingScale.x, endingScale.x, t), startingScale.y, startingScale.z);
            dashBar.transform.localScale = newScale;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dashBar.transform.localScale = endingScale;  // Ensure the scale is set to the original value at the end
    }

    
    //Update is called once per frame
    //Update is used because the input system settings call events on dynamic update/not fixed update
    void Update()
    {
        GroundCheck();
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        Vector3 movement = new Vector3(_movementInput.x, 0.0f, _movementInput.y);
        _rb.AddForce(movement * currentSpeed, ForceMode.VelocityChange);
        
        // Rotate the character to face the direction of movement
        if (movement.magnitude > 0.1f)  // Avoid rotation when movement is too small or zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    
    //This method uses a physics overlap sphere to check if the player is touching the ground (layer mask).
    //This is a common way to do a "ground check".
    private void GroundCheck()
    {
        Vector3 origin = groundCheck.transform.position;
        if (Physics.OverlapSphere(origin, groundCheckRadius, groundLayer).Length > 0)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(50f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log(health);
    }
}