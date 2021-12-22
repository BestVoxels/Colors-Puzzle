using UnityEngine;
using UnityEngine.UI; // For Android

public class ScoreCenterButton : MonoBehaviour
{
    // For Android but not close cuz we have to fill out in inspector
    [SerializeField]
    private Image _scoreCenterImage;
    [SerializeField]
    private Sprite[] _sprites; // not pressed, pressed
    [SerializeField]
    private Animator[] _panelsAnimator;
#if !UNITY_EDITOR && UNITY_ANDROID
    public static ScoreCenterButton Instance { get; private set; }
    private void Awake() => Instance = this;
    private void Start() => _scoreCenterImage.overrideSprite = _sprites[0];
#endif

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

            // Sign In for both platform
            GameSocial.SignIn();

            if (GameSocial.LogInStatus == true)
            {
#if !UNITY_EDITOR && UNITY_ANDROID
                if (_panelsAnimator[0].GetCurrentAnimatorStateInfo(0).IsName("LeaderboardPanel Idle"))
                    OpenButton();
                else
                    CloseButton();
#elif !UNITY_EDITOR && UNITY_IOS
                GameSocial.ShowLeaderboardUI();
#endif
            }
        }
        else if (_permission)
        {
            GameSfx.Instance.PlaySound(2);
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

#if !UNITY_EDITOR && UNITY_ANDROID
    private void OpenButton()
    {
        _scoreCenterImage.overrideSprite = _sprites[1];

        // Open First & Second Buttons with animation
        _panelsAnimator[0].Play("LeaderboardPanel Get In", -1, 0f);
        _panelsAnimator[1].Play("AchievementsPanel Get In", -1, 0f);
    }

    public void CloseButton()
    {
        _scoreCenterImage.overrideSprite = _sprites[0];

        // Close First & Second Buttons with animation
        _panelsAnimator[0].Play("LeaderboardPanel Idle", -1, 0f);
        _panelsAnimator[1].Play("AchievementsPanel Idle", -1, 0f);
    }
#endif
}
