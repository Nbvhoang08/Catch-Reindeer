using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Charecter
{
    public Transform target; // Vị trí mục tiêu
    public LayerMask obstacleLayer; // Layer của vật cản
    public float speed = 2f; // Tốc độ di chuyển
    public float JumpForce = 5f; // Lực nhảy
    public float CaughtForce = 5f; // Lực nhảy
    public float nodeDistance = 2f; // Khoảng cách giữa các điểm trong lộ trình
    public float updateInterval = 0.5f; // Khoảng thời gian giữa các lần cập nhật lộ trình
    public int maxNodes = 10; // Số lượng node tối đa
    public AIState state;
    private AIState lastState; // Lưu trạng thái trước đó
    [SerializeField] private List<Transform> path = new List<Transform>();
    [SerializeField] private int currentPoint = 0;
    [SerializeField] private List<PathAI> pathAIs;
    private Rigidbody2D rb;
    private float lastUpdateTime;
    Vector2 targetPosition;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private bool isJumping = false; // Thêm biến cờ để kiểm tra trạng thái nhảy
    public float jumpHeight = 5f; // Chiều cao nhảy
    public float jumpInterval = 2f;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastUpdateTime = Time.time;
        state = AIState.GoHunt;
        animator = GetComponent<Animator>();
        numDeer = 0;
    }

    void Update()
    {
        CheckGrounded();
        UpdateAnimation();  
        switch (state)
        {
            case AIState.GoHunt:
                GoToHunt();
                break;
            case AIState.GoHome:
                GoToHome();
                break;
            case AIState.Hunting:
                Hunting();
                break;
        }          
    }


    void Hunting()
    {
        if (!Caught)
        {
            if (!isJumping)
            {
                StartCoroutine(Hunt());
            }
        }
        else
        {
            state = AIState.GoHome;
            return;
        }
    }

    IEnumerator Hunt()
    {
        while (!Caught)
        {
            // Thời gian chờ ngẫu nhiên giữa các lần nhảy
            float waitTime = UnityEngine.Random.Range(1f, 3f); // Chờ từ 1-3 giây
            yield return new WaitForSeconds(waitTime);

            // Thực hiện nhảy với lực nhảy thay đổi
            if (isGrounded && !isJumping && !Caught)
            {
                float jumpForce = CaughtForce * UnityEngine.Random.Range(0.8f, 1.2f); // Lực nhảy thay đổi nhẹ
                Jump(jumpForce);
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.5f));
                
            }
        }
    }

    void Jump(float jumpForce)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
           
            isJumping = true;

            // Coroutine đặt lại trạng thái nhảy
            StartCoroutine(ResetJumpingState());
        }
    }

    IEnumerator ResetJumpingState()
    {
        yield return new WaitForSeconds(0.5f); // Chờ nhảy hoàn tất

        // Chờ cho đến khi chạm đất
        yield return new WaitUntil(() => isGrounded);

        isJumping = false;
    }
    void GoToHunt()
    {
        int RdIndex = UnityEngine.Random.Range(0, pathAIs.Count - 1);
        // Kiểm tra nếu trạng thái thay đổi
        if (state != lastState)
        {
            // Nếu trạng thái thay đổi, gọi ChangePath
            ChangePath(RdIndex);
            lastState = state; // Cập nhật trạng thái hiện tại
           
        }

        MoveToPosition();
    }

    void GoToHome()
    {
        if (state != lastState)
        {
            // Nếu trạng thái thay đổi, gọi ChangePath
            ChangePath(pathAIs.Count - 1);
            lastState = state; // Cập nhật trạng thái hiện tại

        }
       
        MoveToPosition();

    }
    void ChangePath(int index)
    {
        if(index<= pathAIs.Count -1  && index >=0 )
        {
            path = pathAIs[index].path;
            currentPoint = 0;
            
        }
    }
    void MoveToPosition()
    {

        if (Time.time - lastUpdateTime > updateInterval)
        {
            lastUpdateTime = Time.time;
        }

        if (currentPoint < path.Count)
        {
            targetPosition = (Vector2)path[currentPoint].position;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Quay mặt theo hướng di chuyển
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }

            // Kiểm tra vật cản
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, nodeDistance, obstacleLayer);
            if (hit.collider == null)
            {
                Vector2 currentPosition = transform.position;
                
                if (targetPosition.y - transform.position.y > 0.5f && isGrounded && !isJumping)
                {
                    JumpToPosition(targetPosition);
       
                }
                else
                {
                    rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
                    float distance = currentPosition.x - targetPosition.x;
                
                    if (Mathf.Abs(distance) < 0.1f && !isJumping)
                    {
                        currentPoint++;
                        
                    }
                }
            }
            else
            {
                Debug.Log(hit.collider.name);
                if (targetPosition.y - transform.position.y > 0.5f && isGrounded && !isJumping)
                {
                    JumpToPosition(targetPosition);
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            if(state == AIState.GoHunt)
            {
                state = AIState.Hunting;
            }else if(state == AIState.GoHome && !Caught){
                StartCoroutine(RestAffterGoHome());
            }
        }
    }
    IEnumerator RestAffterGoHome()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 4));
        state = AIState.GoHunt;
    }
    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("moving", Mathf.Abs(rb.velocity.x) > 0.1f && isGrounded);
            animator.SetBool("onSky", !isGrounded);
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
    public void JumpToPosition(Vector2 targetPos)
    {
        if (!isGrounded || isJumping) return; // Kiểm tra trạng thái nhảy hoặc đang không ở trên mặt đất

        isJumping = true; // Đặt cờ nhảy khi bắt đầu nhảy
        if (rb != null)
        {
            StartCoroutine(JumpCoroutine(targetPos));
           
        }
    }

    IEnumerator JumpCoroutine(Vector2 targetPoint)
    {
        Vector2 startPos = (Vector2)transform.position;
        float journeyLength = Vector3.Distance(startPos, targetPoint);
        float throwDuration = journeyLength / JumpForce;

        float elapsedTime = 0;

        // Tính toán đường cong
        Vector2 controlPoint = (startPos + targetPoint) / 2;
        controlPoint.y += journeyLength * 0.75f;

        while (elapsedTime < throwDuration && rb != null)
        {
            float t = elapsedTime / throwDuration;

            Vector2 newPosition = Vector2.Lerp(
                Vector2.Lerp(startPos, controlPoint, t),
                Vector2.Lerp(controlPoint, targetPoint, t),
                t
            );

            rb.MovePosition(newPosition);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            rb.MovePosition(targetPoint);
        }

        // Chỉ tăng `currentPoint` sau khi hoàn thành nhảy
        currentPoint++;

        isJumping = false; // Đặt lại cờ sau khi nhảy xong
    }

}
[Serializable]
public class PathAI
{
    public List<Transform> path;
}

public enum AIState
{
    Hunting,
    GoHome,
    GoHunt
}