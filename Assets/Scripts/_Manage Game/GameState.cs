using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameState : MonoBehaviour
{
    [SerializeField]
    private Text _subHeaderText;
    [SerializeField]
    private Text _instructionText;

    // Changes according to the theme
    [SerializeField]
    private Image _gameStatePanelImage;
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private Renderer[] _partiRenderer; // red, orange, light blue, light green

    // For Animators in GamePlay
    [SerializeField]
    private Animator _instructionAnimator;
    [SerializeField]
    private Animator _gridAnimator;
    [SerializeField]
    private Animator _revivePanelAnimator;
    [SerializeField]
    private Animator _ansButtonsAnimator;

    // For Animators in MenuPanel
    [SerializeField]
    private Animator[] _topTextImageAnimators;
    [SerializeField]
    private Animator _menuPanelAnimator;
    [SerializeField]
    private Animator _settingPanelAnimator;
    [SerializeField]
    private Animator[] _shopPanelsAnimator;
    [SerializeField]
    private Animator _creditsPanelAnimator;
    [SerializeField]
    private Animator _supportPanelAnimator;
    [SerializeField]
    private Animator _readMePanelAnimator;

    // Colors for background (darker)
    private readonly Color32[] _mainCameraDarkerColors =
    {
        new Color32(55, 54, 63, 255), // dark theme
        new Color32(255, 252, 155, 255), // gold theme
        new Color32(216, 246, 138, 255), // light green theme
        new Color32(182, 248, 255, 255), // light blue theme
        new Color32(180, 212, 255, 255), // blue theme
        new Color32(255, 216, 156, 255), // orange theme
        new Color32(255, 194, 173, 255), // light red theme
        new Color32(207, 187, 243, 255), // purple theme
        new Color32(253, 188, 231, 255), // pink theme
        new Color32(255, 236, 182, 255) // default theme
    };

    // Color that will lerping back (lighter)
    private Color32 _mainCameraLighterColor;

    // This number will get changes by GameTheme
    public static byte CameraColorAccessor { get; set; } = 9; // this number is the last element in array

    // Colors for particles to lerp
    private readonly Color32[] _partiDarkerColors =
    {
        new Color32(39, 39, 39, 255), // dark theme
        new Color32(225, 227, 108, 255), // gold theme
        new Color32(166, 186, 90, 255), // light green theme
        new Color32(138, 210, 205, 255), // light blue theme
        new Color32(126, 157, 200, 255), // blue theme
        new Color32(186, 152, 94, 255), // orange theme
        new Color32(207, 125, 95, 255), // light red theme
        new Color32(163, 135, 215, 255), // purple theme
        new Color32(231, 121, 194, 255), // pink theme
        new Color32(204, 184, 121, 255) // default theme
    };

    // Colors for particles
    private readonly Color32[] _partiColors =
    {
        new Color32(207, 52, 23, 255), // red
        new Color32(255, 138, 0, 255), // orange
        new Color32(0, 119, 255, 255), // light blue
        new Color32(133, 164, 0, 255), // light green
        new Color32(255, 255, 255, 255) // white
    };

    // This number will get changes by GameTheme
    public static byte PartiColorAccessor { get; set; } = 9; // this number is the last element in array

    // For GameState Panel Location
    public static string Location { get; set; } = "toBegin";

    // For Counting at revive button
    private float _skipDelay = 3f;
    private float _secondReviveDelay = 1f;
    public static float SkipTimer { get; set; } = 0f;

    // For Counting at particles & stop lerping
    private float _partiChangesDelay = 0f;
    private float _partiChangesTimer = 0f;

    // LerpingTimer1Delay no need to use field cuz no modification its time is 2 sec
    public static float LerpingTimer2Delay { private get; set; } = 1.5f;

    private float _lerpingTimer1 = 0f;
    private float _lerpingTimer2 = 0f;
    private string _lerpsInfo = "allowLerping";

    // For opening begin text
    private float _beginTextDelay = 0.75f;
    private float _beginTextTimer = 0f;

    // For explore time
    private byte _exploreTime = 20;

    // For Other Class To Use
    public static bool IsGameStarted { get; set; } = false;
    public static bool IsGameEnded { get; set; } = false;
    public static bool Revive { get; set; } = false;

    public static GameState Instance { get; private set; }
    private void Awake() => Instance = this;

    // This script has to run before game promotion cuz it use IsGameStarted to check
    private void Start()
    {
        // Set Target FrameRate of the mobile to maximum
        Application.targetFrameRate = 60;

        // Set Static Member back to default
        Location = "toBegin";

        LerpingTimer2Delay = 1.5f;

        IsGameStarted = false;
        IsGameEnded = false;
        Revive = false;

        SkipTimer = 0f;

        if (TrialButton.ExploreStarted == true)
        {
            StartCoroutine( EndExploreMenu() );
        }
    }
	
    private void Update()
    {
        // opening begin text (have to declare if alone here cuz both of these will run at the same time)
        if (IsGameStarted == false && _beginTextTimer <= _beginTextDelay)
        {
            _beginTextTimer = _beginTextTimer + Time.deltaTime;

            if (_beginTextTimer >= _beginTextDelay && Location == "toBegin" && TrialButton.ExploreStarted == false)
            {
                MessageUI.Instance.OpenMessages("all (Get In)");
            }
        }

        // set particles to color
        if (IsGameStarted == false && Location == "toBegin" && _lerpsInfo == "allowLerping")
        {
            if (_partiChangesTimer >= _partiChangesDelay)
            {
                _lerpingTimer1 = _lerpingTimer1 + Time.deltaTime;

                for (int i = 0; i < _partiRenderer.Length; i++)
                {
                    _partiRenderer[i].material.color = Color.Lerp(_partiRenderer[i].material.color, _partiColors[i], 0.04f);
                }

                // This will block code to continue lerping
                if (_lerpingTimer1 >= 2f)
                {
                    _lerpsInfo = "finished 1";
                }
            }
            else
            {
                _partiChangesTimer = _partiChangesTimer + Time.deltaTime;
            }
        }
        // set particles to white - when open setting or shop
        else if (IsGameStarted == false && Location == "toMenu" && (_lerpsInfo == "allowLerping" || _lerpsInfo == "finished 1") )
        {
            _lerpingTimer2 = _lerpingTimer2 + Time.deltaTime;

            for (int i = 0; i < _partiRenderer.Length; i++)
            {
                _partiRenderer[i].material.color = Color.Lerp(_partiRenderer[i].material.color, _partiColors[4], 0.065f);
            }

            // This will block code to continue lerping
            if (_lerpingTimer2 >= LerpingTimer2Delay)
            {
                _lerpsInfo = "finished 2";
            }
        }
        else if (Revive == true)
        {
            // Enter Once
            if(_revivePanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("RevivePanel Idle"))
            {
                // Play revivePanel animation Get In
                _revivePanelAnimator.Play("RevivePanel Get In", -1, 0f);

                // Play answerButtons animation Move Down
                _ansButtonsAnimator.Play("AnswerButtonsPanel Move Down", -1, 0f);

                // Close #PauseGamePanel 
                PausePlayGameButton.Instance.ClosePausePanel();
            }

            // Start Counting (in 'else' cuz it no need to plus once it is reached the delay)
            if(SkipTimer >= _skipDelay)
            {
                // Enter Once
                if(_instructionAnimator.GetCurrentAnimatorStateInfo(0).IsName("InstructionText Idle"))
                {
                    // Play Instruction Text Skipping & Set its text
                    _instructionAnimator.Play("InstructionText Skipping", -1, 0f);
                    _instructionText.text = "skip";
                }
            }
            else if(SkipTimer >= _secondReviveDelay && _revivePanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("RevivePanel Get In"))
            {
                // Play revivePanel animation base on current level score
                _revivePanelAnimator.Play(GamePlay.Level >= 11 ? "RevivePanel Third Idle" : "RevivePanel Second Idle", -1, 0f);
            }
            else
            {
                SkipTimer = SkipTimer + Time.deltaTime;
            }

            // Play second idle animation after Get In is finished

        }
        else if(IsGameEnded == true)
        {
            // For enter once (check for Idle is when Trial mode & Skipping is when NonTrial)
            if(_instructionAnimator.GetCurrentAnimatorStateInfo(0).IsName("InstructionText Idle") 
            || _instructionAnimator.GetCurrentAnimatorStateInfo(0).IsName("InstructionText Skipping"))
            {
                GameEnded();
            }
        }

        // Play second shopPanels idle animation after Get In is finished
        if(_shopPanelsAnimator[0].GetCurrentAnimatorStateInfo(0).IsName("FirstShopPanel Get In") && 
           _shopPanelsAnimator[0].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _shopPanelsAnimator[0].Play("FirstShopPanel Second Idle", -1, 0f);
        }
        if((_shopPanelsAnimator[1].GetCurrentAnimatorStateInfo(0).IsName("SecondShopPanel Get In") 
        || _shopPanelsAnimator[1].GetCurrentAnimatorStateInfo(0).IsName("SecondShopPanel Right Slide In") 
        || _shopPanelsAnimator[1].GetCurrentAnimatorStateInfo(0).IsName("SecondShopPanel Left Slide In") ) 
        && _shopPanelsAnimator[1].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            if (GameTheme.ThemeTrial[GameTheme.CurSlot] == "yes" && SecondPurchaseButton.HaveTheme[GameTheme.CurSlot] == false)
                _shopPanelsAnimator[1].Play("SecondShopPanel Third Idle", -1, 0f);
            else
                _shopPanelsAnimator[1].Play("SecondShopPanel Second Idle", -1, 0f);
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape) && ProcessingUI.Instance.IsClosed())
            PointerDown();
#endif
    }

    public void PointerDown()
    {
        // Player won't able to click anything when trial is true
        if ((TrialButton.TrialStarted == false || TrialButton.ExploreStarted == true) && GameTutor.TutorStarted == false && Location == "toBegin" && IsGameStarted == false && _beginTextTimer >= _beginTextDelay)
        {
            GameSfx.Instance.PlaySound(0);

            // Close Begin Text
            MessageUI.Instance.CloseMessages("all");

            // Close Menu Panel
            _menuPanelAnimator.Play("MenuPanel Close", -1, 0f);

            // Open TypePanel & TypeInfo Panel
            TypeUI.Instance.OpenTypePanel();
            StartCoroutine(TypeUI.Instance.OpenTypeInfoPanel());

            // Open GroundPanel
            GroundUI.Instance.OpenPanel("blink");

            // Update Type Info Text, no play sound
            TypeUI.Instance.Fetch();

#if !UNITY_EDITOR && UNITY_ANDROID
            ScoreCenterButton.Instance.CloseButton();
#endif

            LerpingTimer2Delay = 0.125f;

            // Set Location
            Location = "toMenu";
        }
        // Allow player to be able to tap skip
        else if (TrialButton.TrialStarted == false && Revive == true && SkipTimer >= _skipDelay)
        {
            GameSfx.Instance.PlaySound(1);

            // Check for highScoreBeated
            _subHeaderText.text = (GamePlay.CurHighScoreBeated == true) ? "<color=#EF7028>New Best!</color>" : "Game Over";

            // Play revivePanel animation Get Out
            _revivePanelAnimator.Play("RevivePanel Get Out", -1, 0f);

            // Set Back
            Revive = false;
            IsGameEnded = true;
        }
        else if (TrialButton.TrialStarted == false && IsGameEnded == true)
        {
            GameSfx.Instance.PlaySound(0);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (Location == "toModefromExtra" && IsGameStarted == false)
        {
            GameSfx.Instance.PlaySound(1);

            // Open Extra Button
            ExtraButton.Instance.OpenPanel();

            // Close ExtraShopPanel & Set Location & Set StatePanel (for gameIAP to use also)
            ExtraPurchaseButton.Instance.CloseAndSetBack();
        }
        else if (Location == "toType" && IsGameStarted == false)
        {
            GameSfx.Instance.PlaySound(1);

            // Set Alpha Back
            SetPanelAlpha(0f);
            // Set back StatePanel to default at ExtraButton script (code location not relevent to the term but just for sake)
            ExtraButton.Instance.StatePanelTo(0);

            // Open TypeInfo Panel
            StartCoroutine( TypeUI.Instance.OpenTypeInfoPanel() );

            // Only close when it is open
            ModeUI.Instance.ClosePanelAndInfo();
            ExtraButton.Instance.ClosePanel();

            // Open GroundPanel
            GroundUI.Instance.OpenPanel("blink");

            // Set Location
            Location = "toMenu";
        }
        else if (Location == "toMenu" && IsGameStarted == false)
        {
            GameSfx.Instance.PlaySound(1);

            // If Setting is OPEN Close SettingPanel with animation
            if (_settingPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("SettingPanel Get In"))
            {
                _settingPanelAnimator.Play("SettingPanel Idle", -1, 0f);
            }
            // If Shop is OPEN Close First & Second Shops with animation
            if (_shopPanelsAnimator[0].GetCurrentAnimatorStateInfo(0).IsName("FirstShopPanel Get In") ||
               _shopPanelsAnimator[0].GetCurrentAnimatorStateInfo(0).IsName("FirstShopPanel Second Idle"))
            {
                StartCoroutine(CloseShops());
            }
            // Only close when it is open
            TypeUI.Instance.CloseTypePanel();
            StartCoroutine( TypeUI.Instance.CloseTypeInfoPanel() );
            GroundUI.Instance.ClosePanel();
            ExtraButton.Instance.ClosePanel();

            // Open BeginText, MunuPanel AND assign alpha
            MessageUI.Instance.OpenMessages("all (Second Idle)");
            _menuPanelAnimator.Play("MenuPanel Get In", -1, 0f);
            SetPanelAlpha(0f);

            // Set back lerping things (doesn't set in Update cuz that else if block won't run every time)
            _partiChangesTimer = 0f;

            _lerpingTimer1 = 0f;
            _lerpingTimer2 = 0f;
            _lerpsInfo = "allowLerping";

            // Set Location
            Location = "toBegin";
        }
        else if (Location == "toSetting" && IsGameStarted == false)
        {
            GameSfx.Instance.PlaySound(1);

            // If Credits is OPEN Close CreditsPanel with animation
            if (_creditsPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("CreditsPanel Get In"))
            {
                _creditsPanelAnimator.Play("CreditsPanel Idle", -1, 0f);
            }
            // If Support is OPEN Close SupportPanel with animation
            if (_supportPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("SupportPanel Get In") ||
                _supportPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("SupportPanel Get In 2"))
            {
                _supportPanelAnimator.Play("SupportPanel Idle", -1, 0f);
            }
            // If ReadMe is OPEN Close ReadMePanel with animation
            if (_readMePanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ReadMePanel Get In"))
            {
                _readMePanelAnimator.Play("ReadMePanel Idle", -1, 0f);
            }

            // Open SettinPanel, AND assign alpha
            _settingPanelAnimator.Play("SettingPanel Get In", -1, 0f);
            SetPanelAlpha(185f);

            // Set Location
            Location = "toMenu";
        }
    }

    public void SetPanelAlpha(float input)
    {
        var tempAlpha = _gameStatePanelImage.color;
        tempAlpha.a = input / 255f;
        _gameStatePanelImage.color = tempAlpha;
    }

    private IEnumerator CloseShops()
    {
        // First close
        _shopPanelsAnimator[0].Play("FirstShopPanel Idle", -1, 0f);
        _shopPanelsAnimator[1].Play("SecondShopPanel Idle", -1, 0f);

        yield return new WaitForSeconds(0.03f);

        // Second close again (incase if in update still true)
        _shopPanelsAnimator[0].Play("FirstShopPanel Idle", -1, 0f);
        _shopPanelsAnimator[1].Play("SecondShopPanel Idle", -1, 0f);

        yield break;
    }

    public void GameFirstStarted(byte mode)
    {
        GamePlay.GameMode = mode;
        GameMusic.Instance.PlaySound(mode);

        // Set Alpha Back
        SetPanelAlpha(0f);
        // Set back StatePanel to default at ExtraButton script (code location not relevent to the term but just for sake)
        ExtraButton.Instance.StatePanelTo(0);
        
        // Open #PauseGamePanel 
        PausePlayGameButton.Instance.OpenPausePanel();

        // Only close when it is open
        TypeUI.Instance.CloseTypePanel();
        StartCoroutine(TypeUI.Instance.CloseTypeInfoPanel());
        ModeUI.Instance.ClosePanelAndInfo();
        GroundUI.Instance.ClosePanel();
        ExtraButton.Instance.ClosePanel();

        // Things to close Only in TrialMode
        if (TrialButton.TrialStarted == true)
        {
            // Close Begin Text
            MessageUI.Instance.CloseMessages("all");

            // Close Menu Panel
            _menuPanelAnimator.Play("MenuPanel Close", -1, 0f);
        }

        // Open the Grid
        _gridAnimator.Play("Grid1Image Get In", -1, 0f);

        // Save lighter color & Lerp MainCamera Background to darker
        _mainCameraLighterColor = _mainCamera.backgroundColor;
        StartCoroutine( CameraLerpTo(_mainCameraDarkerColors[CameraColorAccessor]) );

        // Play particles animator to Darker
        StartCoroutine( PartiLerpTo(_partiDarkerColors[PartiColorAccessor]) );

        // Set HasEntered when blinking is started (Need to remove old IEnumerator first)
        if (GameTheme.IsBlinking)
            GameTheme.HasEntered = true;
        // Set JustOpen to false so Normal Message would show up
        if (MessageUI.JustOpen)
            MessageUI.JustOpen = false;

        // Set IsGameStarted
        IsGameStarted = true;
    }

    private void GameEnded()
    {
        GameMusic.Instance.PlaySound(0);

        // Play Instruction Text Ending & Set its text
        _instructionAnimator.Play("InstructionText Ending", -1, 0f);

        // Play AnswerButtons Get Out
        _ansButtonsAnimator.Play("AnswerButtonsPanel Get Out", -1, 0f);

        // Lerp MainCamera Background to lighter
        StartCoroutine( CameraLerpTo(_mainCameraLighterColor) );

        // Play particles animator to idle
        StartCoroutine( PartiLerpTo(_partiColors[4]) );

        // Set to make it blink when start next game (Can't set in same place as HasEntered cuz of IEnumerator endding time)
        GameTheme.CanBlink = true;

        // Close #PauseGamePanel 
        PausePlayGameButton.Instance.ClosePausePanel();

        if (TrialButton.TrialStarted == false)
        {
            _instructionText.text = "Tap to Continue";

            GroundUI.Instance.SetShareSprite();
            GroundUI.Instance.OpenPanel("don't blink");

            SkipTimer = 0;
        }
        else
        {
            _instructionText.text = "Let's explore home menu!";

            StartCoroutine(StartExploreMenu());
        }

        // Can't set IsGameStarted to false here cuz in PointerUp call GameFirstStarted again!
    }

    private IEnumerator CameraLerpTo(Color colorTo)
    {
        float timer = 0f;

        while (timer < 1f)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            timer = timer + Time.deltaTime;

            _mainCamera.backgroundColor = Color.Lerp(_mainCamera.backgroundColor, colorTo, 0.075f);
        }
        yield break;
    }

    private IEnumerator PartiLerpTo(Color colorTo)
    {
        float timer = 0f;

        while (timer < 1.5f)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            timer = timer + Time.deltaTime;

            for (int i = 0; i < _partiRenderer.Length; i++)
            {
                _partiRenderer[i].material.color = Color.Lerp(_partiRenderer[i].material.color, colorTo, 0.085f);
            }
        }
        yield break;
    }

    // ************For Trial Theme***********

    public IEnumerator StartTrialGamePlay()
    {
        MessageUI.Instance.CloseMessages("messageAB");
        MessageUI.Instance.SetBeginText("Ready!");

        yield return new WaitForSeconds(2f);

        MessageUI.Instance.SetBeginText("Go!");

        yield return new WaitForSeconds(0.75f);

        GameFirstStarted(1);

        yield break;
    }

    private IEnumerator StartExploreMenu()
    {
        yield return new WaitForSeconds(4f);

        TrialButton.ExploreStarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield break;
    }

    private IEnumerator EndExploreMenu()
    {
        MessageUI.Instance.SetBeginText("Explore around! " + _exploreTime);
        MessageUI.Instance.SetMessageAB(0f, "\"Trial Mode\"");
        MessageUI.Instance.OpenMessages("all (Get In)");

        while (_exploreTime > 0)
        {
            // Code will run once here & Wait...
            yield return new WaitForSeconds(1f);
            // Until it met up & run here...

            // No need to let it update beginText again once explore is false
            if (TrialButton.ExploreStarted == false)
            {
                break;
            }

            _exploreTime--;
            MessageUI.Instance.SetBeginText("Explore around! " + _exploreTime);
        }

        // Cannot set begin text outside here cuz When Player Click Try Now again begin text will flash up

        // When the time is out
        if (_exploreTime <= 0)
        {
            // Set back theme to default
            GameTheme.Instance.ToDefaultTheme();

            // Save CurTheme to save file
            GameData.SaveString(GameTheme.CurTheme, "CurTheme");

            GameTheme.Instance.RefreshAllUI();
            GameTheme.Instance.RefreshCurSlot();

            // Set back Trial & Explore to false
            TrialButton.TrialStarted = false;
            TrialButton.ExploreStarted = false;
        }
        // When player click equip their theme OR time is out
        if (TrialButton.TrialStarted == false)
        {
            // Trial & Explore to false Get SET already in SecondPurchaseButton OR from above

            MessageUI.Instance.StartMessaging();
        }

        // Just check TrialStarted == true IF want to check when player try theme again
        yield break;
    }
}
