using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    #region --Fields-- (Inspector)
    [Header("Game Id - ProjectSettings/Services/Ads/Game Id")]
    [SerializeField] private string _androidGameId = "2893044";
    [SerializeField] private string _iOSGameId = "2893043";
    [SerializeField] private bool _testMode = false;

    [Header("Placement Id - Unity Dashboard/Monetization/Placements/ID: ...")]
    [SerializeField] private string _androidPlacementId = "rewardedVideo";
    [SerializeField] private string _iOSPlacementId = "rewardedVideo";
    #endregion



    #region --Fields-- (In Class)
    private string _gameId;
    private string _adUnitId = null; // This will remain null for unsupported platforms
    #endregion



    #region --Properties-- (Auto)
    public static UnityAds Instance { get; private set; }
    #endregion



    #region --Methods-- (Built In)
    private void Awake() => Instance = this;
    
    private void Start() => InitializeAds();
    #endregion



    #region --Methods-- (Custom PUBLIC)
    public void InitializeAds()
    {
#if UNITY_IOS
        _gameId = _iOSGameId;
        _adUnitId = _iOSPlacementId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _adUnitId = _androidPlacementId;
#endif

        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void ShowRewardedVideo()
    {    
        GameState.Revive = false; // Freeze Panel from skipping
        GameMusic.Instance.MuteState(true);

        Advertisement.Show(_adUnitId, this);
    }
    #endregion



    #region --Methods-- (Custom PRIVATE)
    #endregion



    #region --Methods-- (Interface) ~Initialization~
    void IUnityAdsInitializationListener.OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete.");

        // IMPORTANT! Only load content AFTER initialization 
        Advertisement.Load(_adUnitId, this);
    }

    void IUnityAdsInitializationListener.OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    #endregion



    #region --Methods-- (Interface) ~Load~
    void IUnityAdsLoadListener.OnUnityAdsAdLoaded(string placementId)
    {
        //Debug.Log("Ad Loaded: " + placementId);

        if (placementId.Equals(_adUnitId))
        {
            // Ready to serve ads.
        }
    }

    void IUnityAdsLoadListener.OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {placementId}: {error.ToString()} - {message}"); // Use the error details to determine whether to try to load another ad.

        GameAd.Instance.FetchAds("try next", -1);
    }
    #endregion



    #region --Methods-- (Interface) ~Show~
    void IUnityAdsShowListener.OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        //Debug.Log("Ad Show Completed: " + placementId);

        if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            // Grant a reward.
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

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }

    void IUnityAdsShowListener.OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}"); // Use the error details to determine whether to try to load another ad.

        switch (GameAd.CurAd)
        {
            case 0:
                GameState.Revive = true; //Unfreeze Panel
                break;
            case 1:
                break;
        }

        GameAd.Instance.FetchAds("try next", -1);
    }

    void IUnityAdsShowListener.OnUnityAdsShowStart(string placementId)
    {
    }

    void IUnityAdsShowListener.OnUnityAdsShowClick(string placementId)
    {
    }
    #endregion
}
