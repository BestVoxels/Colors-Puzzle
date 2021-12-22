using UnityEngine;

public class LeaderboardButton : MonoBehaviour
{
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
#if !UNITY_EDITOR && UNITY_ANDROID
            GameSfx.Instance.PlaySound(0);

            GameSocial.ShowLeaderboardUI();
#endif
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
