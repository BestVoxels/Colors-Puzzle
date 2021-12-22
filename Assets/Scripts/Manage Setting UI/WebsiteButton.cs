using UnityEngine;

public class WebsiteButton : MonoBehaviour
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

            // Go to Website
            Application.OpenURL("https://www.bestvoxels.com");
        }
    }
}
