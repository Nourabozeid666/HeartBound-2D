using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] float movementSpeed;
    private Animator animator;
    private Vector2 moveInput;

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashCooldown = 0.45f;

    bool isDashing = false;
    bool dashOnCooldown = false;
    private Vector2 dashDirection;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.MovePosition ( rb.position + moveInput * Time.fixedDeltaTime * movementSpeed);
        if (isDashing)
        {
            // Only dash movement while dashing (don’t also apply normal movement this frame)
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            //return;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("lastInputX", moveInput.x);
            animator.SetFloat("lastInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("inputX", moveInput.x);
        animator.SetFloat("inputY", moveInput.y);
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (isDashing || dashOnCooldown) return;
        isDashing = true;
        StartCoroutine(dashtRountine());
        isDashing = false;
    } 
    IEnumerator dashtRountine()
    {
        yield return new WaitForSeconds(0.4f);
    }
}
