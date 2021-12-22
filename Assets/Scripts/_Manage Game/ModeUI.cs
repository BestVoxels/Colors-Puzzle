using UnityEngine;
using UnityEngine.UI;

public class ModeUI : MonoBehaviour // THIS CLASS - manage changing Game Types & mode UI text
{
    [SerializeField]
    private Image[] _buttonImages;
    [SerializeField]
    private Sprite[] _playSprites;
    [SerializeField]
    private Sprite[] _normalSprites;
    [SerializeField]
    private Sprite[] _disabledSprites;

    [SerializeField]
    private Text _headerText;
    [SerializeField]
    private Text _infoText;
    [SerializeField]
    private Animator _modePanelAnimator;
    [SerializeField]
    private Animator _modeInfoPanelAnimator;

    [SerializeField]
    private string[,] _modeMessages =
    {
        { "Choose Game Mode", "Endless or Timer" },
        { "New Endless Mode!", "Unlock after completed Endless mode level " + TaskE[1] },
        { "New Timer Mode!", "Unlock after completed Endless mode level " + TaskE[2] },
        { "New Game Type!", "Unlock after completed Timer mode level " + TaskT }
    };

    private sbyte _previous = -1;

    // For GameType 1
    public static int[] TaskE { get; } = { 0, 150, 250 };
    // For GameType 2
    public static byte TaskT { get; } = 12;

    public static ModeUI Instance { get; private set; }
    private void Awake() => Instance = this;

    // This script has to run after game play highscore is loaded
    private void Start() => RefreshButtons();

    public void Fetch(sbyte number, string header, string info)
    {
        // Set button sprite back to normal
        if (_previous != -1 && (UnlockedTaskE((byte)_previous) || UnlockedTaskT()) )
        {
            _buttonImages[_previous].overrideSprite = _normalSprites[_previous];
        }

        // Set button sprite first & start game play
        if (number != -1 && (UnlockedTaskE((byte)number) || UnlockedTaskT()) )
        {
            GameSfx.Instance.PlaySound(0);

            if (number == _previous)
                GameState.Instance.GameFirstStarted((byte)(number + 1));
            else
                _buttonImages[number].overrideSprite = _playSprites[number];

            ExtraButton.Instance.ClosePanel();
        }
        else
        {
            switch (number)
            {
                case -1: // Default message when it first pop up
                    header = _modeMessages[0, 0];
                    info = _modeMessages[0, 1];
                    break;
                case 1: // When Endless Mode is still locked
                    header = _modeMessages[1, 0];
                    info = _modeMessages[1, 1];
                    ExtraButton.Instance.OpenPanel();
                    break;
                case 2: // When Timer Mode is still locked
                    header = _modeMessages[2, 0];
                    info = _modeMessages[2, 1];
                    ExtraButton.Instance.OpenPanel();
                    break;
            }

            // When New Game Type is still locked
            if (number != -1 && NotUnlockedTaskT())
            {
                header = _modeMessages[3, 0];
                info = _modeMessages[3, 1];
                ExtraButton.Instance.OpenPanel();
            }

            GameSfx.Instance.PlaySound(number == -1 ? (byte)0 : (byte)2);
        }

        // Show Info text UI with animation get in & closing When info text isn't itself or Animation is idle
        if (_infoText.text != info || _modeInfoPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ModeInfoPanel Idle"))
        {
            _headerText.text = header;
            _infoText.text = info;

            _modeInfoPanelAnimator.Play("ModeInfoPanel Get In", -1, 0f);
        }

        _previous = number;
    }

    public void RefreshButtons()
    {
        if (IsComingType() || NotUnlockedTaskT())
        {
            for (byte i = 0; i < _buttonImages.Length; i++)
            {
                _buttonImages[i].overrideSprite = _disabledSprites[i];
            }
        }
        else
        {
            for (byte i = 0; i < _buttonImages.Length; i++) // start from 0 incase refresh after type is 0
            {
                if (UnlockedTaskE(i) || UnlockedTaskT())
                {
                    _buttonImages[i].overrideSprite = _normalSprites[i];
                }
            }
        }
    }

    public void OpenPanel() => _modePanelAnimator.Play("ModePanel Get In", -1, 0f);

    public void ClosePanelAndInfo()
    {
        // If ModePanel is OPEN Close with animation
        if (_modePanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("ModePanel Get In"))
        {
            _modePanelAnimator.Play("ModePanel Idle", -1, 0f);
            _modeInfoPanelAnimator.Play("ModeInfoPanel Idle", -1, 0f);
        }
    }

    // Method that use to check Coming Soon Type
    public static bool IsComingType() => GamePlay.GameType == GamePlay.MaxType + 1;

    // Methods that use Tasks pack with GameType to check OR our playnow product
    private bool UnlockedTaskE(byte number) => (GamePlay.GameType == 1 && GamePlay.HighScoreE >= TaskE[number]) || ExtraPurchaseButton.HavePlayNow;

    private bool UnlockedTaskT() => ((GamePlay.GameType == 0 || GamePlay.GameType >= 2) && GamePlay.HighScoreT >= TaskT) || ExtraPurchaseButton.HavePlayNow;
    public static bool NotUnlockedTaskT() => (GamePlay.GameType == 0 || GamePlay.GameType >= 2) && GamePlay.HighScoreT < TaskT && ExtraPurchaseButton.HavePlayNow == false;
}
