using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Charecter Player;
    public Charecter Enemy;
    public int WinningDeerCount = 10; // Số lượng hươu cần để thắng

    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver)
            return;

        // Kiểm tra điều kiện thắng thua
        if (Player.numDeer >= WinningDeerCount && Enemy.numDeer >= WinningDeerCount)
        {
            // Nếu cả hai đạt cùng lúc
            GameWin();
        }
        else if (Player.numDeer >= WinningDeerCount)
        {
            // Character 1 thắng
            GameWin();
        }
        else if (Enemy.numDeer >= WinningDeerCount)
        {
            // Character 2 thắng (tức là người chơi thua)
            GameOver();
        }
    }

    private void GameWin()
    {
        isGameOver = true;
        StartCoroutine(WinGame());
        Debug.Log("Game Win");
    }
    IEnumerator WinGame()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.OpenUI<Win>();// Hiển thị giao diện Win
        Time.timeScale = 0f; // Dừng thời gian
    }
    
    private void GameOver()
    {
        isGameOver = true;
        StartCoroutine(LoseGame());
        Debug.Log("Game Over");
    }
    IEnumerator LoseGame()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.OpenUI<Lose>();// Hiển thị giao diện Lose
        Time.timeScale = 0f; // Dừng thời gian
    }
}
