using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExtraPurchaseButton : MonoBehaviour
{
    [SerializeField]
    private Animator _extraShopPanelAnimator;
    [SerializeField]
    private Text _purchaseText;
    [SerializeField]
    private Button _purchaseButton;
    [SerializeField]
    private Image _purchaseImage;
    [SerializeField]
    private Sprite _purchaseSprite; // enable

    // ******* Player save game Product ********
    public static bool HavePlayNow { get; set; } = false;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public static ExtraPurchaseButton Instance { get; private set; }
    private void Awake() => Instance = this;

    // This has to run before GamePlay cuz HavePlayNow being use in Start() there
    private void Start()
    {
        // Load HavePlayNow from save file (If there is a save)
        if (GameData.IsFileExist("HavePlayNow"))
        {
            HavePlayNow = bool.Parse(GameData.LoadString("HavePlayNow"));
        }

        RefreshButton();
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);

        if (_permission)
        {
            GameSfx.Instance.PlaySound(_purchaseButton.interactable ? (byte)0 : (byte)2);

            if (HavePlayNow == false)
            {
                // Doing iAP stuff (save data there)
                GameIAP.Instance.BuyExtraProducts(0);
            }
        }
    }
    
    public void RefreshButton()
    {
        _purchaseButton.interactable = GameIAP.Instance.IsInitialized();
        _purchaseText.text = GameIAP.Instance.GetExtraPrice(0);

        if (GameIAP.Instance.IsInitialized())
            _purchaseImage.overrideSprite = _purchaseSprite;
    }

    public void CloseAndSetBack()
    {
        GameState.Instance.SetPanelAlpha(80f);

        ExtraButton.Instance.StatePanelTo(2);

        StartCoroutine( ClosePanel() );

        // Set Location
        GameState.Location = "toType";
    }

    public IEnumerator OpenPanel()
    {
        _extraShopPanelAnimator.Play("ExtraShopPanel Get In", -1, 0f);

        yield return new WaitForSeconds(0.75f);

        if (_extraShopPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ExtraShopPanel Get In"))
            _extraShopPanelAnimator.Play("ExtraShopPanel Second Idle", -1, 0f);

        yield break;
    }

    private IEnumerator ClosePanel()
    {
        _extraShopPanelAnimator.Play("ExtraShopPanel Idle", -1, 0f);

        yield return new WaitForSeconds(0.03f);

        _extraShopPanelAnimator.Play("ExtraShopPanel Idle", -1, 0f);

        yield break;
    }
}
