using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GamePlay : MonoBehaviour
{
    #region --Fields-- (Inspector)
    // For Managing Header,SubHeader
    [SerializeField]
    private Text _headerText;
    [SerializeField]
    private Text _subHeaderText;

    // For Managing Animations
    [SerializeField]
    private Animator[] _topTextImageAnimators;
    [SerializeField]
    private Animator _ansButtonsAnimator;
    [SerializeField]
    private Animator _gridAnimator;
    [SerializeField]
    private Animator _questionPanelAnimator;

    // For Managing Grid Colors
    [SerializeField]
    private List<Image> _firstImages = new List<Image>();
    [SerializeField]
    private List<Image> _secondImages = new List<Image>();
    [SerializeField]
    private List<Image> _thirdImages = new List<Image>();

    // For Checking Itself colors, Set Interactable
    [SerializeField]
    private Image[] _ansButtonsImages;
    [SerializeField]
    private Button[] _ansButtonsButton;


    // For Font that have ≠ & Sprite letter n & Manage Question Rect
    [SerializeField]
    private Font[] _font; // Pixel : 0 | Arial : 1
    [SerializeField]
    private Sprite _letterSprites;
    [SerializeField]
    private RectTransform[] _questionsRect;


    // For Managing QA - GameObjects (Animation must not close these gameObject in Idle)
    [SerializeField]
    private GameObject[] _questionImagesGameObject;
    [SerializeField]
    private GameObject _ansButtonGameObject;
    [SerializeField]
    private GameObject[] _ansButtonsTextGameObject;
    [SerializeField]
    private GameObject[] _ansPanelsImageGameObject;

    // For Managing QA - Color, Text (Don't Allow Set)
    [SerializeField]
    private Image[] _questionImages;
    public Image[] QuestionImages { get { return _questionImages; } }
    [SerializeField]
    private Text _questionText;
    public Text QuestionText { get { return _questionText; } }
    [SerializeField]
    private Image[] _ansPanelsImage;
    public Image[] AnsPanelsImage { get { return _ansPanelsImage; } }
    [SerializeField]
    private Text[] _ansButtonsText;
    public Text[] AnsButtonsText { get { return _ansButtonsText; } }
    #endregion


    #region --Fields-- (In Class)
    // List For Storing OpenedImages
    public static List<Image> OpenedImages { get; } = new List<Image>();

    // For Breaking the images
    // This list will not include the new images that is being assigns
    private List<int> _rangeOfNumbers = new List<int>();
    // This list will store the next level image (current = 1, this will have 2)
    private List<Image> _nextLevelImages = new List<Image>();
    private byte _openedCount = 0;


    // Colors for Buttons
    private readonly Color32[] _buttonColors =
    {
        new Color32(100, 100, 100, 255), // dark theme
        new Color32(203, 198, 106, 255), // gold theme
        new Color32(174, 202, 104, 255), // light green theme
        new Color32(122, 196, 198, 255), // light blue theme
        new Color32(119, 169, 212, 255), // blue theme
        new Color32(202, 150, 91, 255), // orange theme
        new Color32(219, 145, 119, 255), // light red theme
        new Color32(153, 116, 221, 255), // purple theme
        new Color32(224, 126, 189, 255), // pink theme
        new Color32(191, 175, 141, 255) // default theme
    };

    private readonly Color32[] _trueFalseColors = {
        new Color32(106, 203, 22, 255),  // Green
        new Color32(200, 25, 33, 255) // Red
    };

    // This number will get changes by GameTheme
    public static byte AnsColorAccessor { get; set; } = 9; // this number is the last element in array

    // Colors for Grids
    private readonly Color32[] _gridColors = {
        new Color32(44, 44, 44, 255), // black
        new Color32(150, 150, 150, 255), // gray
        new Color32(150, 94, 10, 255), // brown
        new Color32(74, 74, 255, 255), // blue
        new Color32(70, 240, 251, 255), // light blue
        new Color32(17, 123, 17, 255), // green
        new Color32(135, 219, 39, 255), // light green
        new Color32(246, 55, 55, 255), // red
        new Color32(255, 156, 35, 255), // orange
        new Color32(255, 227, 0, 255), // yellow
        new Color32(164, 16, 255, 255), // purple
        new Color32(255, 108, 239, 255) // pink
    };
    // Colors for Grids use for randoming
    public static Color32[] GridColorsManaged;

    // For Assign the correct button & answer button
    public static byte CorrectButton { private get; set; } = 0;
    public static byte AnsButton { get; set; } = 0;

    // For Timer
    private float _openDelay = 1.5f;
    private float _openTimer = 0f;

    private float _timeUpDelay = 5f; // this will get override in GameSetUp()
    private float _timeUpTimer = 0f;
    private float _countDownTimer = 0f;

    private float _endingDelay = 0.75f; // Must change value at Correct & Wrong IF Block
    private float _endingTimer = 0f;

    // For Checking
    private bool _isOpened = false;
    private bool _isNextLevel = true;
    public static bool IsRepeatLevel { get; set; } = false;
    private bool _enterCheck = false;

    // For Game Mode
    public static byte GameMode { get; set; } = 1;
    // For Game Type
    public static byte GameType { get; set; } = 1;
    public static byte MaxType { get; } = 4;
    public static bool HaveNewType { get; } = false; // can set to false

    // For Current Level & both highScore &  curHighScoreBeated
    public static int Level { get; private set; } = 0;
    public static bool CurHighScoreBeated { get; private set; } = false;
    public static int HighScoreE { get; set; } = 0;
    public static byte HighScoreT { get; set; } = 0;

    public static bool SkipFrame { get; set; } = false;

    public byte ReviveAmount { get; set; } = 3; // Didn't make static cuz its value need to set back
    // For notice when player use revive
    private bool _isRevived = false;

    // For Trial Mode
    private byte _maxLevel = 10;

    private bool _notFirstTime = false;

    // Correct words - Used to use when answer is correct Under CheckingAnswer() -> Under play sound -> "_subHeaderText.text = "<color=#84C110>" + _correctWords[Random.Range(0, _correctWords.Length)] + "</color>";"
    //private readonly string[] _correctWords = { "Correct!", "Nice!", "Awesome!", "You Got It!", "Well Done!", "Cool!" };

    // Central Place to manage things for each types to use (Polymorphism & Inheritance concept)
    private static readonly float[][] _minMax =
    {
        new float[] { 10f, 17f },
        new float[] { 8f, 20f },
        new float[] { 8f, 15f },
        new float[] { 8f, 15f }
    };

    private static readonly float[][] _timers =
    {
        new float[] { 6f, 75f, 8.3f, 9f },
        new float[] { 8f, 95f, 12f, 14f },
        new float[] { 7f, 90f, 11.3f, 13f },
        new float[] { 7f, 90f, 11.3f, 13f }
    };

    // Initialize all children to _mainType to use its method from its children
    private MainType[] _mainType =
    {
        new TypeOne(_timers[0], _minMax[0]),
        new TypeTwo(_timers[1], _minMax[1]),
        new TypeThree(_timers[2], _minMax[2]),
        new TypeFour(_timers[3], _minMax[3])
    };

    // For randoming gameType
    private byte _cur = 0;
    private byte _previous = 0;

    private bool _isMix = false;

    private byte _dupliMax = 1;
    private byte _dupliNum = 0;
    #endregion


    #region --Methods-- (Builtin)
    public static GamePlay Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        // Load HighScoreE from save file (If there is a save)
        if (GameData.IsFileExist("HighScoreE"))
        {
            HighScoreE = int.Parse(GameData.LoadString("HighScoreE"));
        }
        // Load HighScoreT from save file (If there is a save)
        if (GameData.IsFileExist("HighScoreT"))
        {
            HighScoreT = byte.Parse(GameData.LoadString("HighScoreT"));
        }

        // Set GameType to 0 when unlocked all Game Types
        if (HighScoreT >= ModeUI.TaskT || ExtraPurchaseButton.HavePlayNow)
        {
            GameType = 0;
            TypeUI.Instance.RefreshSprite();
        }

        // Clear Static list
        OpenedImages.Clear();
        // GridColorsManaged no need to set cuz it will get set in ManageGridColors();

        // Set Static Member back to default
        CorrectButton = 0;
        AnsButton = 0;
        IsRepeatLevel = false;

        GameMode = 1;
        // GameType no need to set back, its stuff don't have set

        CurHighScoreBeated = false;
        SkipFrame = false; // this will be true when ads is finished

        Level = 0;

        // Clear second and third level Grids
        foreach (Image img in _secondImages)
        {
            img.color = Color.clear;
        }

        foreach (Image img in _thirdImages)
        {
            img.color = Color.clear;
        }

        // Assign the default of OpenedImage
        foreach(Image img in _firstImages)
        {
            OpenedImages.Add(img);
        }
    }

    private void Update()
    {
        if (GameState.IsGameStarted == true)
        {
            // Call Once
            if (_isNextLevel == true)
            {
                // Set once player start game with this Game Type (Need If block to set isMix)
                if (GameType == 0)
                    _isMix = true;

                // It will stay zero when it's mix type
                GameScore.Instance.StarterGameType = _isMix ? (byte)0 : GameType;

                Random:
                if (_isMix)
                    GameType = (byte)Random.Range(1, MaxType + 1);

                _dupliNum = (byte)(GameType == _previous ? _dupliNum + 1 : 0);

                if (_dupliNum >= _dupliMax + 1)
                    goto Random;
                _previous = GameType;

                _cur = (byte)(GameType - 1);
                
                // notFirstTime will set to true in RefreshTheLevel SO it will run once
                if (_notFirstTime == false)
                {
                    ManageGridColors();
                    ManageQAUI();
                    if (_isMix == false)
                        ModeSetUp(_mainType[_cur].Timers[0], _mainType[_cur].Timers[1]);
                    else
                        ModeSetUp(_mainType[_cur].Timers[0], 85f);
                }
                else if (_isMix)
                {
                    SetBackQAUI();
                    ManageQAUI();
                    TimeSetUp(_mainType[_cur].Timers[0]); // After L.15, RefreshTheLevel will override timer value
                }

                _mainType[_cur].NextLevel();

                _isNextLevel = false;
            }
            else if (IsRepeatLevel == true)
            {
                GameMusic.Instance.PlaySound(GameMode); // When revive from ads or jr

                _mainType[_cur].RepeatLevel();

                IsRepeatLevel = false;
            }

            QuestionAndTimer();
            CheckingAnswer();
            UpdatingTopTextsAnimation();
        }
    }
    #endregion


    #region --Methods-- (Custom)
    public void RefreshTheLevel(string levelChecker, float changesE1, float changesE2)
    {
        // Set Things Back
        _isOpened = false;
        _openTimer = 0f;
        IndicatorUI.Instance.PlayIndicatorImageAnimation(IndicatorUI.IndicatorImageAnimator.Close);

        // Only reset timeUpTimer in mode 1,2
        if (GameMode == 1 || GameMode == 2)
        {
            _timeUpTimer = 0f;
        }
        _endingTimer = 0f;

        AnsButton = 0;

        // Set 3 buttons color back
        foreach (Image img in _ansButtonsImages)
        {
            img.color = _buttonColors[AnsColorAccessor];
        }

        // Make all buttons can iteract
        foreach (Button button in _ansButtonsButton)
        {
            button.interactable = true;
        }
        // **************************************

        Level += (levelChecker == "increase") ? 1 : 0;

        // This method will loop check the current level itself
        IndicatorUI.Instance.IndicateText();

        if (GameMode == 1 || GameMode == 2)
        {
            // Update header text
            _headerText.text = TrialButton.TrialStarted == false ? "Level " + Level : "Trial Level";

            // ***Update HighScoreE at CheckingAnswer

            // Set time for timeUpDelay (Set default value for mode 1 & 2 at ModeSetUp() or TimeSetUp())
            if (Level >= 25)
                _timeUpDelay = changesE2;
            else if (Level >= 15)
                _timeUpDelay = changesE1;

            // Update the subHeaderText with starter second (Order should be under Set Time above)
            _subHeaderText.text = _timeUpDelay.ToString("0.00") + " sec";
        }
        else if (GameMode == 3)
        {
            // Update headerText
            _headerText.text = "Level " + (Level - 9);

            // ***Update HighScoreT at CheckingAnswer

            // Update the subHeaderText with count down second
            _subHeaderText.text = _countDownTimer.ToString("0.00") + " sec";

            CloseQuestion(); // No need to close for Endless Mode cuz it will be closed when answer is correct
        }

        // Play TopTextImageAnimators Get In
        _topTextImageAnimators[0].Play("HeaderText Get In", -1, 0f);
        _topTextImageAnimators[1].Play("HeaderImage Get In", -1, 0f);
        _topTextImageAnimators[2].Play("SubHeaderText Get In", -1, 0f);
        _topTextImageAnimators[3].Play("SubHeaderImage Get In", -1, 0f);

        // Play AnswerAnimators Get In
        _ansButtonsAnimator.Play("AnswerButtonsPanel Get In", -1, 0f);

        // Play GridAnimator Get In
        if (_notFirstTime)
            _gridAnimator.Play("Grid1Image Pop Up", -1, 0f);
        else
            _notFirstTime = true;

        // Close ScoreText & ComboText when reload new level
        GameScore.Instance.PlayScoreTextAnimation(GameScore.ScoreTextAnimator.Close);
        GameScore.Instance.PlayComboAnimation(GameScore.ComboAnimator.Close);
    }

    public void OpenUpGrids()
    {
        // Set amount of images to be open
        switch (Level)
        {
            case 3:
                _openedCount = 1;

                for (int a = 0; a < _firstImages.Count; a++)
                {
                    _rangeOfNumbers.Add(a);
                }
                break;

            case 6:
                _openedCount = 2;
                break;

            case 9:
                _openedCount = 0;

                _nextLevelImages.Clear();
                break;

            case 11:
                _openedCount = 2;

                for (int a = 0; a < _secondImages.Count; a++)
                {
                    _rangeOfNumbers.Add(a);
                }

                // **** Set These ****
                _firstImages.Clear();
                for (int a = 0; a < _secondImages.Count; a++)
                {
                    _firstImages.Add(_secondImages[a]);
                }

                _secondImages.Clear();
                for (int a = 0; a < _thirdImages.Count; a++)
                {
                    _secondImages.Add(_thirdImages[a]);
                }
                // *******************
                break;

            case 20:
                _openedCount = 3;
                break;

            case 26:
                _openedCount = 0;

                _nextLevelImages.Clear();
                break;
        }

        // Open the Images Randomly
        if (_openedCount > 0)
        {
            for (byte a = 0; a < _openedCount; a++)
            {
                // Random which position of image should be break
                int randomedNumber = Random.Range(0, _rangeOfNumbers.Count);
                int elementNumber = _rangeOfNumbers[randomedNumber];

                // remove that element from rangeOfNumbers
                _rangeOfNumbers.RemoveAt(randomedNumber);

                // Clear firstImages & secondImages colors
                _firstImages[elementNumber].color = Color.clear;

                // Add images from secondImages & thirdImages to nextLevelImages with calculation range
                for (int b = elementNumber * 4; b < (elementNumber + 1) * 4; b++)
                {
                    _nextLevelImages.Add(_secondImages[b]);
                }
            }

            // Clear OpenedImage Waiting for new value
            OpenedImages.Clear();

            foreach (Image img in _firstImages)
            {
                // Only assign the one with color
                if (img.color != Color.clear)
                {
                    OpenedImages.Add(img);
                }
            }

            foreach (Image img in _nextLevelImages)
            {
                OpenedImages.Add(img);
            }
        }
    }

    private void QuestionAndTimer()
    {
        // Start Counting up for open the question
        if (_isOpened == false && AnsButton == 0)
        {
            switch (SkipFrame)
            {
                case false:
                    // Keep counting...
                    _openTimer = _openTimer + Time.unscaledDeltaTime;
                    break;

                case true:
                    // Ignore or Skip this frame (next frame Time.unscaledDeltaTime value will be normal)
                    // Set back
                    SkipFrame = false;
                    break;
            }

            // If timer reaches the delay...
            if (_openTimer >= _openDelay)
            {
                OpenQuestion();

                if (GameMode == 1 || GameMode == 2)
                {
                    // Assign with 0 when the counter is reaches normally
                    _timeUpTimer = (_openTimer - _openDelay < 0.2f) ? 0f : _openTimer - _openDelay;
                }
                else if (GameMode == 3)
                {
                    // Addup with 0 when the counter is reaches normally
                    _timeUpTimer += (_openTimer - _openDelay < 0.2f) ? 0f : _openTimer - _openDelay;
                }
            }
        }
        // Start Counting up for the timer
        else if (_isOpened == true && AnsButton == 0)
        {
            switch (PausePlayGameButton.IsGamePause)
            {
                case false:
                    // When game is not Paused - use unscaledDeltaTime cuz it will count in background
                    _timeUpTimer = _timeUpTimer + Time.unscaledDeltaTime;
                    break;

                case true:
                    // When game is Paused - use deltaTime cuz it won't get timer will actually paused
                    _timeUpTimer = _timeUpTimer + Time.deltaTime;
                    break;
            }

            if (_timeUpTimer < _timeUpDelay)
            {
                _countDownTimer = _timeUpDelay - _timeUpTimer;

                _subHeaderText.text = _countDownTimer.ToString("0.00") + " sec";
            }
            // If timer reaches the delay...
            else if (_timeUpTimer >= _timeUpDelay)
            {
                GameSfx.Instance.PlaySound(5);

                // ...assign time is up
                _subHeaderText.text = "<color=#A23D3D>Time up!</color>";

                // ...this will make the code go to "when answer is wrong"
                // and also block button from taping the button
                AnsButton = 5;

                // No need to take a screen shot here when time is up
                // Cuz this will enter "when answer is wrong" and take there
            }
        }
    }

    private void CheckingAnswer()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AnsButton = CorrectButton;

        // When answer is correct
        if (AnsButton != 0 && AnsButton == CorrectButton)
        {
            if (_ansButtonsImages[CorrectButton - 1].color.Equals(_buttonColors[AnsColorAccessor]))
            {
                GameSfx.Instance.PlaySound(3);

                if (GameMode == 1 || GameMode == 2)
                {
                    // Close Question - for the GameScore class to display there
                    CloseQuestion();

                    // Calculate and Display Score - on SubHeaderText UI / ScoreText UI / Combo UI
                    GameScore.Instance.DisplayScore(GameScore.DisplaySequence.Running, _countDownTimer, OpenedImages.Count, _timeUpDelay);

                    UpdateHighScoreE();
                }
                else if (GameMode == 3)
                    UpdateHighScoreT(); // When checking Level -9 at CheckingAnswer() OR -10 at RefreshTheLevel()

                // Make all buttons can't iteract
                foreach (Button button in _ansButtonsButton)
                {
                    button.interactable = false;
                }

                // Change Buttons Color to Correct
                _ansButtonsImages[CorrectButton - 1].color = _trueFalseColors[0];

                // Use this to enter once
                _enterCheck = false;

                // For non-trial mode
                if (TrialButton.TrialStarted == false)
                {
                    // ...take a screen shot
                    StartCoroutine(GroundButton.Instance.TakeScreenShot());
                }
                // For Trial mode
                else
                {
                    // Set new endingDelay depends on Level
                    _endingDelay = (Level < _maxLevel) ? 0.75f : 3.0f;
                }

                // ********* Unlocking Achievement FOR BOTH *********
                if (_openTimer < _openDelay)
                {
                    GameSocial.UnlockAchievement(6); // lucky
                }
                else if ((GameMode == 1 || GameMode == 2) && _timeUpTimer < 1f && Level >= 10)
                {
                    GameSocial.UnlockAchievement(3); // hawk eyed
                }
                else if ((GameMode == 1 || GameMode == 2) && _countDownTimer < 1f)
                {
                    GameSocial.UnlockAchievement(4); // so close
                }
                // *****************************************
            }

            // Start counting
            _endingTimer = _endingTimer + Time.deltaTime;

            if (_endingTimer >= _endingDelay)
            {
                // When This running in Trial mode
                if (TrialButton.TrialStarted == true && _enterCheck == false)
                {
                    if (Level < _maxLevel)
                    {
                        // Go to next level
                        _isNextLevel = true;
                    }
                    else
                    {
                        EndTrialGamePlay();
                    }
                }
                // If not, also Make it enter once (Cuz when above code is get in once & waiting this will run)
                else if (_enterCheck == false)
                {
                    // Go to next level
                    _isNextLevel = true;
                }
                _enterCheck = true;
            }
        }
        // When answer is wrong
        else if (AnsButton != 0 && AnsButton != CorrectButton)
        {
            if (_ansButtonsImages[0].color.Equals(_buttonColors[AnsColorAccessor]))
            {
                if (GameMode == 1 || GameMode == 2)
                {
                    // Calculate and Display Score - on SubHeaderText UI / ScoreText UI / Combo UI
                    GameScore.Instance.DisplayScore(GameScore.DisplaySequence.Ended, _countDownTimer, OpenedImages.Count, _timeUpDelay);
                }

                // Change the subHeader
                if (_timeUpTimer < _timeUpDelay)
                {
                    GameSfx.Instance.PlaySound(4);

                    _subHeaderText.text = "<color=#A23D3D>Wrong!</color>";
                }

                // Make all buttons can't iteract
                foreach (Button button in _ansButtonsButton)
                {
                    button.interactable = false;
                }

                // Change Buttons Color to Correct & Wrong
                foreach (Image img in _ansButtonsImages)
                {
                    img.color = _trueFalseColors[1];
                }
                _ansButtonsImages[CorrectButton - 1].color = _trueFalseColors[0];

                // Use this to enter once
                _enterCheck = false;

                // For non-trial mode
                if (TrialButton.TrialStarted == false)
                {
                    // Set new endingDelay depends on ReviveAmount
                    _endingDelay = (ReviveAmount >= 1) ? 0.75f : 3.0f;

                    // Only take screen shot when answer is wrong at first level...
                    if ((Level == 1 && GameMode == 1) || (Level == 10 && (GameMode == 2 || GameMode == 3)))
                    {
                        // ...take a screen shot
                        StartCoroutine(GroundButton.Instance.TakeScreenShot());
                    }
                }
                // For trial mode
                else
                {
                    // Set new endingDelay depends on Level
                    _endingDelay = (Level < _maxLevel) ? 0.75f : 3.0f;
                }
            }

            // Start counting (in 'else' block cuz "OpenTimer, TimeUpTimer, EndingTimer" won't continue running when they reaches the delay)
            if (_endingTimer >= _endingDelay)
            {
                // When This running in Trial mode
                if (TrialButton.TrialStarted == true && _enterCheck == false)
                {
                    if (Level < _maxLevel)
                    {
                        // Go to next level
                        _isNextLevel = true;
                    }
                    else
                    {
                        EndTrialGamePlay();
                    }
                }
                // If not, also Make it enter once
                else if (_enterCheck == false)
                {
                    if (((GameMode == 1 || GameMode == 2) || (GameMode == 3 && AnsButton != 5)) && ReviveAmount >= 1)
                    {
                        GameMusic.Instance.PlaySound(4);

                        // Update the text
                        _subHeaderText.text = "Revive? (" + ReviveAmount + ")";
                        // Play slide down animation
                        _topTextImageAnimators[2].Play("SubHeaderText Get In", -1, 0f);
                        _topTextImageAnimators[3].Play("SubHeaderImage Get In", -1, 0f);

                        CloseQuestion();

                        // Set Revive to true
                        GameState.Revive = true;

                        // Minus amount
                        ReviveAmount--;
                        _isRevived = true;
                    }
                    else
                    {
                        // Check for highScoreBeated
                        _subHeaderText.text = (CurHighScoreBeated == true) ? "<color=#EF7028>New Best!</color>" : "Game Over";

                        CloseQuestion();

                        // Set IsGameEnded to true
                        GameState.IsGameEnded = true;
                    }
                }
                _enterCheck = true;
            }
            else
            {
                _endingTimer = _endingTimer + Time.deltaTime;
            }
        }
    }

    private void UpdatingTopTextsAnimation()
    {
        // Play HeaderImage Idle animation after Get In is finished
        if (_topTextImageAnimators[1].GetCurrentAnimatorStateInfo(0).IsName("HeaderImage Get In") &&
           _topTextImageAnimators[1].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _topTextImageAnimators[1].Play("HeaderImage Idle", -1, 0f);
        }
        // Play SubHeaderImage Idle animation after Get In is finished
        if (_topTextImageAnimators[3].GetCurrentAnimatorStateInfo(0).IsName("SubHeaderImage Get In") &&
           _topTextImageAnimators[3].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            _topTextImageAnimators[3].Play("SubHeaderImage Idle", -1, 0f);
        }
    }

    private void ManageGridColors()
    {
        sbyte avoidColor = -1;

        switch (GameTheme.CurTheme)
        {
            case "default":
            case "gold":
            case "orange":
                avoidColor = 9;
                break;
            case "dark":
                avoidColor = 0;
                break;
            case "lightGreen":
                avoidColor = 6;
                break;
            case "lightBlue":
            case "blue":
                avoidColor = 4;
                break;
            case "red":
            case "purple":
            case "pink":
                avoidColor = 11;
                break;
        }
        GridColorsManaged = new Color32[avoidColor != -1 ? 11 : 12];
        sbyte a = -1;

        for (byte i = 0; i < GridColorsManaged.Length; i++)
        {
            a += (i != avoidColor) ? (sbyte)1 : (sbyte)2;
            GridColorsManaged[i] = _gridColors[a];
        }
    }

    private void SetBackQAUI()
    {
        QuestionText.text = "x";
        QuestionText.font = _font[0];

        // Close 2 Question Images, Set Sprite, Set size
        for (byte i = 0; i < 2; i++)
        {
            _questionImagesGameObject[i].SetActive(false);

            QuestionImages[i].overrideSprite = null;
            _questionsRect[i].sizeDelta = new Vector2(40f, 40f);
        }

        // Open third ans button
        _ansButtonGameObject.SetActive(true);

        // Close 3 ansTexts
        for (byte i = 0; i < 3; i++)
        {
            _ansButtonsTextGameObject[i].SetActive(false);
            AnsButtonsText[i].text = "x";
            AnsButtonsText[i].font = _font[0];
        }

        // Close 6 ansColorImages
        for (byte i = 0; i < 6; i++)
            _ansPanelsImageGameObject[i].SetActive(false);
    }
    private void ManageQAUI()
    {
        switch (GameType)
        {
            case 1:
                // Set here cuz no need to change throughout the game
                QuestionText.text = "  = ?";

                // Open left Question Image 
                _questionImagesGameObject[0].SetActive(true);
                // Open 3 ansTexts
                for (byte i = 0; i < 3; i++)
                    _ansButtonsTextGameObject[i].SetActive(true);
                break;

            case 2:
                // Open 3 ansColorImages only first one in panel
                for (byte i = 0; i < 6; i += 2)
                    _ansPanelsImageGameObject[i].SetActive(true);
                break;

            case 3:
            case 4:
                // Set here cuz no need to change throughout the game
                QuestionText.text = (GameType == 3) ? "=" : "<b>≠</b>";
                if (GameType == 4)
                    QuestionText.font = _font[1];

                // Close third ans button
                _ansButtonGameObject.SetActive(false);

                // Open 2 Question Images, 2 ansTexts
                for (byte i = 0; i < 2; i++)
                {
                    _questionImagesGameObject[i].SetActive(true);
                    _ansButtonsTextGameObject[i].SetActive(true);
                }
                break;

            case 5:
            case 6:
                // Set here cuz no need to change throughout the game
                QuestionText.text = (GameType == 5) ? "=" : "<b>≠</b>";
                if (GameType == 6)
                    QuestionText.font = _font[1];

                // Open, Set Sprite, Set size for question images
                for (byte i = 0; i < 2; i++)
                {
                    _questionImagesGameObject[i].SetActive(true);
                    QuestionImages[i].overrideSprite = _letterSprites;
                    _questionsRect[i].sizeDelta = new Vector2(25f, 25f);
                }                   

                // Open 3 ansTexts & Assign text
                for (byte i = 0; i < 3; i++)
                {
                    _ansButtonsTextGameObject[i].SetActive(true);
                    AnsButtonsText[i].text = (GameType == 5) ? "=" : "<b>≠</b>";
                    if (GameType == 6)
                        AnsButtonsText[i].font = _font[1];
                }

                // Open 6 ansColorImages
                for (byte i = 0; i < 6; i++)
                    _ansPanelsImageGameObject[i].SetActive(true);
                break;
        }
    }

    private void ModeSetUp(float defaultE, float defaultT)
    {
        // If GameMode is 2 or 3 open grid as level 10
        if (GameMode == 2 || GameMode == 3)
        {
            for (byte i = 0; i < 9; i++) // run 9 times cuz after this it will run 1 more time
            {
                ++Level;
                OpenUpGrids();
            }
        }
        // default timer can't set at RefreshTheLevel() cuz have different start level
        if (GameMode == 1 || GameMode == 2)
        {
            _timeUpDelay = defaultE;
        }
        // If GameMode is 3 set timeUpDelay to specify one
        if (GameMode == 3)
        {
            _timeUpDelay = defaultT;

            _countDownTimer = _timeUpDelay - _timeUpTimer;
        }
    }
    private void TimeSetUp(float defaultE)
    {
        if (GameMode == 1 || GameMode == 2)
        {
            _timeUpDelay = defaultE;
        }
    }

    // Use in QuestionAndTimer
    private void OpenQuestion()
    {
        _questionPanelAnimator.Play("QuestionPanel Get In", -1, 0f);
        _isOpened = true;
    }
    // Use in RefreshTheLevel, CheckingAnswer, EndTrialGamePlay
    private void CloseQuestion()
    {
        _questionPanelAnimator.Play("QuestionPanel Idle", -1, 0f);
    }

    // Use in CheckingAnswer
    private void EndTrialGamePlay()
    {
        // Play TopTextImageAnimators Get In
        _topTextImageAnimators[0].Play("HeaderText Get In", -1, 0f);
        _topTextImageAnimators[1].Play("HeaderImage Get In", -1, 0f);
        _topTextImageAnimators[2].Play("SubHeaderText Get In", -1, 0f);
        _topTextImageAnimators[3].Play("SubHeaderImage Get In", -1, 0f);

        // Update the Header & SubHeader Text
        _headerText.text = "Trial Mode";
        _subHeaderText.text = "Hope u like!";

        GameState.IsGameEnded = true;

        CloseQuestion();
    }

    // Update HighScoreE and HighScoreT
    private void UpdateHighScoreE()
    {
        // Update the HighScoreE
        if (GameScore.Instance.TotalScore > HighScoreE && TrialButton.TrialStarted == false)
        {
            HighScoreE = GameScore.Instance.TotalScore;
            CurHighScoreBeated = true;

            // Save HighScoreE to save file
            GameData.SaveString(HighScoreE.ToString(), "HighScoreE");


            // ********* Add score to leaderborad FOR BOTH *********
            GameSocial.AddScoreToLeaderBoardE(HighScoreE);
            // *****************************************

#if !UNITY_EDITOR && UNITY_ANDROID
                // ******* Incrementing Achievement FOR ANDROID *******
                if (Level - 1 <= 33)
                {
                    GameSocial.IncrementAchievement(1);
                }
                // *****************************************
#endif
            // Set Congrat message
            if (HighScoreE >= ModeUI.TaskE[1] && HighScoreE <= ModeUI.TaskE[2] && ExtraPurchaseButton.HavePlayNow == false)
                MessageUI.CongratAt = 1;
            if (HighScoreE >= ModeUI.TaskE[2] && HighScoreE <= ModeUI.TaskE[2] + 50 && ExtraPurchaseButton.HavePlayNow == false)
                MessageUI.CongratAt = 2;

            // Set AskToReview
            if (HighScoreE >= 50 && HighScoreE <= 150)
                MessageUI.AskToReview = true;
        }

        // ********* Unlocking Achievement FOR BOTH *********
        if (HighScoreE >= ModeUI.TaskE[2] && _isRevived == false)
        {
            GameSocial.UnlockAchievement(1);
        }
        if (HighScoreE >= ModeUI.TaskE[2])
        {
            GameSocial.UnlockAchievement(5);
        }
        // *****************************************

#if !UNITY_EDITOR && UNITY_IOS
            // ********* Unlocking Achievement FOR IOS *********
            if (Level - 1 >= 33)
            {
                GameSocial.UnlockAchievement(7);
            }
            // *****************************************
#endif
        // Update ThemeTrial accordingly to HighScoreE
        if (HighScoreE >= GameTheme.ThemeTrialTask[8])
        {
            // This loop will runs 7 times not include the first element
            for (byte i = 8; i >= 1; i--)
            {
                if (HighScoreE >= GameTheme.ThemeTrialTask[i] && GameTheme.ThemeTrial[i] == "no")
                {
                    GameTheme.ThemeTrial[i] = "yes";
                }
            }
            // Save ThemeTrial to save file
            GameData.SaveStringArray(GameTheme.ThemeTrial, "ThemeTrial");
        }
        // Give First Theme to player
        if (HighScoreE >= GameTheme.ThemeTrialTask[0] && SecondPurchaseButton.HaveTheme[0] == false)
        {
            SecondPurchaseButton.HaveTheme[0] = true;

            // Save HaveTheme to save file
            GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");

            // Set Congrat message
            MessageUI.CongratAt = 4;
        }
    }

    private void UpdateHighScoreT()
    {
        // Update the HighScoreT
        if (Level - 9 > HighScoreT)
        {
            HighScoreT = (byte)(Level - 9);
            CurHighScoreBeated = true;

            // Save HighScoreT to save file
            GameData.SaveString(HighScoreT.ToString(), "HighScoreT");


            // ********* Add score to leaderborad FOR BOTH *********
            GameSocial.AddScoreToLeaderBoardT(HighScoreT);
            // *****************************************

#if !UNITY_EDITOR && UNITY_ANDROID
                // ******* Incrementing Achievement FOR ANDROID *******
                if (Level - 9 <= 15)
                {
                    GameSocial.IncrementAchievement(2);
                }
                // *****************************************
#endif
            // Set Congrat message
            if (HighScoreT == ModeUI.TaskT && ExtraPurchaseButton.HavePlayNow == false)
                MessageUI.CongratAt = 3;
        }

        // ********* Unlocking Achievement FOR BOTH *********
        if (Level - 9 == 10 && _isRevived == false)
        {
            GameSocial.UnlockAchievement(2);
        }
        // *****************************************

#if !UNITY_EDITOR && UNITY_IOS
            // ********* Unlocking Achievement FOR IOS *********
            if (Level - 9 == 15)
            {
                GameSocial.UnlockAchievement(8);
            }
            // *****************************************
#endif
    }

    #endregion
}