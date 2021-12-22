using UnityEngine;
using System.Collections;

public class ArrowButtons : MonoBehaviour
{
   [SerializeField]
    private Animator _shopPanelAnimator;

    private byte[] _minMaxSize = { 0, 8 }; // 8 is the length of the array - 1

    [SerializeField]
    private Animator[] _animators; // left, right
    private bool _permission = false;
    private byte _index = 0;

    public void PointerDown(int buttonIndex)
    {
        _index = (byte)buttonIndex;
        _animators[_index].Play("Press", -1, 0f);
    }
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp ()
    {
        if (_permission)
        {
            if (GameTheme.CurSlot == _minMaxSize[_index])
            {
                // Play disable sound
                GameSfx.Instance.PlaySound(2);
            }
            else
            {
                GameSfx.Instance.PlaySound(0);

                GameTheme.CurSlot = (_index == 0) ? --GameTheme.CurSlot : ++GameTheme.CurSlot;
                GameTheme.Instance.RefreshCurSlot();

                // Play animation sliding in
                StartCoroutine(LRSlideIn());
            }
        }

        _animators[_index].Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    private IEnumerator LRSlideIn()
    {
        string text = _index == 0 ? "SecondShopPanel Left Slide In" : "SecondShopPanel Right Slide In";

        // First sliding in
        _shopPanelAnimator.Play(text, -1, 0f);

        yield return new WaitForSeconds(0.01f);

        // Second sliding in again (incase if in update still true)
        _shopPanelAnimator.Play(text, -1, 0f);

        yield break;
    }
}
