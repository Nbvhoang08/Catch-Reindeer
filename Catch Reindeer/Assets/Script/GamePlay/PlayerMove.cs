using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove :  Charecter
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundCheckDistance = 0.1f;

    [Header("Input Settings")]
    [SerializeField] private float inputCooldown = 0.2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Input cooldown tracking
    private float lastMoveTime;
  
    public float horizontalInput;
    private bool isJumpRequested;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Xử lý input
        HandleInput();

        // Kiểm tra va chạm mặt đất
        CheckGrounded();
        if (isGrounded)
        {
            animator.SetBool("moving", horizontalInput != 0);
        }
        animator.SetBool("onSky",!isGrounded);


    }

    void FixedUpdate()
    {
        // Di chuyển và nhảy trong FixedUpdate để đảm bảo physics smooth
        HandleMovement();
        ClampPosition();
    }

    void HandleInput()
    {
        // Ngăn chặn spam input
        if (Time.time - lastMoveTime < inputCooldown)
            return;

      

        // Xoay sprite dựa trên hướng di chuyển
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
            lastMoveTime = Time.time;
        }

    }

    public void Jump()
    {
        if (isGrounded)
        {
            isJumpRequested = true;
            lastMoveTime = Time.time;
        }
       
       
    }


    void CheckGrounded()
    {
        // Kiểm tra va chạm mặt đất bằng Raycast
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            LayerMask.GetMask("Ground")
        );
       
        isGrounded = hit.collider != null;
    }

    void HandleMovement()
    {
        // Di chuyển ngang
        float targetVelocity = horizontalInput * moveSpeed;
        rb.velocity = new Vector2(targetVelocity, rb.velocity.y);

        // Nhảy
        if (isJumpRequested && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumpRequested = false;
        }
    }

    // Giới hạn di chuyển (tuỳ chọn)
    void ClampPosition()
    {
        // Lấy kích thước của màn hình
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // Giới hạn vị trí của người chơi trong khoảng từ -screenWidth đến screenWidth
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenWidth, screenWidth);
        transform.position = pos;
    }




}
