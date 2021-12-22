using UnityEngine;

public class ShopButton : MonoBehaviour
{
    [SerializeField]
    private Animator _menuPanelAnimator;
    [SerializeField]
    private Animator[] _shopPanelsAnimator;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission && GameTutor.TutorStarted == false && (TrialButton.TrialStarted == false || TrialButton.ExploreStarted == true))
        {
            GameSfx.Instance.PlaySound(0);

            // Close MenuPanel, BeginText AND assign alpha
            _menuPanelAnimator.Play("MenuPanel Close", -1, 0f);
            MessageUI.Instance.CloseMessages("all");
            GameState.Instance.SetPanelAlpha(185f);

            // Open First & Second Shops with animation
            _shopPanelsAnimator[0].Play("FirstShopPanel Get In", -1, 0f);
            _shopPanelsAnimator[1].Play("SecondShopPanel Get In", -1, 0f);

            GameState.LerpingTimer2Delay = 1.5f;

            // Set Location
            GameState.Location = "toMenu";

            // Set HasEntered when blinking is started
            if (GameTheme.IsBlinking)
                GameTheme.HasEntered = true;

#if !UNITY_EDITOR && UNITY_ANDROID
            ScoreCenterButton.Instance.CloseButton();
#endif
        }
        else if (_permission)
        {
            GameSfx.Instance.PlaySound(2);
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
