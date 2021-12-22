using UnityEngine;
using UnityEngine.UI;

public class FirstPurchaseButton : MonoBehaviour
{
    // For First Shop UI to manage
    [SerializeField]
    private Text _shopHeaderText;
    [SerializeField]
    private Text _shopInfoText;
    [SerializeField]
    private Text _purchaseText;
    [SerializeField]
    private Button _purchaseButton;
    [SerializeField]
    private Image _purchaseImage;
    [SerializeField]
    private Sprite _purchaseSprite; // enable

    // For Revive button to manage
    [SerializeField]
    private GameObject _reviveTextGameObject;
    [SerializeField]
    private Image _reviveButtonImage;
    [SerializeField]
    private Sprite _justReviveSprite;

    // For promotions
    private string[] _promotionInfo =
    {
        "+ Just Revive!!!\n+ EVERY Themes in SHOP!!!!\n\n",
        "+ Just Revive!!!\n+ " + GameTheme.ThemeName[1] + " & " + GameTheme.ThemeName[2] + "!!!\n(2 themes)\n\n",
        "+ Just Revive!!!\n+ " + GameTheme.ThemeName[3] + " & " + GameTheme.ThemeName[4] + "!!!\n(2 themes)\n\n",
        "+ Just Revive!!!\n+ " + GameTheme.ThemeName[5] + " & " + GameTheme.ThemeName[6] + "!!!\n(2 themes)\n\n",
        "+ Just Revive!!!\n+ " + GameTheme.ThemeName[7] + " & " + GameTheme.ThemeName[8] + "!!!\n(2 themes)\n\n",
        "+ EVERY Themes in SHOP!!!!\n\n",
        "+ 3 themes!!!\n("+ GameTheme.ThemeName[3] + ", " + GameTheme.ThemeName[4] + ", " + GameTheme.ThemeName[5] + ")\n\n",
        "+ 3 themes!!!\n("+ GameTheme.ThemeName[6] + ", " + GameTheme.ThemeName[7] + ", " + GameTheme.ThemeName[8] + ")\n\n",
        "+ 3 themes!!!\n("+ GameTheme.ThemeName[1] + ", " + GameTheme.ThemeName[2] + ", " + GameTheme.ThemeName[5] + ")\n\n",
        "+ 3 themes!!!\n("+ GameTheme.ThemeName[1] + ", " + GameTheme.ThemeName[2] + ", " + GameTheme.ThemeName[3] + ")\n\n",
        "+ 3 themes!!!\n("+ GameTheme.ThemeName[4] + ", " + GameTheme.ThemeName[7] + ", " + GameTheme.ThemeName[8] + ")\n\n",
        "+ 3 themes!!!\n("+ GameTheme.ThemeName[4] + ", " + GameTheme.ThemeName[5] + ", " + GameTheme.ThemeName[6] + ")\n\n",
    };

    // ******* Player save game Product ********
    public static bool HaveJustRevive { get; set; } = false;

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    public static FirstPurchaseButton Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        // Load HaveJustRevive from save file (If there is a save)
        if (GameData.IsFileExist("HaveJustRevive"))
        {
            HaveJustRevive = bool.Parse(GameData.LoadString("HaveJustRevive"));
        }

        RefreshReviveButton(); // Call RefreshFirstShop in GamePromotion cuz CurShowing will get load after this
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

            // IF There is a promotion showing OR player HAVE NOT bought Just Revive
            if (GamePromotion.CurShowing != -1)
            {
                // Doing iAP stuff (save data there)
                GameIAP.Instance.BuyPromotionProducts((byte)GamePromotion.CurShowing);
            }
            else if (HaveJustRevive == false)
            {
                // Doing iAP stuff (save data there)
                GameIAP.Instance.BuyProducts(0);
            }
        }
    }

    public void RefreshFirstShop()
    {
        if (GamePromotion.CurShowing != -1)
        {
            _shopHeaderText.text = "*PROMOTION*";

            _shopInfoText.text = _promotionInfo[GamePromotion.CurShowing];

            _purchaseButton.interactable = GameIAP.Instance.IsInitialized();
            _purchaseText.text = GameIAP.Instance.GetPromoPrice((byte)GamePromotion.CurShowing);

            if (GameIAP.Instance.IsInitialized())
                _purchaseImage.overrideSprite = _purchaseSprite;
            // (GamePromotion's algorithm can call this method to update text with refreshAllUI)
        }
        else if (GamePromotion.CurShowing == -1)
        {
            // IF player HAVE NOT bought JR
            if (HaveJustRevive == false)
            {
                _shopHeaderText.text = "Just Revive";

                _shopInfoText.text = "~ No more VIDEOS!\n~ DOUBLE Revive Limit!\n\n(Life Time Product)";

                _purchaseButton.interactable = GameIAP.Instance.IsInitialized();
                _purchaseText.text = GameIAP.Instance.GetPrice(0);

                if (GameIAP.Instance.IsInitialized())
                    _purchaseImage.overrideSprite = _purchaseSprite;
            }
            // IF player HAVE bought JR
            else if (HaveJustRevive == true)
            {
                _shopHeaderText.text = "$Purchased$";

                _shopInfoText.text = "~ Thank you!! ^^\n\n~ Please enjoy";

                _purchaseButton.interactable = false;
                _purchaseText.text = "sold";
                _purchaseImage.overrideSprite = _purchaseSprite;
            }
        }
    }

    public void RefreshReviveButton()
    {
        // IF player bought SUCCESS
        if (HaveJustRevive == true)
        {
            // Open ReviveText GameObject
            _reviveTextGameObject.SetActive(true);

            // Close Video Sprite on ReviveButton Image
            _reviveButtonImage.overrideSprite = _justReviveSprite;

            // Additional Double revive limit
            GamePlay.Instance.ReviveAmount = 6;
        }
    }

    public void AddShopInfoText(string text)
    {
        _shopInfoText.text = _promotionInfo[GamePromotion.CurShowing] + text;
    }
}
