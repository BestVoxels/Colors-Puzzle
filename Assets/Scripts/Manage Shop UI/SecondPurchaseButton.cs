using UnityEngine;

public class SecondPurchaseButton : MonoBehaviour
{
    // ******** Player save game Theme Product **********
    public static bool[] HaveTheme { get; set; } = new bool[9];

    [SerializeField]
    private Animator _animator;
    private bool _permission = false;

    private void Start()
    {
        // Load HaveTheme from save file (If there is a save) this must run before GameTheme
        if (GameData.IsFileExist("HaveTheme"))
        {
            HaveTheme = GameData.LoadBoolArray("HaveTheme");
        }

        // Update shop UI in GameTheme (cuz we have to refresh each slot accordingly)
    }

    public void PointerDown() => _animator.Play("Press", -1, 0f);
    public void PointerExit() => _permission = false;
    public void PointerEnter() => _permission = true;
    public void PointerUp()
    {
        _animator.Play(_permission ? "Up" : "Idle", -1, 0f);

        if (_permission)
        {
            GameSfx.Instance.PlaySound(GameTheme.Instance.PurchaseButton.interactable ? (byte)0 : (byte)2);

            // IF player HAVE NOT bought the current slot
            if (HaveTheme[GameTheme.CurSlot] == false)
            {
                // Doing iAP stuff (save data there)
                GameIAP.Instance.BuyProducts((byte)(GameTheme.CurSlot + 1));
            }
            // IF player HAVE bought the current slot
            else if (HaveTheme[GameTheme.CurSlot] == true)
            {
                if (TrialButton.TrialStarted == true)
                {
                    // In case player get back to their theme while in trial + explore mode
                    TrialButton.TrialStarted = false;
                    TrialButton.ExploreStarted = false;

                    // This help IF player click buy on same theme & equip it (while exploring) or IF click try on others this will reduce job for UpdateCurTheme to find default slot to exchange
                    GameTheme.Instance.ToDefaultTheme(); // IF PUT outside this if, player can't get back to default theme
                }

                GameTheme.Instance.UpdateCurTheme();

                // Save CurTheme to save file
                GameData.SaveString(GameTheme.CurTheme, "CurTheme");

                GameTheme.Instance.RefreshAllUI();
                GameTheme.Instance.RefreshCurSlot();
            }
        }
    }
}
