using UnityEngine;

public class ThirdAnswerButton : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public void PointerDown() => _animator.Play(GamePlay.AnsButton == 0 ? "Press" : "Idle", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (GamePlay.AnsButton == 0)
        {
            if (_permission)
            {
                GamePlay.AnsButton = 3;
            }

            _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
        }
    }
}
