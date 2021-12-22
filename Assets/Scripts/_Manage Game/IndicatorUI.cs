using UnityEngine;
using UnityEngine.UI;
using System;

public class IndicatorUI : MonoBehaviour
{
    // Declare inside class since we want to access IndicatorUI.IndicatorImageAnimator cuz it's mainly belong to this class not globally stuffs
    public enum IndicatorImageAnimator
    {
        Close,
        Open
    }

    #region --Fields-- (Inspector)
    [Header("IndicatorUI Properties")]
    [Tooltip("Range Interval Percent - this will be calculated with Current Task")]
    [Range(0f, _intervalPercentMax)]
    [SerializeField]
    private float _intervalPercent = 3f;

    [Tooltip("Percent arrary to get Task score to compare with")]
    [Range(0f, _percentMax)]
    [SerializeField]
    private float[] _taskPercents =
    {
        10f,
        20f,
        50f,
        80f
    };

    [Tooltip("Enter string to indicator player here ['Remaining' - to show remaining score | 'Reward' - to show what player will get]")]
    [TextArea]
    [SerializeField]
    private string _indicatorGoalText = "Score 'Remaining' more to get new 'Reward'!";

    [Tooltip("Enter string to congrat player here ['Reward' - to show what player will get]")]
    [TextArea]
    [SerializeField]
    private string _indicatorAchieveText = "Congratulation! Unlocked new 'Reward'!";

    [Header("Text To Access")]
    [SerializeField]
    private Text _indicatorText;

    [Header("Animations To Control")]
    [SerializeField]
    private Animator _indicatorImageAnimator;
    #endregion


    #region --Fields-- (In Class)
    private readonly int[] _allTaskE =
    {
        ModeUI.TaskE[1],
        ModeUI.TaskE[2],
        GameTheme.ThemeTrialTask[0],
        ModeUI.TaskT
    };
    private bool[] _allBoolE =
    {
        ExtraPurchaseButton.HavePlayNow == false,
        ExtraPurchaseButton.HavePlayNow == false,
        SecondPurchaseButton.HaveTheme[0] == false,
        ExtraPurchaseButton.HavePlayNow == false
    };
    private readonly string[] _allReward =
    {
        "Endless Game Mode",
        "Timer Game Mode",
        "Dark Theme",
        "Game Types"
    };

    private int _prevLevel = -1;
    private byte _curTask = 0;
    private bool _isUpdatedCurTask = false;
    #endregion


    #region --Fields-- (Constant)
    private const float _percentMax = 100f;
    private const float _intervalPercentMax = 5f;
    private const byte _taskTStartPosition = 3; // In array
    #endregion


    #region --Methods-- (Builtin)
    public static IndicatorUI Instance { get; private set; }
    private void Awake() => Instance = this;
    #endregion


    #region --Methods-- (Custom)
    public void IndicateText()
    {
        // Make it upto date with these bool values
        _allBoolE = new bool[]
        {
            ExtraPurchaseButton.HavePlayNow == false,
            ExtraPurchaseButton.HavePlayNow == false,
            SecondPurchaseButton.HaveTheme[0] == false,
            ExtraPurchaseButton.HavePlayNow == false
        };

        // Get HighScore and StartPosition According to GameMode
        int highScore = (GamePlay.GameMode == 3) ? GamePlay.HighScoreT : GamePlay.HighScoreE;
        byte startPos = (byte)(GamePlay.GameMode == 3 ? _taskTStartPosition : 0);
        byte length = (byte)(GamePlay.GameMode == 3 ? _allTaskE.Length : _taskTStartPosition);

        // Only Update Once
        if (!_isUpdatedCurTask)
        {
            UpdateCurrentTask(highScore, startPos, length);
            _isUpdatedCurTask = true;
        }

        // Find the Task
        for (byte a = startPos; a < length + 1; a++) // +1 for Last Task to be Congrat
        {
            //print($"a : {a}, CurTask : {_curTask}");

            // When Player Achieve the Task
            if (_curTask != a && a > _curTask && _allBoolE[_curTask])
            {
                DisplayIndicatorText(_allReward[_curTask]);

                // Update current Task to be new one
                _curTask = a;

                // Break first before move to next Task
                break;
            }
            // Find the Task
            else if (a < length && highScore <= _allTaskE[a] && _allBoolE[a])
            {
                // Only Indicate if it didn't Indicate previously (So it won't indicate twice)
                if (_prevLevel == -1)
                {
                    // Calculate each Task Score
                    foreach (float each in _taskPercents)
                    {
                        int taskScore = (int)(_allTaskE[a] * Mathf.InverseLerp(0f, _percentMax, each));

                        int currentScore = (GamePlay.GameMode == 3) ? GamePlay.Level - 10 : GameScore.Instance.TotalScore;

                        int intervalRange = (int)Mathf.Ceil(_allTaskE[a] * Mathf.InverseLerp(0f, _percentMax, _intervalPercent));

                        //print($"Each task score for TaskE {_allTaskE[a]} get {each}% = {taskScore}");
                        // If TotalScore is in range of current task Score
                        if (currentScore >= (taskScore - intervalRange) && currentScore <= (taskScore + intervalRange))
                        {
                            //print($"Matches! CurrentScore : {currentScore} is in range of {taskScore - intervalRange} and {taskScore + intervalRange}");
                            DisplayIndicatorText(_allTaskE[a] - currentScore, _allReward[a]);
                            _prevLevel = GamePlay.Level;

                            goto End;
                        }
                    }
                }
                else
                    _prevLevel = -1; // Set back if it indicated on Previous Level

                // Break first before move to next Task
                break;
            }
        }
    End:;
    }

    private void UpdateCurrentTask(int highScore, byte startPos, byte length)
    {
        // Get _curTask To Appropiate Task
        for (byte a = startPos; a < length; a++)
            if (highScore <= _allTaskE[a] && _allBoolE[a])
            {
                _curTask = a;
                return;
            }

        // When Player already Achieved in that Mode, we set Task to be Max so that it won't congrat
        _curTask = length;
    }

    #region --Methods-- (Display Score with text format)
    private void DisplayIndicatorText(int remaining, string reward)
    {
        PlayIndicatorImageAnimation(IndicatorImageAnimator.Open);

        _indicatorText.text = _indicatorGoalText.Replace("'Remaining'", $"{remaining}").Replace("'Reward'", reward);
    }

    private void DisplayIndicatorText(string reward)
    {
        PlayIndicatorImageAnimation(IndicatorImageAnimator.Open);

        _indicatorText.text = _indicatorAchieveText.Replace("'Reward'", reward);
    }
    #endregion

    #region --Methods-- (Play Animations for UI)
    public void PlayIndicatorImageAnimation(IndicatorImageAnimator input)
    {
        switch (input)
        {
            case IndicatorImageAnimator.Close:
                _indicatorImageAnimator.Play("IndicatorImage Idle", -1, 0f);
                break;
            case IndicatorImageAnimator.Open:
                _indicatorImageAnimator.Play("IndicatorImage Get In", -1, 0f);
                break;
        }
    }
    #endregion
    #endregion
}