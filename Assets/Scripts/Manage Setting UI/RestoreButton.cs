using UnityEngine;

public class RestoreButton : MonoBehaviour
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

            // Reset before get called in ProcessPurchase
            GameIAP.HasRestored = false;

            // Doing restore stuff (only for Apple cuz Google will automatically doing this)
            GameIAP.Instance.RestorePurchases();
        }
    }
}
