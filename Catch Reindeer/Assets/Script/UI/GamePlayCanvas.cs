
using UnityEngine;
using UnityEngine.UI;

public class GamePlayCanvas : UICanvas
{
    public PlayerMove player;
    public Text playerTxt;
    public Text enemyTxt ;
    public GameManager manager;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMove>();
        }
        if (manager == null)
        {
            manager = FindAnyObjectByType<GameManager>();
        }
    }
    public void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMove>();
        }
        if (manager == null)
        {
            manager = FindAnyObjectByType<GameManager>();
        }
        if (playerTxt != null)
        {
            playerTxt.text = manager.Player.numDeer.ToString()+"/10";
        }
        if (enemyTxt != null)
        {
           enemyTxt.text = manager.Enemy.numDeer.ToString() + "/10";
        }
    }
    public void PauseBtn()
    {
        Time.timeScale = 0;
        UIManager.Instance.OpenUI<PauseCanvas>();
        SoundManager.Instance.PlayClickSound();
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
