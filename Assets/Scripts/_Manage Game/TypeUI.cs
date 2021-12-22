using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Operator1
{
    Increment,
    Decrement
}

public class TypeUI : MonoBehaviour
{
    [SerializeField]
    private Image[] _buttonImage;
    [SerializeField]
    private Sprite[] _typeSprites;
    [SerializeField]
    private Sprite _comingTypeSprite;

    [SerializeField]
    private Text _headerText;
    [SerializeField]
    private Text _infoText;
    [SerializeField]
    private Animator _typePanelAnimator;
    [SerializeField]
    private Animator _typeInfoPanelAnimator;

    [SerializeField]
    private string[,] _typeMessages =
    {
        {"Mix Types", "Randomly choose a Game Type for each level" },
        {"Game Type 1", "Count the number of colors provided" },
        {"Game Type 2", "Match color with the number provided" },
        {"Game Type 3", "The provided colors are EQUAL or NOT" },
        {"Game Type 4", "The provided colors are UNEQUAL or NOT" },
        {"New Game Type", "...Coming Soon..." }
    };
    //New Game Types (add before ...Coming Soon...): "Find 2 colors are UNEQUAL" | "Find 2 colors are EQUAL"

    private string[] _firstTimeMessage =
    {
        "Choose Game Type",
        "Click Arrow below to Choose Game Mode"
    };

    public static TypeUI Instance { get; private set; }
    private void Awake() => Instance = this;

    // This script do NOT need to run after GamePlay cuz doesn't need saves from GamePlay to be loaded
    private void Start() => RefreshSprite();

    private byte GameTypeIncrement()
    {
        if (GamePlay.GameType < GamePlay.MaxType + (GamePlay.HaveNewType ? 1 : 0))
            return (byte)(GamePlay.GameType + 1);
        else
            return 0;
    }

    private byte GameTypeDecrement()
    {
        if (GamePlay.GameType > 0)
            return (byte)(GamePlay.GameType - 1);
        else
            return (byte)(GamePlay.MaxType + (GamePlay.HaveNewType ? 1 : 0));
    }

    // For TypeButton left & right to call
    public void RefreshStuff(Operator1 opt)
    {
        // Set GameType
        GamePlay.GameType = (opt == Operator1.Increment) ? GameTypeIncrement() : GameTypeDecrement();

        RefreshSprite();

        // Save to HasClicked (Even it's not the groundButton but text should change when player click on These button)
        GroundUI.Instance.SaveFirstClick();

        // Update Type Info Text, no play sound
        Fetch();

        ModeUI.Instance.RefreshButtons();
        GameTheme.Instance.RefreshAllUI();
        GameTheme.Instance.RefreshCurSlot(); // calling this to make arrow buttons change to trial color
    }

    // Update Type Text call above and GameState, TypeButton for middle one
    public void Fetch()
    {
        _headerText.text = _typeMessages[GamePlay.GameType, 0];
        _infoText.text = _typeMessages[GamePlay.GameType, 1];

        // When player hasn't click on Ground Button yet
        if (GroundUI.HasClicked == false)
        {
            _headerText.text = _firstTimeMessage[0];
            _infoText.text = _firstTimeMessage[1];
        }
    }

    // For GamePlay to refresh after set GameType
    public void RefreshSprite()
    {
        _buttonImage[0].overrideSprite = (!ModeUI.IsComingType()) ? _typeSprites[GameTypeDecrement()] : _comingTypeSprite;
        _buttonImage[1].overrideSprite = (!ModeUI.IsComingType()) ? _typeSprites[GamePlay.GameType] : _comingTypeSprite;
        _buttonImage[2].overrideSprite = (!ModeUI.IsComingType()) ? _typeSprites[GameTypeIncrement()] : _comingTypeSprite;
    }

    // Methods for other class to open and close
    public void OpenTypePanel() => _typePanelAnimator.Play("TypePanel Get In", -1, 0f);

    public IEnumerator OpenTypeInfoPanel()
    {
        _typeInfoPanelAnimator.Play("TypeInfo Get In", -1, 0f);

        yield return new WaitForSeconds(0.75f);

        if (_typeInfoPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TypeInfo Get In"))
            _typeInfoPanelAnimator.Play("TypeInfo Second Idle", -1, 0f);

        yield break;
    }

    public void CloseTypePanel()
    {
        // If TypePanel is OPEN Close with animation
        if (!_typePanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TypePanel Idle"))
            _typePanelAnimator.Play("TypePanel Idle", -1, 0f);
    }

    public IEnumerator CloseTypeInfoPanel()
    {
        if (!_typeInfoPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TypeInfo Idle"))
            _typeInfoPanelAnimator.Play("TypeInfo Idle", -1, 0f);

        yield return new WaitForSeconds(0.03f);

        if (!_typeInfoPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("TypeInfo Idle"))
            _typeInfoPanelAnimator.Play("TypeInfo Idle", -1, 0f);

        yield break;
    }

    public void SlideLeftTypePanel()
    {
        _typePanelAnimator.Play("TypePanel Left Slide In", -1, 0f);
        _typeInfoPanelAnimator.Play("TypeInfo Left Slide In", -1, 0f);
    }

    public void SlideRightTypePanel()
    {
        _typePanelAnimator.Play("TypePanel Right Slide In", -1, 0f);
        _typeInfoPanelAnimator.Play("TypeInfo Right Slide In", -1, 0f);
    }
}