using UnityEngine;
using UnityEngine.UI;

public class SetBackgroundButton : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _particles;
    [SerializeField]
    private Text _setBackgroundText;
    [SerializeField]
    private Image _setBackgroundImage;

    [SerializeField]
    private Sprite[] _sprites; // enable, disable sprite

    // Use to check wheather to background is shwoing or not
    public static bool IsBGShowing { get; private set; } = true;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    private void Start()
    {
        // Load IsBGShowing from save file (If there is a save)
        if (GameData.IsFileExist("IsBGShowing"))
        {
            IsBGShowing = bool.Parse(GameData.LoadString("IsBGShowing"));
        }

        UpdateButtonUI();
        SetVisibilityNText();
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            // Set Value
            IsBGShowing = !IsBGShowing;

            // Call
            SetVisibilityNText();

            // Save IsBGShowing to save file
            GameData.SaveString(IsBGShowing.ToString(), "IsBGShowing");

            UpdateButtonUI();
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    private void UpdateButtonUI()
    {
        _setBackgroundImage.overrideSprite = (IsBGShowing == true) ? _sprites[0] : _sprites[1];
    }

    private void SetVisibilityNText()
    {
        foreach (GameObject each in _particles)
            each.SetActive(IsBGShowing);

        _setBackgroundText.text = IsBGShowing ? "Hide Background" : "Show Background";
    }
}
