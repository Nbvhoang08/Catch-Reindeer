using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerSpawner : MonoBehaviour
{
    public GameObject deerPrefab; // Prefab của con hươu
    public float spawnInterval = 2f; // Khoảng thời gian giữa các lần spawn
    public float screenWidth = 10f; // Chiều rộng của màn hình
    private int maxDeerCount = 5; // Số lượng hươu tối đa chưa bị bắt
    private List<GameObject> deerList = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnDeer());
    }

    IEnumerator SpawnDeer()
    {
        while (true)
        {
            if (deerList.Count < maxDeerCount)
            {
                SpawnSingleDeer();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSingleDeer()
    {
        Vector3 spawnPosition = new Vector3(-screenWidth / 2, 0, 0); // Không cần random theo trục dọc
        GameObject deer = Instantiate(deerPrefab, spawnPosition, Quaternion.identity);
        deer.GetComponent<Deer>().Initialize(screenWidth);
        deerList.Add(deer);
    }

    public void RemoveDeer(GameObject deer)
    {
        deerList.Remove(deer);
    }
}

