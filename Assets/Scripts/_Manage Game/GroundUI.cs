using UnityEngine;
using UnityEngine.UI;

public class GroundUI : MonoBehaviour
{
    [SerializeField]
    private Image _buttonImage;
    [SerializeField]
    private Sprite _arrowSprite;
    [SerializeField]
    private Sprite _shareSprite;

    [SerializeField]
    private Animator _groundPanelAnimator;

    private readonly Color32 _blinkColor = new Color32(250, 206, 106, 255);

    public static bool HasClicked { get; private set; } = false;

    public static GroundUI Instance { get; private set; }
    private void Awake() => Instance = this;

    // This script do NOT need to run after GamePlay cuz doesn't need saves from GamePlay to be loaded
    private void Start()
    {
        SetArrowSprite();

        // Load HasClicked from save file (If there is a save)
        if (GameData.IsFileExist("HasClicked"))
        {
            HasClicked = bool.Parse(GameData.LoadString("HasClicked"));
        }
    }

    // For Itself to call when start
    private void SetArrowSprite() => _buttonImage.overrideSprite = _arrowSprite;

    // For GameState to call when GameEnded
    public void SetShareSprite() => _buttonImage.overrideSprite = _shareSprite;

    // For GroundButton/TypeUI to call when clicked
    public void SaveFirstClick()
    {
        if (HasClicked == false)
        {
            HasClicked = true;
            // Save HasClicked to save file
            GameData.SaveString(HasClicked.ToString(), "HasClicked");
        }
    }

    public void OpenPanel(string toBlink)
    {
        _groundPanelAnimator.Play("GroundPanel Get In", -1, 0f);

        if (toBlink == "blink" && HasClicked == false)
            StartCoroutine(GameTheme.Instance.GroundBlink(10, 0.3f, _blinkColor));
    }

    public void ClosePanel()
    {
        // If GroundPanel is OPEN Close with animation
        if (_groundPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("GroundPanel Get In"))
        {
            _groundPanelAnimator.Play("GroundPanel Idle", -1, 0f);
        }
    }
}
