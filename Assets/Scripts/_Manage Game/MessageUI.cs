using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using System.Collections;
using System.Collections.Generic;

public class MessageUI : MonoBehaviour
{
    [SerializeField]
    private Text[] _messagesText;
    [SerializeField]
    private Animator[] _messagesAnimator;
    [SerializeField]
    private Transform _messageBTranform;

    private readonly float[] _timing =
    {
        4f, // Task
        6f, // Normal
        8f // Congrat & Android AskToReview
    };

    // These texts are for messaging algorithm, for others check by Find Reference at SetMessages methods
    private readonly string[,] _taskTexts =
    {
        { "CONGRATULATION!!!\n", " UNLOCKED" },
        { "<b>TASK</b>\n", "*didn't use*" },
        { " more in Timer Mode", " more to Unlock!" }
    };

    private readonly string[] _taskText =
    {
        "<b>TASK</b>\nunlock ",
        "New Endless Mode",
        "New Timer Mode",
        "New Game Types",
        "Dark Theme"
    };

    private readonly string[] _normalText =
    {
        //"\"Take a rest while playing by\nwaiting to revive\"",
        "\"Try to count by 2\"",
        //"\"Have any Questions?\ncontact us\"",
        //"\"Feel free to give us\nfeedback!\"",
        //"\"See official website from\nSupport button at Settings\"",
        "\"Website & Instagram links\ncheck at Support button\"",
        //"\"check our Games at\nofficial website!\"",
        "\"Follow us on Instagram!\n(bestvoxels)\"",
        "\"The more u revive\nThe less unlock score\nfor Dark Theme!\"",
    };

    private readonly string[] _ab = { "A", "B" };
    private byte _cur = 1;
    private bool _isBSet = false;
    private byte _bIndex = 0;

    private byte _index = 0;
    private byte[] _mixNumbers; // normalText's Size
    private List<byte> _numbers = new List<byte>();

    public static bool AskToReview { get; set; } = false;
    public static byte CongratAt { get; set; } = 0;
    public static bool JustOpen { get; set; } = true;
    public static float Time { get; private set; } = 0f;

    public static MessageUI Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        // Set Static Member back to default (No need to set JustOpen & CongratAt cuz use through scene)
        Time = 0f; // Time will get changes anyway when SetMessageAB call

        // Shuffle when want to show Normal text
        if (JustOpen == false)
            Shuffle();

        // Get MessageB Index in hierarchy like this cuz UI in there can changes
        _bIndex = (byte)_messageBTranform.GetSiblingIndex();

        if (GameTutor.TutorStarted == false)
            StartMessaging();
    }

    // For other classes to call & for more central place to manage beginText
    public void StartMessaging()
    {
        SetBeginText("tap to begin");
        StartCoroutine(Messaging());
    }

    // Messaging Algorithm - if this NOT run, classes that call OpenMessages Need to call SetMessageAB too
    private IEnumerator Messaging()
    {
        while (GameState.IsGameStarted == false && TrialButton.TrialStarted == false && TrialButton.ExploreStarted == false)
        {
            if (AskToReview)
            {
#if UNITY_IOS
                Device.RequestStoreReview();
#elif UNITY_ANDROID
                SetMessageAB(_timing[2], "Like our Game?\nWould u mind Rating us ^^");
                yield return new WaitForSeconds(Time);
#endif

                AskToReview = false;
            }
            if (CongratAt >= 1)
            {
                SetMessageAB(_timing[2], _taskTexts[0, 0] + _taskText[CongratAt] + _taskTexts[0, 1]);
                yield return new WaitForSeconds(Time);

                CongratAt = 0;

                goto TaskSection;
            }
            if (JustOpen == false)
            {
                SetMessageAB(_timing[1], _normalText[_mixNumbers[_index]]);
                yield return new WaitForSeconds(Time);

                _index = (byte)(_index == _normalText.Length - 1 ? 0 : ++_index);
            }

        TaskSection:
            byte a = GetTask();

            if (a >= 1)
            {
                SetMessageAB(_timing[0], _taskText[0] + _taskText[a] + "!");
                yield return new WaitForSeconds(Time);

                int diff = GetDiff(a);

                if (diff > 100 || diff == 0)
                    goto TaskSection;

                SetMessageAB(_timing[0], _taskTexts[1, 0] + Prefix(diff) + diff + _taskTexts[2, a != 3 ? 1 : 0]);
                yield return new WaitForSeconds(Time);
            }
            else if (a == 0 && JustOpen == true)
            {
                JustOpen = false;
                Shuffle();
            }
        }

        yield break;
    }

    public static byte GetTask()
    {
        if (GamePlay.HighScoreE < ModeUI.TaskE[1] && ExtraPurchaseButton.HavePlayNow == false)
            return 1;
        else if (GamePlay.HighScoreE < ModeUI.TaskE[2] && ExtraPurchaseButton.HavePlayNow == false)
            return 2;
        else if (GamePlay.HighScoreT < ModeUI.TaskT && ExtraPurchaseButton.HavePlayNow == false)
            return 3;
        else if (SecondPurchaseButton.HaveTheme[0] == false)
            return 4;
        else
            return 0;
    }

    public static int GetDiff(byte a)
    {
        int taskEorTaskTheme = (a == 1 || a == 2) ? ModeUI.TaskE[a] : GameTheme.ThemeTrialTask[0];
        return (a != 3 ? taskEorTaskTheme - GamePlay.HighScoreE : ModeUI.TaskT - GamePlay.HighScoreT);
    }

    public static string Prefix(int diff) => (diff < 10) ? "only " : "";

    public void SetMessageAB(float time, string text)
    {
        // This method will finish stuffs here first, then continue running from the call
        Time = time;

        _messagesText[_cur].text = text;

        if (_cur == 2)
            _isBSet = true;
        
        // If MessageB is set, start switching text
        if (_isBSet && GameState.Location == "toBegin" && (TrialButton.TrialStarted == false || TrialButton.ExploreStarted == true))
        {
            _messageBTranform.SetSiblingIndex(_cur == 2 ? _bIndex - 1 : _bIndex);

            _messagesAnimator[_cur].Play("Message" + _ab[_cur - 1] + "Text Fade In", -1, 0f);
            _messagesAnimator[CurRev()].Play("Message" + _ab[CurRev() - 1] + "Text Fade Out", -1, 0f);

            StartCoroutine(PlaySecondIdle(_cur, 1.3f));
        }

        // Updating after cur is used
        _cur = (byte)(_cur == 1 ? 2 : 1);
    }

    public void SetBeginText(string text) => _messagesText[0].text = text;

    public void OpenMessages(string command)
    {
        // Open Begin Text
        _messagesAnimator[0].Play("BeginText Second Idle", -1, 0f);

        // Open MessagesAB depends on canGetIn State (Can't check its text cuz its text will be changed first)
        if (command == "all (Get In)")
        {
            _messagesAnimator[1].Play("MessageAText Get In", -1, 0f);

            StartCoroutine(PlaySecondIdle(1, 1.3f));
        }
        else if (command == "all (Second Idle)")
            _messagesAnimator[CurRev()].Play("Message" + _ab[CurRev() - 1] + "Text Second Idle", -1, 0f);
    }

    public void CloseMessages(string command)
    {
        if (command == "all")
            _messagesAnimator[0].Play("BeginText Idle", -1, 0f);

        // Don't have to close these twice cuz doesn't check in Update like GameState
        _messagesAnimator[1].Play("MessageAText Idle", -1, 0f);
        _messagesAnimator[2].Play("MessageBText Idle", -1, 0f);
    }

    private IEnumerator PlaySecondIdle(byte message, float sec)
    {
        yield return new WaitForSeconds(sec); // Need wait quite a while cuz wants animation to match begin text

        // Also block if it is Fading Out when SetMessageAB get call closely
        if (_messagesAnimator[message].GetCurrentAnimatorStateInfo(0).IsName("Message" + _ab[message - 1] + "Text Fade Out") == false &&
            GameState.Location == "toBegin" && (TrialButton.TrialStarted == false || TrialButton.ExploreStarted == true))
            _messagesAnimator[message].Play("Message" + _ab[message - 1] + "Text Second Idle", -1, 0f);

        yield break;
    }

    private byte CurRev() => (byte)(_cur == 2 ? 1 : 2);

    private void Shuffle()
    {
        _numbers.Clear();
        _mixNumbers = new byte[_normalText.Length];

        // Assign numbers 0,1,2
        for (byte a = 0; a < _normalText.Length; a++)
        {
            _numbers.Add(a);
        }

        // Randomly assign number from list to array
        for (byte a = 0; a < _normalText.Length; a++)
        {
            byte randomedNumber = (byte)Random.Range(0, _numbers.Count);
            byte element = _numbers[randomedNumber];

            _numbers.RemoveAt(randomedNumber);

            _mixNumbers[a] = element;
        }
    }
}