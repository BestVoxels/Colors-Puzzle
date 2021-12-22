using UnityEngine;

public class TrialButton : MonoBehaviour
{
    public static bool TrialStarted { get; set; } = false;
    public static bool ExploreStarted { get; set; } = false;

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
            GameSfx.Instance.PlaySound(GameAd.Instance.ReviveTrialButton[1].interactable ? (byte)0 : (byte)2);

            if (GameTheme.ThemeTrial[GameTheme.CurSlot] == "no")
            {
                ProcessingUI.Instance.ClosePanel("\"Score at least " + GameTheme.ThemeTrialTask[GameTheme.CurSlot] + " First!\"");
            }
            else
            {
                GameAd.Instance.FetchAds("default", 1);
            }
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
