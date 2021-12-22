using UnityEngine;
using System.Collections;

public class TypeButton : MonoBehaviour
{
    private IEnumerator _typeInfo;

    [SerializeField]
    private Animator[] _animators;
    private bool _permission = false;
    private byte _index = 0;

    public void PointerDown(int buttonIndex)
    {
        _index = (byte)buttonIndex;
        _animators[_index].Play("Press", -1, 0f);
    }
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            switch (_index)
            {
                case 0:
                    // Decrement with Fetch Text method, no play sound
                    TypeUI.Instance.RefreshStuff(Operator1.Decrement);

                    // Play Slide Panel
                    TypeUI.Instance.SlideRightTypePanel();
                    break;
                case 1:
                    // Update Type Info Text, no play sound (no need to call here but for more flexibility in future)
                    TypeUI.Instance.Fetch();

                    // Open TypeInfo Panel Again
                    
                    if (_typeInfo != null)
                        StopCoroutine(_typeInfo);
                    
                    _typeInfo = TypeUI.Instance.OpenTypeInfoPanel();

                    StartCoroutine(_typeInfo);
                    break;
                case 2:
                    // Increment with Fetch Text method, no play sound
                    TypeUI.Instance.RefreshStuff(Operator1.Increment);

                    // Play Slide Panel
                    TypeUI.Instance.SlideLeftTypePanel();
                    break;
            }
        }

        _animators[_index].Play(_permission ? "Up" : "Idle", -1, 0f);
    }
}