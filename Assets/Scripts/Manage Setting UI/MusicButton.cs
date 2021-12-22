using UnityEngine;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField]
    private Image _musicImage;
    [SerializeField]
    private Sprite[] _sprites; // enable, disable sprite

    // Use to check wheather to play music or not
    public static bool CanPlayMusic { get; private set; } = true;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    private void Start()
    {
        // Load CanPlayMusic from save file (If there is a save)
        if (GameData.IsFileExist("CanPlayMusic"))
        {
            CanPlayMusic = bool.Parse(GameData.LoadString("CanPlayMusic"));
        }

        UpdateButtonUI();
        GameMusic.Instance.MuteState(!CanPlayMusic);
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            CanPlayMusic = !CanPlayMusic;

            // Save CanPlayMusic to save file
            GameData.SaveString(CanPlayMusic.ToString(), "CanPlayMusic");

            UpdateButtonUI();
            GameMusic.Instance.MuteState(!CanPlayMusic);
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    private void UpdateButtonUI()
    {
        _musicImage.overrideSprite = (CanPlayMusic == true) ? _sprites[0] : _sprites[1];
    }
}
