using UnityEngine;

public class IgButton : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);

        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            // Go to Instagram Account *this will direct to app if user have it, or website if don't*
            Application.OpenURL("https://www.instagram.com/bestvoxels"); // ONLY app "instagram://user?username=bestvoxels"
        }
    }
}
