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
    [SerializeField] public float dashSpeed = 20;
    [SerializeField] public float dashCooldown = 0.95f;
    [SerializeField] public float dashDuration = 0.15f;
    [SerializeField] float doubleDashCoolDown = 0.5f;

    [Tooltip("Do not touch that from the hierchey")]
    public int manyDashes=0;

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
            return;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (context.performed || context.started)
        {
            if (moveInput.sqrMagnitude > 0.00001f)
                lastFacing = moveInput.normalized;
            animator.SetBool("isWalking", true);
            animator.SetFloat("inputX", moveInput.x);
            animator.SetFloat("inputY", moveInput.y);
            return;
        }
        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("lastInputX", lastFacing.x);
            animator.SetFloat("lastInputY", lastFacing.y);
            animator.SetFloat("inputX", 0f);
            animator.SetFloat("inputY", 0f);
        }
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
        if (playerLayer >= 0 && obstacleLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, true);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("isDashing", false);

        if (playerLayer >= 0 && obstacleLayer >= 0)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, false);

        // count how many dashes we've done in this chain
        manyDashes++;
        if (dashCooldown>=.3f)
        {
            dashOnCooldown = true;
            yield return new WaitForSeconds(dashCooldown);
            dashOnCooldown = false;
        }

        // If we've spent 2 quick dashes, apply the longer penalty before allowing another
        if (manyDashes >= 2 && dashCooldown < .3f)
        {
            dashOnCooldown = true;
            yield return new WaitForSeconds(doubleDashCoolDown);
            dashOnCooldown = false;
            manyDashes = 0; // reset the chain after the penalty
        }
        // else: allow a second dash immediately (no global cooldown)
    }

}
