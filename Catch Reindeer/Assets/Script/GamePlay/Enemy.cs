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

            // Kiểm tra vật cản
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, nodeDistance, obstacleLayer);
            if (hit.collider == null)
            {
                Debug.Log("GG");
                // Di chuyển tới điểm tiếp theo
                rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
                
                // Kiểm tra khoảng cách để chuyển sang điểm tiếp theo
                if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                {
                    currentPoint++;
                }
                else
                {
                    Debug.Log("jam");
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
            Debug.Log("Done");
            // Dừng lại khi đến đích
            rb.velocity = Vector2.zero;
        }
    }

   /* List<Vector2> FindPath(Vector2 start, Vector2 goal)
    {
        List<Vector2> openList = new List<Vector2>();
        HashSet<Vector2> closedList = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, float> gScore = new Dictionary<Vector2, float>();
        Dictionary<Vector2, float> fScore = new Dictionary<Vector2, float>();

        openList.Add(start);
        gScore[start] = 0;
        fScore[start] = Vector2.Distance(start, goal);

        Debug.Log("Starting pathfinding from " + start + " to " + goal);

        Vector2 current = start; // Khởi tạo biến current

        while (openList.Count > 0)
        {
            current = openList[0];
            foreach (var node in openList)
            {
                if (fScore[node] < fScore[current])
                {
                    current = node;
                }
            }

            Debug.Log("Current node: " + current);

            if (Vector2.Distance(current, goal) < nodeDistance)
            {
                Debug.Log("Goal reached");
                return ReconstructPath(cameFrom, current, goal);
            }

            openList.Remove(current);
            closedList.Add(current);

            if (closedList.Count > maxNodes)
            {
                Debug.Log("Max nodes reached");
                break; // Dừng lại nếu số lượng node vượt quá giới hạn
            }

            foreach (var neighbor in GenerateNeighbors(current, goal))
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + Vector2.Distance(current, neighbor);

                if (!openList.Contains(neighbor))
                {
                    Debug.Log("Adding neighbor: " + neighbor);
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Vector2.Distance(neighbor, goal);
            }
        }

        Debug.Log("No path found, returning partial path");
        return ReconstructPartialPath(cameFrom, current); // Trả về danh sách các node đã xác định được
    }

    List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current, Vector2 goal)
    {
        List<Vector2> totalPath = new List<Vector2> { goal };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        Debug.Log("Path found: " + string.Join(", ", totalPath));
        return totalPath;
    }

    List<Vector2> ReconstructPartialPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        List<Vector2> totalPath = new List<Vector2> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        Debug.Log("Partial path found: " + string.Join(", ", totalPath));
        return totalPath;
    }

    List<Vector2> GenerateNeighbors(Vector2 node, Vector2 goal)
    {
        List<Vector2> neighbors = new List<Vector2>();

        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            Vector2.up + Vector2.right, Vector2.up + Vector2.left,
            Vector2.down + Vector2.right, Vector2.down + Vector2.left
        };

        foreach (var direction in directions)
        {
            Vector2 neighbor = node + direction * nodeDistance;
            if (!Physics2D.OverlapCircle(neighbor, 0.1f, obstacleLayer))
            {
                // Chỉ thêm các node dẫn đến mục tiêu
                if (Vector2.Distance(neighbor, goal) < Vector2.Distance(node, goal))
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }*/

/*    void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }*/
}
