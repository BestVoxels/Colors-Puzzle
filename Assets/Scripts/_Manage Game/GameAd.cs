using UnityEngine;
using UnityEngine.UI;

public class GameAd : MonoBehaviour
{
    [SerializeField]
    private Animator _revivePanelAnimator;
    [SerializeField]
    private Animator _instructionAnimator;
    [SerializeField]
    private Button[] _reviveTrialButton;
    public Button[] ReviveTrialButton { get { return _reviveTrialButton; } } // Declare here cuz this class already has instance of it & it manage both of them in array perfectly

    private const byte _defaultAd = 1; //"Google" = 1 | "Unity" = 2 (Manage Order at Switch Case in FetchAds)
    private const byte _maxAds = 2;

    private byte _adStarter = _defaultAd;
    private byte _adError = 0;

    public static sbyte CurAd = -1;

    public static GameAd Instance { get; private set; }
    private void Awake() => Instance = this;

    public void FetchAds(string input, sbyte ad)
    {
        if (ad != -1)
            CurAd = ad;

        if (input == "try next")
        {
            _adError++;

            if (_adError == _maxAds)
            {
                ReviveTrialButton[CurAd].interactable = false;
                ProcessingUI.Instance.ClosePanel("~Failed to Load~\nTry Again");
                return;
            }

            _adStarter = (byte)(_adStarter < _maxAds ? _adStarter++ : 1);
        }
        // Try DefaultAd again (When Pressed Button Again)
        else if (input == "default" && _adError > 0)
        {
            _adStarter = _defaultAd;
            _adError = 0;
            ReviveTrialButton[CurAd].interactable = true;

            GoogleAds.Instance.CreateAndLoadRewardedAd((byte)CurAd);
            // Can't do for Unity cuz don't have
        }

        switch (_adStarter)
        {
            case 1:
                GoogleAds.Instance.ShowRewardedVideo();
                break;

            case 2:
                UnityAds.Instance.ShowRewardedVideo(); // pass ad num too cuz we need to continue fetching back and forth
                break;
        }
    }

    public void GrantRevive()
    {
        GameState.Revive = false;
        GameState.SkipTimer = 0f;

        _revivePanelAnimator.Play("RevivePanel Idle", -1, 0f);
        _instructionAnimator.Play("InstructionText Idle", -1, 0f);

        GameTheme.IncreseRevived();
        GameTheme.Instance.RefreshTask();

        // Open #PauseGamePanel 
        PausePlayGameButton.Instance.OpenPausePanel();

        // Repeat same level
        GamePlay.IsRepeatLevel = true;
    }

    public void GrantTrial(byte type)
    {
        // Set trial to tried
        GameTheme.ThemeTrial[GameTheme.CurSlot] = "tried";
        // Save ThemeTrial to save file
        GameData.SaveStringArray(GameTheme.ThemeTrial, "ThemeTrial");

        // Start trial mode & disable explore mode (In case player click try now when explore is not finish)
        TrialButton.TrialStarted = true;
        TrialButton.ExploreStarted = false;

        // This help IF player click try on same theme again (while exploring) or IF click try on others this will reduce job for UpdateCurTheme to find default slot to exchange
        GameTheme.Instance.ToDefaultTheme();
        // Change Theme color for that but don't save
        GameTheme.Instance.UpdateCurTheme();

        GameTheme.Instance.RefreshAllUI();
        GameTheme.Instance.RefreshCurSlot();

        // Set GameType to value from Ads (For ButtonsPanel stuff no need to show user & will get refresh in start)
        GamePlay.GameType = type;

        // Exit from shop panel & Start trial game play
        GameState.Instance.PointerDown();
        StartCoroutine(GameState.Instance.StartTrialGamePlay());
    }
}
