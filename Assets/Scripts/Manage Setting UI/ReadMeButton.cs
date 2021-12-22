using UnityEngine;
using UnityEngine.UI;

public class ReadMeButton : MonoBehaviour
{
    [SerializeField]
    private Animator _settingPanelAnimator;
    [SerializeField]
    private Animator _readMePanelAnimator;

    [SerializeField]
    private Text _saveGameInfoText;
    [SerializeField]
    private Text _saveIAPInfoText;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        _saveGameInfoText.text = "Log in to Google Play to save high score & dark theme\n(when unlocked by score)";
        _saveIAPInfoText.text = "If you make any purchase the system will restore automatically";
#elif !UNITY_EDITOR && UNITY_IOS
        _saveGameInfoText.text = "Log in to Game Center to save high score & dark theme\n(when unlocked by score)";
        _saveIAPInfoText.text = "If you make any purchase you could restore from support button";
#endif
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            // Close SettingPanel, AND assign alpha
            _settingPanelAnimator.Play("SettingPanel Idle", -1, 0f);
            GameState.Instance.SetPanelAlpha(220f);

            // Open ReadMePanel with animation
            _readMePanelAnimator.Play("ReadMePanel Get In", -1, 0f);

            // Set Location
            GameState.Location = "toSetting";
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
