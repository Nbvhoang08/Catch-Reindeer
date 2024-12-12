using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : Charecter
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckDistance = 0.1f;


    [Header("Pathfinding Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float maxHorizontalJumpDistance = 5f;

    [SerializeField] private Transform Target;
    private Rigidbody2D rb;
    private Vector2 currentTarget;
    public PathfindingStrategy currentStrategy;
    private PathfindingCalculation pathCalculation;
    private bool isGrounded;
    private float lastStrategyChangeTime;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    

    // Các trạng thái di chuyển chi tiết
    public enum PathfindingStrategy
    {
        DirectApproach,
        VerticalAscent,
        HorizontalTraverse,
        ObstacleAvoidance,
        FindAlternativeRoute
    }

    private struct PathfindingCalculation
    {
        public Vector2 targetPosition;
        public float distanceToTarget;
        public Vector2 directionToTarget;
        public bool isDirectPathBlocked;
        public bool isVerticalApproachPossible;
    }

   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
      /*  InitializePathfinding();*/
        SetTarget((Vector2)Target.position);
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void InitializePathfinding()
    {
        currentStrategy = PathfindingStrategy.DirectApproach;
        lastStrategyChangeTime = Time.time;

    }

    void Update()
    {
        CheckGrounded();
        AnalyzePathToTarget();
        VisualizePathfindingStrategy();
        UpdateAnimation();
        SelectBestStrategy();
       
        IsCurrentStrategyViable();


    }

    void FixedUpdate()
    {
        ExecutePathfindingStrategy();
    }

    void AnalyzePathToTarget()
    {
        // Tính toán chi tiết về đường đi
        pathCalculation = CalculatePathfindingParameters();

        // Quyết định chiến lược di chuyển
        DetermineOptimalStrategy();
    }

    PathfindingCalculation CalculatePathfindingParameters()
    {
        PathfindingCalculation calculation = new PathfindingCalculation
        {
            targetPosition = currentTarget,
            directionToTarget = (currentTarget - (Vector2)transform.position).normalized,
            distanceToTarget = Vector2.Distance(transform.position, currentTarget)
        };

        // Kiểm tra chướng ngại vật trực tiếp
        RaycastHit2D directPathObstacle = Physics2D.Raycast(
            transform.position,
            calculation.directionToTarget,
            calculation.distanceToTarget,
            obstacleLayer | groundLayer
        );

        calculation.isDirectPathBlocked = directPathObstacle.collider != null;

        // Kiểm tra khả năng tiếp cận theo chiều dọc
        calculation.isVerticalApproachPossible = CalculateVerticalApproachFeasibility();
       
        return calculation;
    }

    bool CalculateVerticalApproachFeasibility()
    {
        // Tính toán khả năng nhảy lên vị trí target
        float verticalDistance = Mathf.Abs(currentTarget.y - transform.position.y);
        float horizontalDistance = Mathf.Abs(currentTarget.x - transform.position.x);

        // Áp dụng công thức vật lý để tính toán quỹ đạo nhảy
        float maxReachableHeight = (jumpForce * jumpForce) / (2f * Physics2D.gravity.magnitude);

        // Kiểm tra nếu khoảng cách theo chiều dọc và ngang nằm trong giới hạn cho phép
        if (verticalDistance <= maxReachableHeight && horizontalDistance <= maxHorizontalJumpDistance)
        {
            // Kiểm tra chướng ngại vật trên đường nhảy
            RaycastHit2D verticalObstacle = Physics2D.Raycast(
                transform.position,
                Vector2.up,
                verticalDistance,
                obstacleLayer | groundLayer
            );

            // Nếu có chướng ngại vật, trả về false
            if (verticalObstacle.collider != null)
            {
                return false;
            }

            // Kiểm tra chướng ngại vật theo chiều ngang
            RaycastHit2D horizontalObstacle = Physics2D.Raycast(
                transform.position,
                Vector2.right * Mathf.Sign(currentTarget.x - transform.position.x),
                horizontalDistance,
                obstacleLayer | groundLayer
            );

            // Nếu có chướng ngại vật, trả về false
            if (horizontalObstacle.collider != null)
            {
                return false;
            }

            // Nếu không có chướng ngại vật, trả về true
            return true;
        }

        // Nếu khoảng cách vượt quá giới hạn, trả về false
        return false;
    }

    void DetermineOptimalStrategy()
    {
        // Điều kiện thay đổi chiến lược
        bool shouldChangeStrategy =
            Time.time - lastStrategyChangeTime > 3f || // Thời gian chiến lược
            !IsCurrentStrategyViable(); // Kiểm tra khả thi của chiến lược

        if (shouldChangeStrategy)
        {
            currentStrategy = SelectBestStrategy();
            lastStrategyChangeTime = Time.time;
        }
    }

    PathfindingStrategy SelectBestStrategy()
    {
        if (pathCalculation.isDirectPathBlocked)
        {
            // Nếu bị chặn đường trực tiếp
            if (pathCalculation.isVerticalApproachPossible)
                return PathfindingStrategy.VerticalAscent;
            else
                return PathfindingStrategy.FindAlternativeRoute;
        }
        else
        {
            // So sánh vị trí target để chọn chiến lược
            if (currentTarget.y > transform.position.y)
                return PathfindingStrategy.VerticalAscent;

            return PathfindingStrategy.DirectApproach;
        }
    }

    bool IsCurrentStrategyViable()
    {
        switch (currentStrategy)
        {
            case PathfindingStrategy.DirectApproach:
                return !pathCalculation.isDirectPathBlocked;

            case PathfindingStrategy.VerticalAscent:
                return pathCalculation.isVerticalApproachPossible;

            default:
                return true;
        }
    }

    void ExecutePathfindingStrategy()
    {
        switch (currentStrategy)
        {
            case PathfindingStrategy.DirectApproach:
                MoveDirectlyToTarget();
                break;

            case PathfindingStrategy.VerticalAscent:
                PerformVerticalJump();
                break;

            case PathfindingStrategy.FindAlternativeRoute:
                SeekAlternativeRoute();
                break;
        }
    }

    void MoveDirectlyToTarget()
    {
        Vector2 moveDirection = pathCalculation.directionToTarget;
        rb.velocity = new Vector2(
            moveDirection.x * moveSpeed,
            rb.velocity.y
        );
    }

    void PerformVerticalJump()
    {
        if (!isGrounded) return;
        
        // Tính toán lực nhảy chính xác
        float verticalDistance = currentTarget.y - transform.position.y;
        float jumpAngle = Mathf.Atan2(verticalDistance, Mathf.Abs(currentTarget.x - transform.position.x));

        Vector2 jumpDirection = new Vector2(
            Mathf.Sign(currentTarget.x - transform.position.x) * Mathf.Cos(jumpAngle),
            Mathf.Sin(jumpAngle)
        ).normalized;
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    }

    void SeekAlternativeRoute()
    {
        // Tìm điểm di chuyển thay thế
        Vector2 alternativePosition = FindBestAlternativePosition();
        currentTarget = alternativePosition;
 
    }

    Vector2 FindBestAlternativePosition()
    {
        // Logic tìm vị trí thay thế
        Vector2[] potentialPositions = {
            (Vector2)transform.position + Vector2.right * 5,
            (Vector2)transform.position + Vector2.left * 5,
            (Vector2)transform.position + Vector2.up * 5
        };
   
        Vector2 bestPosition = transform.position;
        float minObstacleDistance = float.MaxValue;

        foreach (Vector2 pos in potentialPositions)
        {
            
            RaycastHit2D obstacleCheck = Physics2D.Raycast(
                transform.position,
                (pos - (Vector2)transform.position).normalized,
                Vector2.Distance(transform.position, pos),
                obstacleLayer
            );

            if (!obstacleCheck.collider &&
                Vector2.Distance(pos, currentTarget) < minObstacleDistance)
            {
                bestPosition = pos;
                Debug.Log(pos);
                minObstacleDistance = Vector2.Distance(pos, currentTarget);
            }
        }

        return bestPosition;
    }

    public void SetTarget(Vector2 target)
    {
        currentTarget = target;
        InitializePathfinding();
  
    }
    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    void VisualizePathfindingStrategy()
    {
        Debug.DrawLine(
            transform.position,
            currentTarget,
            GetStrategyColor(currentStrategy)
        );
    }
    // Phương thức public để set target

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("moving", Mathf.Abs(rb.velocity.x) > 0.1f &&  !isGrounded);
            animator.SetBool("onSky", !isGrounded); 
        }
    }
    Color GetStrategyColor(PathfindingStrategy strategy)
    {
        switch (strategy)
        {
            case PathfindingStrategy.DirectApproach:
                return Color.green;
            case PathfindingStrategy.VerticalAscent:
                return Color.blue;
            case PathfindingStrategy.FindAlternativeRoute:
                return Color.red;
            default:
                return Color.white;
        }
    }
}
