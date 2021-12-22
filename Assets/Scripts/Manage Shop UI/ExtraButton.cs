using UnityEngine;

public class ExtraButton : MonoBehaviour
{
    [SerializeField]
    private Animator _extraPanelAnimator;
    [SerializeField]
    private Transform[] _transforms; // gameStatePanel, groundPanel, typePanel

    private byte[] _positions = new byte[3];

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public static ExtraButton Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        for (byte i = 0; i < _positions.Length; i++)
            _positions[i] = (byte)_transforms[i].GetSiblingIndex();
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        if (_permission)
        {
            GameSfx.Instance.PlaySound(0);

            // Close Extra Button AND assign alpha
            ClosePanel();
            GameState.Instance.SetPanelAlpha(120f);

            StatePanelTo(1);

            // Open ExtraShop Panel with animation
            StartCoroutine( ExtraPurchaseButton.Instance.OpenPanel() );

            // Set Location
            GameState.Location = "toModefromExtra";
        }

        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);
    }

    public void StatePanelTo(byte number) => _transforms[0].SetSiblingIndex(_positions[number]);

    public void OpenPanel() => _extraPanelAnimator.Play("ExtraPanel Get In", -1, 0f);

    public void ClosePanel()
    {
        // If ExtraPanel is OPEN Close with animation
        if (_extraPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ExtraPanel Get In"))
        {
            _extraPanelAnimator.Play("ExtraPanel Idle", -1, 0f);
        }
    }
}
