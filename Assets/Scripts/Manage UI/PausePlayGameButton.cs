using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PausePlayGameButton : MonoBehaviour
{
    [SerializeField]
    private byte _countDownSeconds = 3;

    [SerializeField]
    private Text _playText;
    [SerializeField]
    private Animator[] _pausePlayPanelAnimators;
    [SerializeField]
    private Animator _playBackgroundAnimator;

    [SerializeField]
    private Animator[] _animators;
    private bool _permission = false;
    private byte _index = 0;

    public static bool IsGamePause { get; private set; } = false;

    public static PausePlayGameButton Instance { get; private set; }
    private void Awake() => Instance = this;

    public void PointerDown(int buttonIndex)
    {
        _index = (byte)buttonIndex;
        _animators[_index].Play("Press", -1, 0f);
    }
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);
            switch (_index)
            {
                case 0:
                    PauseGame();
                    break;
                case 1:
                    // Start Counting Down
                    StartCoroutine(PlayGame());
                    break;
            }
        }

        _animators[_index].Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    private void PauseGame()
    {
        // Pause the game
        Time.timeScale = 0f;

        IsGamePause = true;

        // Set things back
        OpenPlayBackground();
        OpenPlayPanel();
        ClosePausePanel();

        ProcessingUI.Instance.OpenPanel("");
    }

    private IEnumerator PlayGame()
    {
        CountDownPlayPanel();

        for (byte i = _countDownSeconds; i > 0; i--)
        {
            _playText.text = i.ToString();

            yield return new WaitForSecondsRealtime(1);
        }

        _playText.text = "0";

        // Set things back
        ClosePlayBackground();
        ClosePlayPanel();
        OpenPausePanel();

        ProcessingUI.Instance.InstantClose();

        // Play the Game
        Time.timeScale = 1f;

        IsGamePause = false;

        yield break;
    }

    public void OpenPausePanel() => _pausePlayPanelAnimators[0].Play("PauseGamePanel Get In", -1, 0f);
    public void ClosePausePanel() => _pausePlayPanelAnimators[0].Play("PauseGamePanel Idle", -1, 0f);

    private void OpenPlayPanel() => _pausePlayPanelAnimators[1].Play("PlayGamePanel Get In", -1, 0f);
    private void CountDownPlayPanel() => _pausePlayPanelAnimators[1].Play("PlayGamePanel Count Down", -1, 0f);
    private void ClosePlayPanel() => _pausePlayPanelAnimators[1].Play("PlayGamePanel Idle", -1, 0f);

    private void OpenPlayBackground() => _playBackgroundAnimator.Play("PlayGameBackgroundImage Get In", -1, 0f);
    private void ClosePlayBackground() => _playBackgroundAnimator.Play("PlayGameBackgroundImage Idle", -1, 0f);
}