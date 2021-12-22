using UnityEngine;
using UnityEngine.UI;

public class ProcessingUI : MonoBehaviour
{
    [SerializeField]
    private Text _infoText;
    public Text InfoText { get { return _infoText; } }

    [SerializeField]
    private Animator _processingPanelAnimator;

    public static ProcessingUI Instance { get; private set; }
    private void Awake() => Instance = this;

    public void OpenPanel(string text)
    {
        InfoText.text = text;

        _processingPanelAnimator.Play("ProcessingPanel Get In", -1, 0f);
    }

    public void ClosePanel(string text)
    {
        InfoText.text = text;
        
        _processingPanelAnimator.Play("ProcessingPanel Get Out", -1, 0f);
    }

    public void InstantClose() => _processingPanelAnimator.Play("ProcessingPanel Idle", -1, 0f);

    // Being used by Android Escape Key in GameState
    public bool IsClosed() => _processingPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ProcessingPanel Idle") ||
            (_processingPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ProcessingPanel Get Out") && _processingPanelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
}
