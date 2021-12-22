using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTheme : MonoBehaviour
{
    // ******UI FOR THEME******
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private Image _gameStatePanelImage;
    [SerializeField]
    private Image _playGameBackgroundImage;
    [SerializeField]
    private Image _scorePanelImage;

    [SerializeField]
    private Text _headerText;
    [SerializeField]
    private Text _subHeaderText;
    [SerializeField]
    private RectTransform _subHeaderRect;

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text[] _gameText; // begin, instruction, question

    [SerializeField]
    private Text[] _typeInfoText;
    [SerializeField]
    private Text[] _modeInfoText; // element 3 - 5 for Mode Indicator Text Under each Mode

    [SerializeField]
    private Text[] _messageText;

    [SerializeField]
    private Image[] _menuPanelImage;
    [SerializeField]
    private Image[] _typePanelImage;
    [SerializeField]
    private Image[] _modePanelImage;
    [SerializeField]
    private Image _groundImage;

    [SerializeField]
    private Image[] _socialImage;

    [SerializeField]
    private Image[] _leftRightImage;


    // ******UI FOR BOTH******
    [SerializeField]
    private Text[] _shopHeaderText; // Second element is also for changing text
    [SerializeField]
    private Text[] _shopInfoText; // Second element is also for changing text
    [SerializeField]
    private Text[] _purchaseText; // Second element is also for changing text
    [SerializeField]
    private Image[] _purchaseImage; // Second element is also for changing sprite
    [SerializeField]
    private Image[] _dotsImage;

    // ******UI FOR MANAGING SLOT & PROMOTION PANEL******
    [SerializeField]
    private Button _purchaseButton;
    public Button PurchaseButton { get { return _purchaseButton; } } // For SecondPurchaseButton to check
    [SerializeField]
    private Sprite[] _purchaseSprites; // enable, disable
    [SerializeField]
    private RectTransform[] _dotsRect;
    [SerializeField]
    private Image _shopHeaderImage;
    [SerializeField]
    private Image[] _shopBorderImages;
    [SerializeField]
    private Button[] _leftRightButtons;
    [SerializeField]
    private Image _shopPanelImage;
    [SerializeField]
    private Image _trialImage; // Closing Image
    [SerializeField]
    private Text _trialText; // Closing Text
    [SerializeField]
    private RectTransform _shopInfoRect;


    // ******COLORS FOR THEME******
    private readonly Color32[] _mainCameraColors =
    {
        new Color32(36, 35, 41, 255), // dark theme
        new Color32(255, 252, 185, 255), // gold theme
        new Color32(230, 255, 175, 255), // light green theme
        new Color32(210, 251, 255, 255), // light blue theme
        new Color32(201, 224, 255, 255), // blue theme
        new Color32(255, 231, 192, 255), // orange theme
        new Color32(255, 215, 201, 255), // light red theme
        new Color32(228, 210, 255, 255), // purple theme
        new Color32(255, 210, 240, 255), // pink theme
        new Color32(253, 244, 218, 255) // default theme
    };

    private readonly Color32[] _gameStatePanelColors =
    {
        new Color32(100, 100, 100, 0), // dark theme
        new Color32(253, 242, 132, 0), // gold theme
        new Color32(224, 255, 143, 0), // light green theme
        new Color32(179, 254, 255, 0), // light blue theme
        new Color32(170, 211, 255, 0), // blue theme
        new Color32(255, 208, 138, 0), // orange theme
        new Color32(253, 183, 158, 0), // light red theme
        new Color32(207, 182, 255, 0), // purple theme
        new Color32(255, 189, 233, 0), // pink theme
        new Color32(255, 234, 179, 0) // default theme
    };

    private readonly Color32[] _headerColors =
    {
        new Color32(241, 241, 241, 255), // dark theme
        new Color32(156, 134, 0, 255), // gold theme
        new Color32(62, 118, 8, 255), // light green theme
        new Color32(19, 131, 137, 255), // light blue theme
        new Color32(42, 92, 140, 255), // blue theme
        new Color32(154, 91, 17, 255), // orange theme
        new Color32(137, 62, 34, 255), // light red theme
        new Color32(82, 45, 152, 255), // purple theme
        new Color32(144, 20, 102, 255), // pink theme
        new Color32(120, 120, 120, 255) // default theme
    };

    private readonly Color32[] _subHeaderColors =
    {
        new Color32(214, 214, 214, 255), // dark theme
        new Color32(185, 160, 10, 255), // gold theme
        new Color32(99, 161, 11, 255), // light green theme
        new Color32(23,170, 179, 255), // light blue theme
        new Color32(72, 131, 188, 255), // blue theme
        new Color32(192, 120, 34, 255), // orange theme
        new Color32(178, 90, 58, 255), // light red theme
        new Color32(121, 76, 204, 255), // purple theme
        new Color32(197, 70, 154, 255), // pink theme
        new Color32(140, 140, 140, 255) // default theme
    };

    private readonly string[] _subHeaderRichColors =
    {
        "FFFFFF", // dark theme
        "D7B908", // gold theme
        "6FB00D", // light green theme
        "26BBC3", // light blue theme
        "5E99D2", // blue theme
        "DB8B2B", // orange theme
        "CB7252", // light red theme
        "9B70EA", // purple theme
        "E766BC", // pink theme
        "A2A2A2" // default theme
    };

    private readonly Color32[] _gameTextColors =
    {
        new Color32(233, 233, 233, 255), // dark theme
        new Color32(183, 164, 65, 255), // gold theme
        new Color32(114, 164, 0, 255), // light green theme
        new Color32(86, 164, 163, 255), // light blue theme
        new Color32(96, 134, 190, 255), // blue theme
        new Color32(188, 133, 66, 255), // orange theme
        new Color32(183, 114, 89, 255), // light red theme
        new Color32(133, 94, 204, 255), // purple theme
        new Color32(192, 104, 172, 255), // pink theme
        new Color32(135, 135, 135, 255) // default theme
    };

    private readonly Color32[] _menuPanelColors =
    {
        new Color32(190, 190, 190, 255), // dark theme
        new Color32(227, 224, 151, 255), // gold theme
        new Color32(198, 234, 136, 255), // light green theme
        new Color32(158, 234, 230, 255), // light blue theme
        new Color32(153, 202, 255, 255), // blue theme
        new Color32(234, 199, 160, 255), // orange theme
        new Color32(243, 181, 160, 255), // light red theme
        new Color32(201, 178, 243, 255), // purple theme
        new Color32(246, 173, 221, 255), // pink theme
        new Color32(224, 212, 185, 255) // default theme
    };

    private readonly Color32[] _style1Colors =
    {
        new Color32(200, 200, 200, 255), // dark theme
        new Color32(226, 223, 144, 255), // gold theme
        new Color32(198, 231, 130, 255), // light green theme
        new Color32(164, 231, 229, 255), // light blue theme
        new Color32(165, 206, 251, 255), // blue theme
        new Color32(231, 196, 156, 255), // orange theme
        new Color32(241, 177, 156, 255), // light red theme
        new Color32(200, 178, 241, 255), // purple theme
        new Color32(248, 175, 223, 255), // pink theme
        new Color32(224, 210, 178, 255) // default theme
    };

    private readonly Color32[] _style2Colors =
    {
        new Color32(150, 150, 150, 255), // dark theme
        new Color32(207, 203, 110, 255), // gold theme
        new Color32(175, 217, 105, 255), // light green theme
        new Color32(127, 209, 207, 255), // light blue theme
        new Color32(135, 187, 243, 255), // blue theme
        new Color32(217, 175, 128, 255), // orange theme
        new Color32(229, 150, 124, 255), // light red theme
        new Color32(182, 151, 241, 255), // purple theme
        new Color32(250, 143, 213, 255), // pink theme
        new Color32(203, 186, 148, 255) // default theme
    };

    private readonly Color32[] _groundColors =
    {
        new Color32(160, 160, 160, 255), // dark theme
        new Color32(229, 225, 133, 255), // gold theme
        new Color32(194, 229, 126, 255), // light green theme
        new Color32(145, 231, 230, 255), // light blue theme
        new Color32(133, 187, 253, 255), // blue theme
        new Color32(236, 185, 126, 255), // orange theme
        new Color32(236, 163, 137, 255), // light red theme
        new Color32(192, 162, 243, 255), // purple theme
        new Color32(236, 168, 222, 255), // pink theme
        new Color32(221, 204, 168, 255) // default theme
    };

    private readonly Color32[] _leftRightPurchaseColors =
    {
        new Color32(110, 110, 110, 255), // dark theme
        new Color32(200, 189, 106, 255), // gold theme
        new Color32(158, 198, 85, 255), // light green theme
        new Color32(109, 187, 195, 255), // light blue theme
        new Color32(81, 154, 234, 255), // blue theme
        new Color32(204, 114, 18, 255), // orange theme
        new Color32(207, 115, 81, 255), // light red theme
        new Color32(143, 98, 227, 255), // purple theme
        new Color32(215, 103, 177, 255), // pink theme
        new Color32(195, 153, 109, 255) // default theme
    };

    private readonly Color32[] _shopHeaderColors =
    {
        new Color32(77, 77, 77, 255), // dark theme
        new Color32(109, 92, 0, 255), // gold theme
        new Color32(43, 73, 6, 255), // light green theme
        new Color32(0, 84, 82, 255), // light blue theme
        new Color32(0, 51, 104, 255), // blue theme
        new Color32(125, 55, 0, 255), // orange theme
        new Color32(142, 43, 7, 255), // light red theme
        new Color32(53, 16, 120, 255), // purple theme
        new Color32(111, 19, 80, 255), // pink theme
        new Color32(108, 58, 0, 255) // default theme
    };

    private readonly Color32[] _shopInfoColors =
    {
        new Color32(94, 94, 94, 255), // dark theme
        new Color32(142, 119, 0, 255), // gold theme
        new Color32(45, 94, 0, 255), // light green theme
        new Color32(0, 101, 108, 255), // light blue theme
        new Color32(39, 84, 137, 255), // blue theme
        new Color32(137, 70, 0, 255), // orange theme
        new Color32(164, 62, 25, 255), // light red theme
        new Color32(74, 34, 147, 255), // purple theme
        new Color32(135, 0, 89, 255), // pink theme
        new Color32(89, 61, 2, 255) // default theme
    };

    private readonly Color32[] _dotsColors =
    {
        new Color32(200, 200, 200, 150), // dark theme
        new Color32(255, 247, 146, 150), // gold theme
        new Color32(146, 200, 47, 150), // light green theme
        new Color32(102, 185, 195, 150), // light blue theme
        new Color32(81, 151, 234, 150), // blue theme
        new Color32(219, 131, 38, 150), // orange theme
        new Color32(217, 116, 79, 150), // light red theme
        new Color32(144, 95, 236, 150), // purple theme
        new Color32(224, 99, 182, 150), // pink theme
        new Color32(195, 158, 102, 150) // default theme
    };

    // ******COLORS FOR SLOT******
    private readonly Color32[] _headerImageColors =
    {
        new Color32(133, 133, 133, 255), // dark color
        new Color32(222, 210, 0, 255), // gold color
        new Color32(181, 221, 0, 255), // light green color
        new Color32(139, 214, 221, 255), // light blue color
        new Color32(101, 148, 226, 255), // blue color
        new Color32(255, 165, 0, 255), // orange color
        new Color32(245, 126, 82, 255), // light red color
        new Color32(180, 111, 231, 255), // purple color
        new Color32(230, 146, 243, 255), // pink color
        new Color32(219, 198, 136, 255) // default color
    };

    private readonly Color32[] _trialColors =
    {
        new Color32(255, 125, 0, 150), // for dots
        new Color32(255, 165, 0, 255) // for menu, leftRight, purchase buttons
    };

    private readonly Color32[] _promotionColors =
    {
        new Color32(222, 72, 20, 255), // for menu image
        new Color32(255, 133, 95, 190), // for shop panel image
        new Color32(99, 8, 0, 255), // for shop header text
        new Color32(123, 24, 0, 255), // for shop info text
        new Color32(207, 53, 0, 255), // for purchase button
        new Color32(255, 143, 0, 255) // for purchase text
    };

    private byte _colorAccessor = 0;
    private string[] _slotForm = { "dark", "gold", "lightGreen", "lightBlue", "blue", "orange", "red", "purple", "pink" };
    public static string[] ThemeName { get; } = { "Dark", "Gold", "Matcha", "Sky", "Ocean", "Marigold", "Peach", "Lavender", "Fuchsia" };

    // Declared as static to save value after scene is reloaded
    public static byte CurSlot { get; set; } = 0;
    private static byte _previousSlot = 0;

    // For Menu Button to blink
    public static bool IsBlinking { get; private set; } = false;
    public static bool HasEntered { private get; set; } = false;
    public static bool CanBlink { private get; set; } = true;
    private Color32 _targetColor;
    private float _blinkTime = 0.275f;
    // If don't want it to blink when game is restarted, removed CanBlink to true at GameEnded method in GameState

    private const int _dMax = 350;
    // Declared here cuz this will get manage here often
    public static string[] ThemeTrial { get; set; } = { "no", "no", "no", "no", "no", "no", "no", "no", "no" };
    public static int[] ThemeTrialTask { get; private set; } = { _dMax, 100, 100, 80, 80, 60, 60, 40, 40 }; // size of this being use is for loop in GamePlay & GameSocial
    private static int _revived = 0;
    private static int _revivedTemp = 0; // make RefreshTask run faster
    private int[] _revivePlan = { 300, 250, 200 }; // need more revive plan (1 revive at 300), (2 at 250), (3 at 200)// Static for method use
    private byte _deductAmount = 10;
    private byte _left = 0;
    private bool _reached = false;

    // Declared here cuz we can make sure that this will be loaded before start looping in Start Method
    public static string CurTheme { get; private set; } = "default";
    // now no other class use but in the future SecondPurchaseButton could use to save

    public static GameTheme Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        // Load curTheme from save file (If there is a save) // get saved in secondPurchaseButton, GameState
        if (GameData.IsFileExist("CurTheme") && TrialButton.TrialStarted == false) // not load when trial mode
        {
            CurTheme = GameData.LoadString("CurTheme");
        }

        // Load ThemeTrial from save file (If there is a save) // get saved in secondPurchaseButton, GamePlay
        if (GameData.IsFileExist("ThemeTrial"))
        {
            ThemeTrial = GameData.LoadStringArray("ThemeTrial");
        }

        // Load Revived from save file (If there is a save) // get saved in GameAds, ReviveButton by Increse method
        if (GameData.IsFileExist("Revived") && ThemeTrialTask[0] == _dMax)
        {
            _revived = int.Parse(GameData.LoadString("Revived"));

            _revivedTemp = _revived;
        }

        // Refresh DarkTheme Task after Revived is loaded
        RefreshTask();

        // Reassign slotColors cuz one of this may get being use by player
        if (CurTheme != "default")
        {
            for (byte a = 0; a < _slotForm.Length; a++)
            {
                if (_slotForm[a] == CurTheme)
                {
                    _slotForm[a] = "default";
                    break;
                }
            }
        }

        // Refresh UI first cuz dots need to set alpha later
        RefreshAllUI();
        RefreshCurSlot();
    }

    public void RefreshAllUI()
    {
        _colorAccessor = (byte)(_headerColors.Length - 1); // All array colors need to have the same size

        if (CurTheme != "default")
        {
            for (byte a = 0; a < _slotForm.Length; a++)
            {
                if (_slotForm[a] == "default")
                {
                    _colorAccessor = a;
                    break;
                }
            }
        }

        // access specific color item in an array
        _mainCamera.backgroundColor = _mainCameraColors[_colorAccessor];

        // get alpha from old color
        float oldAlpha = _gameStatePanelImage.color.a;

        _gameStatePanelImage.color = _gameStatePanelColors[_colorAccessor];

        // assign alpha to new color (Can't call method from GameState to change cuz it use old color alpha not updated one)
        var tempAlpha = _gameStatePanelImage.color;
        tempAlpha.a = oldAlpha;
        _gameStatePanelImage.color = tempAlpha;


        // get alpha from old color
        oldAlpha = _playGameBackgroundImage.color.a;

        _playGameBackgroundImage.color = _gameStatePanelColors[_colorAccessor];

        // assign alpha to new color (cuz it is 0 from colors array)
        tempAlpha = _playGameBackgroundImage.color;
        tempAlpha.a = oldAlpha;
        _playGameBackgroundImage.color = tempAlpha;


        // get alpha from old color
        oldAlpha = _scorePanelImage.color.a;

        _scorePanelImage.color = _gameStatePanelColors[_colorAccessor];

        // assign alpha to new color (cuz it is 0 from colors array)
        tempAlpha = _scorePanelImage.color;
        tempAlpha.a = oldAlpha;
        _scorePanelImage.color = tempAlpha;


        _headerText.color = _headerColors[_colorAccessor];
        _subHeaderText.color = _subHeaderColors[_colorAccessor];

        _scoreText.color = _headerColors[_colorAccessor];

        // Manage subHeader text and width (this script have to run after GamePlay to check for highScore values)
        if (GamePlay.HighScoreE != 0 || GamePlay.HighScoreT != 0)
        {
            if (GamePlay.HighScoreT != 0)
            {
                _subHeaderRect.sizeDelta = new Vector2(300f, 5f);

                _subHeaderText.text = $"<size=25>BEST\n<color=#{_subHeaderRichColors[_colorAccessor]}>endless {GamePlay.HighScoreE}</color>, timer {GamePlay.HighScoreT}</size>";
            }
            else
            {
                _subHeaderText.text = $"<size=25>BEST\n<color=#{_subHeaderRichColors[_colorAccessor]}>endless {GamePlay.HighScoreE}</color></size>";
            }
        }
        else
        {
            _subHeaderText.text = $"<color=#{_subHeaderRichColors[_colorAccessor]}>no</color> Best";
        }

        _typeInfoText[0].color = _headerColors[_colorAccessor];
        _typeInfoText[1].color = _subHeaderColors[_colorAccessor];

        _modeInfoText[0].color = _headerColors[_colorAccessor];
        _modeInfoText[1].color = _subHeaderColors[_colorAccessor];


        for (byte i = 0; i < 3; i++)
        {
            _gameText[i].color = _gameTextColors[_colorAccessor];
            _menuPanelImage[i].color = _menuPanelColors[_colorAccessor];

            if (ModeUI.IsComingType())
                _modePanelImage[i].color = _style2Colors[_colorAccessor];
            else if (ModeUI.NotUnlockedTaskT())
                _modePanelImage[i].color = _style1Colors[_colorAccessor];
            else
                _modePanelImage[i].color = GamePlay.GameType % 2 != 0 ? _style1Colors[_colorAccessor] : _style2Colors[_colorAccessor];

            _typePanelImage[i].color = _groundColors[_colorAccessor];

            // for Mode Indicator Text Under each Mode
            _modeInfoText[i + 2].color = _subHeaderColors[_colorAccessor];
        }

        _groundImage.color = _groundColors[_colorAccessor];

        for (byte i = 0; i < 2; i++)
        {
            _messageText[i].color = _headerColors[_colorAccessor];
            _socialImage[i].color = _style1Colors[_colorAccessor];
            _leftRightImage[i].color = _leftRightPurchaseColors[_colorAccessor];
            _purchaseImage[i].color = _leftRightPurchaseColors[_colorAccessor];
            _shopHeaderText[i].color = _shopHeaderColors[_colorAccessor];
            _shopInfoText[i].color = _shopInfoColors[_colorAccessor];

            GamePlay.Instance.QuestionImages[i].color = _gameTextColors[_colorAccessor];
        }

        foreach (Image eachDot in _dotsImage)
        {
            eachDot.color = _dotsColors[_colorAccessor];
        }

        // Set colors for answer buttons & main camera background, particles
        GamePlay.AnsColorAccessor = _colorAccessor;

        GameState.CameraColorAccessor = _colorAccessor;
        GameState.PartiColorAccessor = _colorAccessor;
        
        // Not run when player just pressed Try Now but will run when player start to explore things
        if (TrialButton.TrialStarted == false || TrialButton.ExploreStarted == true)
        {
            // Set it to default color so it won't get blink if no trial or promotion
            _targetColor = _menuPanelColors[_colorAccessor];
            
            // Set Color at MenuButtons & Dots for FreeTrial
            for (byte i = 0; i < ThemeTrial.Length; i++)
            {
                if (ThemeTrial[i] == "yes" && SecondPurchaseButton.HaveTheme[i] == false)
                {
                    _dotsImage[i].color = _trialColors[0];

                    _targetColor = _trialColors[1];
                }
            }
            // Set Color at MenuButtons & All first shop UI for Promotion
            if (GamePromotion.CurShowing != -1)
            {
                _targetColor = _promotionColors[0];
                _shopPanelImage.color = _promotionColors[1];
                _shopHeaderText[0].color = _promotionColors[2];
                _shopInfoText[0].color = _promotionColors[3];
                _purchaseImage[0].color = _promotionColors[4];
                _purchaseText[0].color = _promotionColors[5];
            }
            // Also check _shopPanelImage cuz if not this override trial color above (when have trial but no promotion)
            else if (GamePromotion.CurShowing == -1 && _shopPanelImage.color.Equals(_promotionColors[1]))
            {
				// Only Set blink color to normal & stop it when No Trial
				if (_targetColor.Equals(_menuPanelColors[_colorAccessor]))
				{
					_targetColor = _menuPanelColors[_colorAccessor];

                    if (IsBlinking)
                        HasEntered = true;
				}

                // set back to that theme color
                _shopPanelImage.color = new Color32(238, 238, 238, 190);
                _shopHeaderText[0].color = _shopHeaderColors[_colorAccessor];
                _shopInfoText[0].color = _shopInfoColors[_colorAccessor];
                _purchaseImage[0].color = _leftRightPurchaseColors[_colorAccessor];
                _purchaseText[0].color = Color.white;
            }
            
            // Blinking when target colors is not normal AND not blinking AND can blink
            if (!_targetColor.Equals(_menuPanelColors[_colorAccessor]) && !IsBlinking && CanBlink)
            {
				IsBlinking = true;
                CanBlink = false;

                StartCoroutine(BlinkButton());
            }
            // Set back to its color cuz when this method get call the normal color will override Trial or Promo
            else if(!_targetColor.Equals(_menuPanelColors[_colorAccessor]) && !IsBlinking && !CanBlink)
            {
                _menuPanelImage[1].color = _targetColor;
            }
        }
    }

    private IEnumerator BlinkButton()
    {
        while (HasEntered == false)
        {
            _menuPanelImage[1].color = _menuPanelColors[_colorAccessor];

            yield return new WaitForSeconds(_blinkTime);

            _menuPanelImage[1].color = _targetColor;

            yield return new WaitForSeconds(_blinkTime);
        }

        // Reset stuff to get ready for next blinking
        IsBlinking = false;
        HasEntered = false;
        // Reset here also cuz when changing between promotion and trial this will remove 1 extra canBlink
        CanBlink = false;

        yield break;
    }

    public void RefreshCurSlot()
    {
        // When default came (no difference between HAVE or HAVE NOT bought)
        if (_slotForm[CurSlot] == "default" && TrialButton.TrialStarted == false)
        {
            _shopHeaderText[1].text = "Default Theme";
            _shopHeaderImage.color = _headerImageColors[_headerImageColors.Length - 1];
            _shopBorderImages[0].color = _headerImageColors[_headerImageColors.Length - 1];
            _shopBorderImages[1].color = _headerImageColors[_headerImageColors.Length - 1];

            SetShopInfoRect(0f);
            _shopInfoText[1].text = "~ Your default theme.";

            SetTrialImageText(false);

            PurchaseButton.interactable = true;
            _purchaseText[1].text = "equip";
            _purchaseImage[1].overrideSprite = _purchaseSprites[0];
        }
        // Normal Themes ex. Dark, Gold, Matcha, ...
        else
        {
            // HeaderText & HeaderImage (no difference between HAVE or HAVE NOT bought or Trial)
            _shopHeaderText[1].text = ThemeName[CurSlot] + " Theme";
            _shopHeaderImage.color = _headerImageColors[CurSlot];
            _shopBorderImages[0].color = _headerImageColors[CurSlot];
            _shopBorderImages[1].color = _headerImageColors[CurSlot];

            // IF player HAVE NOT bought the current slot
            if (SecondPurchaseButton.HaveTheme[CurSlot] == false)
            {
                if (CurSlot == 0)
                {
                    SetShopInfoRect(0f);
                    _shopInfoText[1].text = "~ FREE! after scored " + ThemeTrialTask[CurSlot] + " in Endless mode." +
                    (!_reached ? "\n<size=16>(revive " + _left + " more -> " + (ThemeTrialTask[0] - _deductAmount) + ")</size>" : "");

                    SetTrialImageText(false);
                }
                else
                {
                    SetTrialImageText(true);

                    SetShopInfoRect(8f);
                    switch (ThemeTrial[CurSlot])
                    {
                        case "no":
                            _shopInfoText[1].text = "~ New Color Experience!\n(scored " + ThemeTrialTask[CurSlot] + " to TRY)";
                            //_shopInfoText[1].text = "~ TRY this! after scored " + ThemeTrialTask[CurSlot] + " in Endless mode.";

                            GameAd.Instance.ReviveTrialButton[1].interactable = false;
                            break;
                        case "yes":
                            _shopInfoText[1].text = "~ New Color Experience!\n(TRY now!)";
                            //_shopInfoText[1].text = "~ You can now TRY this ONCE!";

                            GameAd.Instance.ReviveTrialButton[1].interactable = true;
                            break;
                        case "tried":
                            _shopInfoText[1].text = "~ New Color Experience!\n(TRY more?)";
                            //_shopInfoText[1].text = "~ Hope you like this colorful theme!";

                            GameAd.Instance.ReviveTrialButton[1].interactable = true;
                            break;
                    }
                }
                    
                PurchaseButton.interactable = GameIAP.Instance.IsInitialized();
                _purchaseText[1].text = GameIAP.Instance.GetPrice((byte)(CurSlot + 1));
                _purchaseImage[1].overrideSprite = GameIAP.Instance.IsInitialized() ? _purchaseSprites[0] : _purchaseSprites[1];
            }
            // IF player HAVE bought the current slot
            else if (SecondPurchaseButton.HaveTheme[CurSlot] == true)
            {
                SetShopInfoRect(0f);
                _shopInfoText[1].text = "~ Your " + ThemeName[CurSlot] + " theme.\nEnjoy!";

                SetTrialImageText(false);

                PurchaseButton.interactable = true;
                _purchaseText[1].text = "equip";
                _purchaseImage[1].overrideSprite = _purchaseSprites[0];
            }
        }


        // Set back arrow button & Manage purchse button here
        if (ThemeTrial[CurSlot] == "yes" && SecondPurchaseButton.HaveTheme[CurSlot] == false)
        {
            _leftRightImage[0].color = _leftRightPurchaseColors[_colorAccessor];
            _leftRightImage[1].color = _leftRightPurchaseColors[_colorAccessor];

            // Set SecondShopPanel Animation to blinking at GameState in Update
        }
        // For set back trial color at button to normal one
        //else { ... }

        // make arrow button as indicator
        for (byte i = 0; i < ThemeTrial.Length; i++)
        {
            if (ThemeTrial[i] == "yes" && SecondPurchaseButton.HaveTheme[i] == false)
            {
                if (i > CurSlot)
                {
                    _leftRightImage[1].color = _trialColors[1];
                    break; // just want to know first more than, cuz the rest of this loop will loop up so doesn't matter to know the rest
                }
                if (i < CurSlot)
                {
                    _leftRightImage[0].color = _trialColors[1];
                }
            }
        }


        // Disable previous dot & size
        var tempAlpha = _dotsImage[_previousSlot].color;
        tempAlpha.a = 0.58823529f; // 150f
        _dotsImage[_previousSlot].color = tempAlpha;

        _dotsRect[_previousSlot].sizeDelta = new Vector2(5, 5);

        // Enable current dot & size
        tempAlpha = _dotsImage[CurSlot].color;
        tempAlpha.a = 1f;
        _dotsImage[CurSlot].color = tempAlpha;

        _dotsRect[CurSlot].sizeDelta = new Vector2(7.5f, 7.5f);


        // Checking for interactable of left right buttons
        if (CurSlot == 0)
        {
            _leftRightButtons[0].interactable = false;
        }
        else if (CurSlot == _slotForm.Length - 1)
        {
            _leftRightButtons[1].interactable = false;
        }
        else if (CurSlot == 1 && _previousSlot == 0)
        {
            _leftRightButtons[0].interactable = true;
        }
        else if (CurSlot == _slotForm.Length - 2 && _previousSlot == _slotForm.Length - 1)
        {
            _leftRightButtons[1].interactable = true;
        }


        // Update previous slot
        _previousSlot = CurSlot;
    }

    private void SetShopInfoRect(float height)
    {
        var temp = _shopInfoRect.anchoredPosition;
        temp.y = height;
        _shopInfoRect.anchoredPosition = temp;
    }
    private void SetTrialImageText(bool status)
    {
        _trialImage.enabled = status;
        _trialText.enabled = status;

        // When false image get closed, need to set back interactable cuz if not it won't get refresh when image is opened
        if (status == false)
            GameAd.Instance.ReviveTrialButton[1].interactable = true;
    }

    // This method manage CurTheme with each slotForm
    public void UpdateCurTheme()
    {
        if (CurTheme == "default" || (CurTheme != "default" && _slotForm[CurSlot] == "default"))
        {
            string temp = _slotForm[CurSlot];

            _slotForm[CurSlot] = CurTheme;

            CurTheme = temp;
        }
        // when current theme isn't default & current color in slotForm isn't default
        else
        {
            // Loop to check for default in slotForm & assign with current theme
            for (byte a = 0; a < _slotForm.Length; a++)
            {
                if (_slotForm[a] == "default")
                {
                    _slotForm[a] = CurTheme;
                    break;
                }
            }

            // Assign current theme with current color in slotForm
            CurTheme = _slotForm[CurSlot];

            // Assign current color in slotForm with default
            _slotForm[CurSlot] = "default";
        }
    }

    // This method restore back to default theme Doesn't depend on slot you are on (Use when explore is done)
    public void ToDefaultTheme()
    {
        for (byte a = 0; a < _slotForm.Length; a++)
        {
            if (_slotForm[a] == "default")
            {
                _slotForm[a] = CurTheme;
                break;
            }
        }

        CurTheme = "default";
    }

    // These 2 methods use for Revived score
    public static void IncreseRevived()
    {
        _revived++;
        _revivedTemp++;

        // Save Revived to save file
        GameData.SaveString(_revived.ToString(), "Revived");
    }
    
    public void RefreshTask()
    {
        if (SecondPurchaseButton.HaveTheme[0] == false)
        {
            for (byte a = 0; a < _revivePlan.Length; a++)
            {
                _left = (byte)(a + 1);

                for (int b = ThemeTrialTask[0]; b > _revivePlan[a]; b -= _deductAmount)
                {
                    _revivedTemp -= a + 1;

                    if (_revivedTemp < 0)
                    {
                        _left = (byte)Mathf.Abs(_revivedTemp);
                        _revivedTemp = a + 1 - _left;
                        return;
                    }

                    ThemeTrialTask[0] -= _deductAmount;
                }
            }
            _reached = true;
        }
    }

    // Method for GameTutor to set MenuButton color
    public void ManageMenuColor(sbyte nToRemove, Color32 colorToSet, sbyte nToSet)
    {
        if (nToRemove != -1)
            _menuPanelImage[nToRemove].color = _menuPanelColors[_menuPanelColors.Length - 1];

        if (nToSet != -1)
            _menuPanelImage[nToSet].color = colorToSet;

        // Reset all when both equal -1
        if (nToRemove == -1 && nToSet == -1)
        {
            for (byte i = 0; i < 3; i++)
                _menuPanelImage[i].color = _menuPanelColors[_menuPanelColors.Length - 1];
        }
    }

    // Method for GroundUI to call
    public IEnumerator GroundBlink(byte amount, float blinkTime, Color32 colorToBlink)
    {
        // Wait for Get In animation to finish
        yield return new WaitForSeconds(0.8f);

        for (byte i = 0; i < amount; i++)
        {
            _groundImage.color = colorToBlink;

            yield return new WaitForSeconds(blinkTime);

            _groundImage.color = _groundColors[_colorAccessor];

            yield return new WaitForSeconds(blinkTime);

            if (GroundUI.HasClicked == true)
                break;
        }

        yield break;
    }
}
