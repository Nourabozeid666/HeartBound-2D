using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] Collider2D Playercollider;

    [Header("Move")]
    [SerializeField] float movementSpeed;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lastFacing = Vector2.down;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 20;
    [SerializeField] float dashCooldown = 0.45f;
    [SerializeField] float dashDuration = 0.15f;

    public bool isDashing { get; private set; }
    bool dashOnCooldown = false;
    private Vector2 dashDirection;
    int playerLayer, obstacleLayer;
    void Awake()
    {
        playerLayer = LayerMask.NameToLayer("player");
        obstacleLayer = LayerMask.NameToLayer("obstacles"); 
        isDashing = false;
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
        moveInput = context.ReadValue<Vector2>();
        if(moveInput.sqrMagnitude > 0.00001f)
        {
            lastFacing = moveInput.normalized;
        }

        animator.SetBool("isWalking", true);
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("lastInputX", moveInput.x);
            animator.SetFloat("lastInputY", moveInput.y);
        }
        animator.SetFloat("inputX", moveInput.x);
        animator.SetFloat("inputY", moveInput.y);
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (isDashing || dashOnCooldown) return;
        //Use 'sqrMagnitude' when you're just comparing many vector lengths (e.g., is the player moving? is enemy close enough?).
        //Use 'magnitude' if you actually need the exact length(e.g., for a UI display or physics effect).
        dashDirection = (moveInput.sqrMagnitude > 0.001f) ? moveInput.normalized : lastFacing;
        StartCoroutine(dashtRountine());
    } 
    IEnumerator dashtRountine()
    {
        isDashing = true;
        animator.SetBool("isDashing", true);
        dashOnCooldown = true;
        Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, true);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        //enable is for components aand setactive is for gameobjects
        animator.SetBool("isDashing",false);
        Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, false);
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }
}
