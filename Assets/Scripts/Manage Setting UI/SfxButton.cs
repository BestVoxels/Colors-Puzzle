using UnityEngine;
using UnityEngine.UI;

public class SfxButton : MonoBehaviour
{
    [SerializeField]
    private Image _sfxImage;
    [SerializeField]
    private Sprite[] _sprites; // enable, disable sprite

    // Use to check wheather to play sfx noti or not
    public static bool CanPlaySfx { get; private set; } = true;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    private void Start()
    {
        // Load CanPlaySfx from save file (If there is a save)
        if (GameData.IsFileExist("CanPlaySfx"))
        {
            CanPlaySfx = bool.Parse(GameData.LoadString("CanPlaySfx"));
        }

        UpdateButtonUI();
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            CanPlaySfx = !CanPlaySfx;

            // Save CanPlaySfx to save file
            GameData.SaveString(CanPlaySfx.ToString(), "CanPlaySfx");

            UpdateButtonUI();
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    private void UpdateButtonUI()
    {
        _sfxImage.overrideSprite = (CanPlaySfx == true) ? _sprites[0] : _sprites[1];
    }
}
