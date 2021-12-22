using UnityEngine;
using UnityEngine.UI;

public class NotificationButton : MonoBehaviour
{
    [SerializeField]
    private Image _notificationImage;
    [SerializeField]
    private Sprite[] _sprites; // enable, disable sprite

    // Use to check wheather to send noti or not
    public static bool CanSend { get; private set; } = true;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    private void Start()
    {
        // Load CanSend from save file (If there is a save)
        if (GameData.IsFileExist("CanSend"))
        {
            CanSend = bool.Parse(GameData.LoadString("CanSend"));
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

            CanSend = !CanSend;

            // Save CanSend to save file
            GameData.SaveString(CanSend.ToString(), "CanSend");

            UpdateButtonUI();
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    private void UpdateButtonUI()
    {
        _notificationImage.overrideSprite = (CanSend == true) ? _sprites[0] : _sprites[1];
    }
}
