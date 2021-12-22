using UnityEngine;

public class CreditsButton: MonoBehaviour
{
    [SerializeField]
    private Animator _settingPanelAnimator;
    [SerializeField]
    private Animator _creditsPanelAnimator;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

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

            // Open CreditsPanel with animation
            _creditsPanelAnimator.Play("CreditsPanel Get In", -1, 0f);

            // Set Location
            GameState.Location = "toSetting";
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
