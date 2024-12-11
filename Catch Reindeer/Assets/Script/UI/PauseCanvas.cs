using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseCanvas : UICanvas
{
    public Sprite OnVolume;
    public Sprite OffVolume;

    [SerializeField] private Image buttonImage;
    void Start()
    {

        UpdateButtonImage();

    }
    public void Resume()
    {
        Time.timeScale = 1;
        UIManager.Instance.CloseUI<PauseCanvas>(0.2f);

        SoundManager.Instance.PlayClickSound();

    }

    public void SoundBtn()
    {
        SoundManager.Instance.TurnOn = !SoundManager.Instance.TurnOn;
        UpdateButtonImage();
        SoundManager.Instance.PlayClickSound();

    }

    private void UpdateButtonImage()
    {
        if (SoundManager.Instance.TurnOn)
        {
            buttonImage.sprite = OnVolume;
        }
        else
        {
            buttonImage.sprite = OffVolume;
        }
    }

}
