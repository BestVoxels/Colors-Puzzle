using UnityEngine;
using System.Collections;

public class GameTutor : MonoBehaviour
{
    private float _colorDelay = 1f;

    private IEnumerator _tutorial;
    private readonly Color32 _tutorColor = new Color32(250, 206, 106, 255);

    public static bool TutorStarted { get; private set; } = false;

    // Need Instance cuz StopCoroutine can't stay inside static method
    public static GameTutor Instance { get; private set; }
    private void Awake() => Instance = this;
    // This script has to run before GamePromotion (DateTime save data) & MessageUI (use TutorStarted)
    private void Start()
    {
        if (GameData.IsFileExist("DateTime") == false)
        {
            TutorStarted = true;

            _tutorial = StartTutorial();
            StartCoroutine(_tutorial);
        }
    }

    private IEnumerator StartTutorial()
    {
        // GameState will open messages for these (unlike when explore started)

        MessageUI.Instance.SetBeginText("<b>Tutorial</b>");

        MessageUI.Instance.SetMessageAB(3f, "\"Welcome!\"");
        yield return new WaitForSeconds(MessageUI.Time);

        // Game Settings
        MessageUI.Instance.SetMessageAB(2f, "<b>Game Settings</b>");
        yield return new WaitForSeconds(_colorDelay);
        GameTheme.Instance.ManageMenuColor(-1, _tutorColor, 0);
        yield return new WaitForSeconds(MessageUI.Time - _colorDelay);
        MessageUI.Instance.SetMessageAB(3f, "~Managing stuff");
        yield return new WaitForSeconds(MessageUI.Time);
        MessageUI.Instance.SetMessageAB(3f, "~Check out Read Me button");
        yield return new WaitForSeconds(MessageUI.Time);

        // Shop
        MessageUI.Instance.SetMessageAB(2f, "<b>Shop</b>");
        yield return new WaitForSeconds(_colorDelay);
        GameTheme.Instance.ManageMenuColor(0, _tutorColor, 1);
        yield return new WaitForSeconds(MessageUI.Time - _colorDelay);
        MessageUI.Instance.SetMessageAB(3f, "~Changing the Game Color Theme");
        yield return new WaitForSeconds(MessageUI.Time);
        MessageUI.Instance.SetMessageAB(3f, "~Check out Game Products");
        yield return new WaitForSeconds(MessageUI.Time);

        // Google Play Games or Game Center
#if !UNITY_EDITOR && UNITY_ANDROID
        MessageUI.Instance.SetMessageAB(2f, "<b>Google Play Games</b>");
#elif UNITY_EDITOR || UNITY_IOS
        MessageUI.Instance.SetMessageAB(2f, "<b>Game Center</b>");
#endif
        yield return new WaitForSeconds(_colorDelay);
        GameTheme.Instance.ManageMenuColor(1, _tutorColor, 2);
        yield return new WaitForSeconds(MessageUI.Time - _colorDelay);
        MessageUI.Instance.SetMessageAB(3f, "~Beat all the\nAchievements!");
        yield return new WaitForSeconds(MessageUI.Time);
        MessageUI.Instance.SetMessageAB(3f, "~Get to the top of the\nLeaderboards!");
        yield return new WaitForSeconds(MessageUI.Time);

        // Final up
        GameTheme.Instance.ManageMenuColor(2, _tutorColor, -1);
        MessageUI.Instance.SetMessageAB(3f, "\"How to begin?\"");
        yield return new WaitForSeconds(MessageUI.Time);
        MessageUI.Instance.SetMessageAB(3f, "~Just Tap anywhere on the screen");
        yield return new WaitForSeconds(MessageUI.Time);
        MessageUI.Instance.SetMessageAB(3f, "~to choose Game\nModes & Types");
        yield return new WaitForSeconds(MessageUI.Time);

        MessageUI.Instance.SetMessageAB(2f, "\"Enjoy!\"");
        yield return new WaitForSeconds(MessageUI.Time);

        TutorStarted = false;
        MessageUI.Instance.StartMessaging();
        yield break;
    }

    // Method for GameSocial to call
    public void EndTutorial()
    {
        StopCoroutine(_tutorial);

        GameTheme.Instance.ManageMenuColor(-1, Color.clear, -1); // actually in GameSocial there is a call on RefreshAllUI so this doesn't nessessary
        TutorStarted = false;
        MessageUI.Instance.StartMessaging();
    }
}
