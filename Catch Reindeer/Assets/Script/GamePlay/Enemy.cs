using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target; // Vị trí mục tiêu
    public LayerMask obstacleLayer; // Layer của vật cản
    public float speed = 2f; // Tốc độ di chuyển
    public float jumpForce = 5f; // Lực nhảy
    public float nodeDistance = 2f; // Khoảng cách giữa các điểm trong lộ trình
    public float updateInterval = 0.5f; // Khoảng thời gian giữa các lần cập nhật lộ trình
    public int maxNodes = 10; // Số lượng node tối đa

    [SerializeField] private List<Transform> path;
    private int currentPoint = 0;
    private Rigidbody2D rb;
    private float lastUpdateTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        /*path = FindPath(transform.position, target.position);*/
        lastUpdateTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
          
            lastUpdateTime = Time.time;
        }

        if (currentPoint < path.Count)
        {
            Vector2 targetPosition = (Vector2)path[currentPoint].position;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            // Quay mặt theo hướng di chuyển
           
            // Kiểm tra vật cản
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, nodeDistance, obstacleLayer);
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
          
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                Debug.Log(transform.localScale);

            }
          
            if (hit.collider == null )
            {
                Vector2 currentPosition = transform.position;
                // Di chuyển tới điểm tiếp theo
                rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
                float distance = currentPosition.x - targetPosition.x;
                // Kiểm tra khoảng cách để chuyển sang điểm tiếp theo
                if (Mathf.Abs(distance) < 0.1f)
                {
                    currentPoint++;
                }
                
            }
            else
            {
                // Nếu gặp vật cản, thực hiện nhảy
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                Debug.Log(hit.collider.gameObject.name);
            }
        }
        else
        {
           
            rb.velocity = Vector2.zero;
        }
    }

  
}
