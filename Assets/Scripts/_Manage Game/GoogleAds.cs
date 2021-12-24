using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class GoogleAds : MonoBehaviour
{
    private const bool _useTestAd = false; // CHANGE TO "FALSE" when release

#pragma warning disable RECS0110
#if UNITY_IOS
    private string[] _adUnitIds =
    {
        _useTestAd ? "ca-app-pub-3940256099942544/1712485313" : "ca-app-pub-6389023471722542/8466364959",
        _useTestAd ? "ca-app-pub-3940256099942544/1712485313" : "ca-app-pub-6389023471722542/1280763639"
    };
#elif UNITY_ANDROID
    private string[] _adUnitIds =
    {
        _useTestAd ? "ca-app-pub-3940256099942544/5224354917" : "ca-app-pub-6389023471722542/1461214735",
        _useTestAd ? "ca-app-pub-3940256099942544/5224354917" : "ca-app-pub-6389023471722542/1939239188"
    };
#endif
#pragma warning restore RECS0110

    private RewardedAd[] _rewardedAds = new RewardedAd[2];
    private bool _isLoading = false;
    private bool _isFailed = false;

    public static GoogleAds Instance { get; private set; }
    private void Awake() => Instance = this;

    private void Start()
    {
        for (byte i = 0; i < _rewardedAds.Length; i++)
            CreateAndLoadRewardedAd(i);
    }

    public void CreateAndLoadRewardedAd(byte ad)
    {
        _rewardedAds[ad] = new RewardedAd(_adUnitIds[ad]);

        // Hook events into the ad's lifecycle
        _rewardedAds[ad].OnAdLoaded += AdLoaded;
        _rewardedAds[ad].OnAdFailedToLoad += AdLoadFailed;
        _rewardedAds[ad].OnAdFailedToShow += AdShowFailed;
        _rewardedAds[ad].OnAdOpening += AdOpening;
        _rewardedAds[ad].OnAdClosed += AdClosed;
        _rewardedAds[ad].OnUserEarnedReward += UserEarnedReward;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        _rewardedAds[ad].LoadAd(request);
    }

    public void ShowRewardedVideo()
    {
        if (_rewardedAds[GameAd.CurAd].IsLoaded())
            _rewardedAds[GameAd.CurAd].Show();
        else if (!_isFailed)
        {
            _isLoading = true;
            ProcessingUI.Instance.OpenPanel("~Loading~");
        }
        else if (_isFailed)
        {
            GameAd.Instance.FetchAds("try next", -1);
        }
    }

    private void AdLoaded(object sender, EventArgs args)
    {
        _isFailed = false;

        if (_isLoading)
        {
            _isLoading = false;
            ProcessingUI.Instance.InstantClose();

            ShowRewardedVideo();
        }
    }

    private void AdLoadFailed(object sender, AdFailedToLoadEventArgs args)
    {
        _isFailed = true;

        if (_isLoading)
        {
            _isLoading = false;
            ProcessingUI.Instance.InstantClose();

            GameAd.Instance.FetchAds("try next", -1);
        }
    }
    private void AdShowFailed(object sender, AdErrorEventArgs args) // Havn't been in this case yet
    {
        _isFailed = true;

        ProcessingUI.Instance.InstantClose(); // Just incase

        GameAd.Instance.FetchAds("try next", -1);
    }

    private void AdOpening(object sender, EventArgs args)
    {
        //For Revive, No need to Freeze panel from skipping cuz this method called when it is already covered the screen
        GameMusic.Instance.MuteState(true);
    }

    private void AdClosed(object sender, EventArgs args)
    {
        GameMusic.Instance.MuteState(!MusicButton.CanPlayMusic); //Unmute sound upto setting

        // Load next Ad
        CreateAndLoadRewardedAd((byte)GameAd.CurAd);
    }

    private void UserEarnedReward(object sender, Reward args) //Access args' value by .Type or .Amount
    {
        switch (GameAd.CurAd)
        {
            case 0:
                GamePlay.SkipFrame = true;
                GameAd.Instance.GrantRevive();
                break;
            case 1:
                GameAd.Instance.GrantTrial((byte)(_useTestAd ? 0 : (args.Amount - 1)));
                break;
        }
    }
}
