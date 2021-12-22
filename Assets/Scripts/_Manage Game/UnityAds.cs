using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour
{
#if UNITY_IOS
    private string gameId = "2893043";
#elif UNITY_ANDROID
    private string gameId = "2893044";
#endif

    public static UnityAds Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        if (Advertisement.isInitialized == false)
        {
            Advertisement.Initialize(gameId, false);
            // test mode change to false when want to publish
        }
    }

    public void ShowRewardedVideo()
    {
        if(Advertisement.IsReady("rewardedVideo") == true && Advertisement.isShowing == false)
        {
            // Assign the ShowOptions' resultCallback with our own ShowResult checking method
            // Assign using Object initialzer
            ShowOptions options = new ShowOptions
            {
                resultCallback = ResultChecker
            };

            Advertisement.Show("rewardedVideo", options);
            
            GameState.Revive = false; // Freeze Panel from skipping
            GameMusic.Instance.MuteState(true);
        }
        else if(Advertisement.IsReady("rewardedVideo") == false)
        {
            GameAd.Instance.FetchAds("try next", -1);
        }
	}

    private void ResultChecker(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                GameMusic.Instance.MuteState(!MusicButton.CanPlayMusic); //Unmute sound upto setting

                switch (GameAd.CurAd)
                {
                    case 0:
                        GamePlay.SkipFrame = true;
                        GameAd.Instance.GrantRevive();
                        break;
                    case 1:
                        GameAd.Instance.GrantTrial(0);
                        break;
                }
                break;

            case ShowResult.Failed: // This wil ads failed no need to run new ads
                switch (GameAd.CurAd)
                {
                    case 0:
                        GameState.Revive = true; //Unfreeze Panel
                        break;
                    case 1:
                        break;
                }
                break;
        }
    }
}
