using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float obstacleAvoidanceDistance = 1f;

    [Header("AI Settings")]
    [SerializeField] private Transform homeBase; // Fixed destination point
    [SerializeField] private LayerMask targetLayer; // Layer for potential targets
    [SerializeField] private LayerMask obstacleLayer; // Layer for obstacles

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Transform currentTarget;
    private bool isGrounded;
    private bool isTrackingTarget;
    private Vector2 movementDirection;
    private float lastJumpTime;
    private float jumpCooldown = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        DetectTargets();
        UpdateAnimation();
        HandleObstacleAvoidance();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void DetectTargets()
    {
        // Find potential targets within detection radius
        Collider2D targetCollider = Physics2D.OverlapCircle(
            transform.position,
            detectionRadius,
            targetLayer
        );

        if (targetCollider != null)
        {
            currentTarget = targetCollider.transform;
            isTrackingTarget = true;
        }
        else
        {
            isTrackingTarget = false;
            currentTarget = null;
        }
    }

    void HandleObstacleAvoidance()
    {
        // Check for obstacles in movement path
        RaycastHit2D obstacleCheck = Physics2D.Raycast(
            transform.position,
            movementDirection,
            obstacleAvoidanceDistance,
            obstacleLayer
        );

        if (obstacleCheck.collider != null && Time.time - lastJumpTime > jumpCooldown)
        {
            // If obstacle detected, attempt to jump over
            TryJump();
        }
    }

    void HandleMovement()
    {
        if (isTrackingTarget && currentTarget != null)
        {
            // Move towards target
            movementDirection = (currentTarget.position - transform.position).normalized;

            // Horizontal movement
            float targetVelocity = movementDirection.x * moveSpeed;
            rb.velocity = new Vector2(targetVelocity, rb.velocity.y);

            // Flip sprite based on movement direction
            if (targetVelocity != 0)
            {
                spriteRenderer.flipX = targetVelocity < 0;
            }

            // Check if target is caught
            if (Vector2.Distance(transform.position, currentTarget.position) < 0.5f)
            {
                ReturnToHomeBase();
            }
        }
        else
        {
            // Return to home base when no target
            ReturnToHomeBase();
        }
    }

    void ReturnToHomeBase()
    {
        if (homeBase != null)
        {
            movementDirection = (homeBase.position - transform.position).normalized;
            float targetVelocity = movementDirection.x * moveSpeed;
            rb.velocity = new Vector2(targetVelocity, rb.velocity.y);
        }
    }

    void TryJump()
    {
        if (isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            LayerMask.GetMask("Ground")
        );

        isGrounded = hit.collider != null;
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("moving", Mathf.Abs(rb.velocity.x) > 0.1f);
            animator.SetBool("onSky", !isGrounded);
        }
    }

    // Optional: Visualize detection radius in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
