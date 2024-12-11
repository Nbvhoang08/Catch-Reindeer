using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayCanvas : UICanvas
{
    public PlayerMove player;
    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMove>();
        }
    }
    public void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMove>();
        }
    }
    public void PauseBtn()
    {
        Time.timeScale = 0;
        UIManager.Instance.OpenUI<PauseCanvas>();
    }
    public void SetMove(float horrizontal)
    {
        player.horizontalInput = horrizontal;
    }
    public void JumpBtn()
    {
        player.Jump();
      
    }
}
