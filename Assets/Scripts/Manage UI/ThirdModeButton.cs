using UnityEngine;

public class ThirdModeButton : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission && TrialButton.TrialStarted == false && !ModeUI.IsComingType())
        {
            ModeUI.Instance.Fetch(2, "Timer Mode", "You have a limited time"); // Play sound in side this method
        }
        else if (_permission)
        {
            // Play disable sound
            GameSfx.Instance.PlaySound(2);
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
