using UnityEngine;

public class ReviveButton : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        // This won't be able to click when button isn't opened cuz idle animation already block eventTrigger
        if (_permission)
        {
            GameSfx.Instance.PlaySound(GameAd.Instance.ReviveTrialButton[0].interactable ? (byte)0 : (byte)2);

            // IF player HAVE NOT bought
            if (FirstPurchaseButton.HaveJustRevive == false && GameState.Revive == true)
            {
                GameAd.Instance.FetchAds("default", 0);
            }
            // ELSE IF player HAVE bought
            else if (FirstPurchaseButton.HaveJustRevive == true && GameState.Revive == true)
            {
                GameAd.Instance.GrantRevive();
            }
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}
