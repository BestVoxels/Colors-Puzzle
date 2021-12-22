using UnityEngine;
using UnityEngine.SocialPlatforms;
#if !UNITY_EDITOR && UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class GameSocial : MonoBehaviour
{
    public static bool LogInStatus { get; private set; } = false;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        // Initialize and activate Google Play Game service
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
#endif
        SignIn();
    }

    public static void SignIn()
    {
        // Sign In for both platform
        Social.localUser.Authenticate(success =>
        {
            if (success)
            {
                // This will run every time IF user loged in
                AssignHighScoreT();
                AssignHighScoreE();
                LogInStatus = true;
            }
        });
    }

    #region ACHIEVEMENTS
    public static void UnlockAchievement(byte number)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        switch (number)
        {
            case 1:
                Social.ReportProgress(GPGSIds.achievement_skillful_endless, 100.0f, success => { });
                break;
            case 2:
                Social.ReportProgress(GPGSIds.achievement_skillful_timer, 100.0f, success => { });
                break;
            case 3:
                Social.ReportProgress(GPGSIds.achievement_hawk_eyed, 100.0f, success => { });
                break;
            case 4:
                Social.ReportProgress(GPGSIds.achievement_so_close, 100.0f, success => { });
                break;
            case 5:
                Social.ReportProgress(GPGSIds.achievement_unlocker, 100.0f, success => { });
                break;
            case 6:
                Social.ReportProgress(GPGSIds.achievement_lucky, 100.0f, success => { });
                break;
        }
#elif !UNITY_EDITOR && UNITY_IOS
        switch (number)
        {
            case 1:
                Social.ReportProgress("skillful_endless", 100.0f, success => { }); // achievement_skillful_endless 
                break;
            case 2:
                Social.ReportProgress("skillful_timer", 100.0f, success => { }); // achievement_skillful_timer
                break;
            case 3:
                Social.ReportProgress("hawk_eyed", 100.0f, success => { }); // achievement_hawk_eyed
                break;
            case 4:
                Social.ReportProgress("so_close", 100.0f, success => { }); // achievement_so_close
                break;
            case 5:
                Social.ReportProgress("unlocker", 100.0f, success => { }); // achievement_unlocker
                break;
            case 6:
                Social.ReportProgress("lucky", 100.0f, success => { }); // achievement_lucky
                break;
            case 7:
                Social.ReportProgress("score_master_endless", 100.0f, success => { }); // achievement_score_master_endless
                break;
            case 8:
                Social.ReportProgress("score_master_timer", 100.0f, success => { }); // achievement_score_master_timer
                break;
        }
#endif
    }

    public static void IncrementAchievement(byte number)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        switch (number)
        {
            case 1:
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_score_master_endless, 1, success => { });
                break;
            case 2:
                PlayGamesPlatform.Instance.IncrementAchievement(GPGSIds.achievement_score_master_timer, 1, success => { });
                break;
        }
#endif
    }

    public static void ShowAchievementsUI()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        Social.ShowAchievementsUI();
#endif
    }
    #endregion


    #region LEADERBOARD
    // Note that the platform and the server will automatically check when setting highscore to leaderboard
    public static void AddScoreToLeaderBoardE(long score)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        Social.ReportScore(score, GPGSIds.leaderboard_endless_score, success => { });
#elif !UNITY_EDITOR && UNITY_IOS
        Social.ReportScore(score, "endless_score", success => { });
#endif
    }

    // Note that the platform and the server will automatically check when setting highscore to leaderboard
    public static void AddScoreToLeaderBoardT(long score)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        Social.ReportScore(score, GPGSIds.leaderboard_timer_score, success => { });
#elif !UNITY_EDITOR && UNITY_IOS
        Social.ReportScore(score, "timer_score", success => { });
#endif
    }

    public static void ShowLeaderboardUI()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        Social.ShowLeaderboardUI();
        // Use below code to show specific leaderboard
        // PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_endless_score);
#elif !UNITY_EDITOR && UNITY_IOS
        Social.ShowLeaderboardUI();
#endif
    }

    private static void AssignHighScoreE()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        ILeaderboard leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
        leaderboard.id = GPGSIds.leaderboard_endless_score;
#elif UNITY_EDITOR || UNITY_IOS
        ILeaderboard leaderboard = Social.CreateLeaderboard();
        leaderboard.id = "endless_score";
#endif
        leaderboard.timeScope = TimeScope.AllTime;
        leaderboard.LoadScores(success =>
        {
            if (success)
            {
                // Only assign leader board score when it is not matches with high score
                if (GamePlay.HighScoreE < (int)leaderboard.localUserScore.value)
                {
                    GamePlay.HighScoreE = (int)leaderboard.localUserScore.value;
                    // Save HighScoreE to save file
                    GameData.SaveString(GamePlay.HighScoreE.ToString(), "HighScoreE");


                    // Remove ThemeTrial cuz assume the already try them
                    if (GamePlay.HighScoreE >= GameTheme.ThemeTrialTask[8])
                    {
                        // This loop will runs 7 times not include the first element
                        for (byte i = 8; i >= 1; i--)
                        {
                            if (GamePlay.HighScoreE >= GameTheme.ThemeTrialTask[i] && GameTheme.ThemeTrial[i] == "no")
                            {
                                GameTheme.ThemeTrial[i] = "tried";
                            }
                        }
                        // Save ThemeTrial to save file
                        GameData.SaveStringArray(GameTheme.ThemeTrial, "ThemeTrial");
                    }
                    // Give First Theme to player
                    if (GamePlay.HighScoreE >= GameTheme.ThemeTrialTask[0] && SecondPurchaseButton.HaveTheme[0] == false)
                    {
                        SecondPurchaseButton.HaveTheme[0] = true;

                        // Save HaveTheme to save file
                        GameData.SaveBoolArray(SecondPurchaseButton.HaveTheme, "HaveTheme");
                    }

                    // Only let this refresh All UI when on main menu
                    if (GameState.IsGameStarted == false)
                    {
                        // Refresh mode buttons *there is a checking inside the method*
                        ModeUI.Instance.RefreshButtons();

                        // Update High Score Text
                        GameTheme.Instance.RefreshAllUI();
                        GameTheme.Instance.RefreshCurSlot(); // calling this to make arrow buttons change to trial color
                    }

                    if (GameTutor.TutorStarted == true)
                        GameTutor.Instance.EndTutorial();
                }
            }
            else
            {
                // Handle fail case: this is when your leaderboard doesn't load scores successfully
            }
        });
    }

    private static void AssignHighScoreT()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        ILeaderboard leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
        leaderboard.id = GPGSIds.leaderboard_timer_score;
#elif UNITY_EDITOR || UNITY_IOS
        ILeaderboard leaderboard = Social.CreateLeaderboard();
        leaderboard.id = "timer_score";
#endif
        leaderboard.timeScope = TimeScope.AllTime;
        leaderboard.LoadScores(success =>
        {
            if (success)
            {
                // Only assign leader board score when it is not matches with high score
                if (GamePlay.HighScoreT < (byte)leaderboard.localUserScore.value)
                {
                    GamePlay.HighScoreT = (byte)leaderboard.localUserScore.value;
                    // Save HighScoreT to save file
                    GameData.SaveString(GamePlay.HighScoreT.ToString(), "HighScoreT");

                    // Only let this refresh All UI when on main menu
                    if (GameState.IsGameStarted == false)
                    {
                        // Refresh mode buttons (Call here cuz we need to use this score to check also)
                        ModeUI.Instance.RefreshButtons();

                        // Update High Score Text
                        GameTheme.Instance.RefreshAllUI();
                        GameTheme.Instance.RefreshCurSlot(); // calling this to make arrow buttons change to trial color
                    }
                }
            }
            else
            {
                // Handle fail case: this is when your leaderboard doesn't load scores successfully
            }
        });
    }
    #endregion
}
