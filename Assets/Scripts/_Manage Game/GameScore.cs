using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScore : MonoBehaviour
{
    // Declare inside class since we want to access GameScore.DisplaySequence cuz it's mainly belong to this class not globally stuffs
    public enum DisplaySequence
    {
        Running,
        Ended
    }
    public enum ScoreTextAnimator
    {
        Close,
        ShowingScore,
        ShowingScoreAtTheEnd
    }
    public enum ComboAnimator
    {
        Close,
        PopUp
    }


    #region --Fields-- (Inspector)
    [Header("GameScore Properties")]
    [SerializeField]
    private float _overallScoreRate = 2f;
    [Tooltip("Player will get Combo if completed the timerDuration with this percentage amount | IF it is 90%, that means player can let the time passed by 90% and still get combo. (The Lower the harder it is)")]
    [Range(0f, _percentMax)]
    [SerializeField]
    private float _comboPercent = 30f;
    [Tooltip("First element is MixType score rate, Second element is FirstType, and so on...")]
    [SerializeField]
    private float[] _typeScore = { 2f, 1f, 1.5f, 1f, 1.5f };
    [Tooltip("Can increase more combo level")]
    [SerializeField]
    private float[] _comboLevel = { 1.5f, 2f, 2.5f, 3f };

    [Header("Text To Access")]
    [SerializeField]
    private Text _subHeaderText;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _comboText;

    [Header("Animations To Control")]
    [SerializeField]
    private Animator _scoreTextAnimator;
    [SerializeField]
    private Animator _comboTextAnimator;
    #endregion


    #region --Fields-- (In Class)
    private byte _comboIndexer = 0;
    #endregion


    #region --Fields-- (Constant)
    private const float _comboMin = 1f;
    private const float _percentMax = 100f;
    #endregion


    #region --Properties-- (Auto)
    // No need to make static since we want it to be reset everytime we reload scene, Declare as Property since we might use in other class
    public int TotalScore { get; private set; }
    public byte StarterGameType { get; set; } // Can't use GamePlay.GameType since this will changes when it's Mix type
    #endregion


    #region --Methods-- (Builtin)
    public static GameScore Instance { get; private set; }
    private void Awake() => Instance = this;
    #endregion


    #region --Methods-- (Custom)
    // Used in GamePlay to show score
    public void DisplayScore(DisplaySequence displaySequence, float remainingTime, int openedImagesCount, float timeUpDelay)
    {
        //print($"RemainingTime : {remainingTime}, OpenedImagesCount : {openedImagesCount}, TimerUpDelay : {timeUpDelay}, StarterType : {StarterGameType}, TypeScore : {_typeScore[StarterGameType]}");

        if (displaySequence == DisplaySequence.Running)
        {

            int levelScore = CalLevelScore(remainingTime, openedImagesCount);
            float _comboMultiplier = CalComboAmount(remainingTime, timeUpDelay);

            int finalScore = (int)(_comboMultiplier * levelScore); // Cast this way will floor it down i.e. 19.5 to 19

            //print($"LevelScore : {levelScore}, ComboMultiplier : {_comboMultiplier}. FinalScore : {finalScore}");

            // Add Current Final Score to TotalScore
            TotalScore += finalScore;


            // Display Current Final Score - on ScoreText UI
            DisplayFinalScore(finalScore);

            // Display Combo Score - on Combo UI
            DisplayComboScore(_comboMultiplier);
        }

        // Display Total Score - on SubHeaderText UI / ScoreText UI
        DisplayTotalScore(displaySequence);
    }

    #region --Methods-- (Formula for calculating Level Score and Combo Score)
    private int CalLevelScore(float remainingTime, int openedImagesCount)
    {
        // Level Score Formula
        return (int)(_typeScore[StarterGameType] + (remainingTime * openedImagesCount * _overallScoreRate));
    }
    private float CalComboAmount(float remainingTime, float timerDuration)
    {
        //print($"InverseLerp : {Mathf.InverseLerp(0f, _percentMax, _comboPercent)}, RemainingTime : {remainingTime}, ComboTimeDuration : {timerDuration * (1f - Mathf.InverseLerp(0f, _percentMax, _comboPercent))}");

        // Combo Formula
        if (remainingTime >= timerDuration * (1f - Mathf.InverseLerp(0f, _percentMax, _comboPercent)))
        {
            // Combo Result
            float result = _comboLevel[_comboIndexer];

            // Increment comboIndexer
            _comboIndexer = (_comboIndexer < _comboLevel.Length - 1) ? ++_comboIndexer : _comboIndexer;

            return result;
        }

        // When Player couldn't do combo - Set combo back
        _comboIndexer = 0;

        // Return the lowest Combo Amount it can be
        return _comboMin;
    }
    #endregion

    #region --Methods-- (Display Score with text format)
    private void DisplayFinalScore(int finalScore)
    {
        PlayScoreTextAnimation(ScoreTextAnimator.ShowingScore);

        _scoreText.text = $"+{finalScore}";
    }

    private void DisplayComboScore(float comboAmount)
    {
        if (comboAmount > 1f)
            PlayComboAnimation(ComboAnimator.PopUp);

        _comboText.text = $"Combo\nx{comboAmount}";
    }

    private void DisplayTotalScore(DisplaySequence displaySequence)
    {
        switch (displaySequence)
        {
            case DisplaySequence.Running:
                _subHeaderText.text = $"{TotalScore}";
                break;

            case DisplaySequence.Ended:
                PlayScoreTextAnimation(ScoreTextAnimator.ShowingScoreAtTheEnd);

                _scoreText.text = $"<size=35>Total</size><size=50>\n{TotalScore}</size>";
                break;
        }
    }
    #endregion

    #region --Methods-- (Play Animations for UI)
    public void PlayScoreTextAnimation(ScoreTextAnimator input)
    {
        switch (input)
        {
            case ScoreTextAnimator.Close:
                _scoreTextAnimator.Play("ScorePanel Idle", -1, 0f);
                break;
            case ScoreTextAnimator.ShowingScore:
                _scoreTextAnimator.Play("ScorePanel Showing Score", -1, 0f);
                break;
            case ScoreTextAnimator.ShowingScoreAtTheEnd:
                StartCoroutine(PlayScoreTextAnimationSecondIdle());
                break;
        }
    }

    private IEnumerator PlayScoreTextAnimationSecondIdle()
    {
        _scoreTextAnimator.Play("ScorePanel Second Get In", -1, 0f);

        yield return new WaitForSeconds(0.5f);

        if (_scoreTextAnimator.GetCurrentAnimatorStateInfo(0).IsName("ScorePanel Second Get In"))
            _scoreTextAnimator.Play("ScorePanel Second Idle", -1, 0f);

        yield break;
    }

    public void PlayComboAnimation(ComboAnimator input)
    {
        switch (input)
        {
            case ComboAnimator.Close:
                _comboTextAnimator.Play("ComboText Idle", -1, 0f);
                break;
            case ComboAnimator.PopUp:
                _comboTextAnimator.Play("ComboText Pop Up", -1, 0f);
                break;
        }
    }
    #endregion
    #endregion
}