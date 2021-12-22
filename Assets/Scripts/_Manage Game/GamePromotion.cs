using UnityEngine;
using System;
using System.Collections;

public class GamePromotion : MonoBehaviour
{
    // *Things to change here when testing, TargetClose to 1 minute, remove _targetDisplay in IEnumerator to 0.15f;*
    private TimeSpan _difference;
    private const byte _targetScore = 150;
    private short _targetDisplay = 0;
    // For game notification to access its value
    public static short TargetClose { get; } = 240; //4 hours

    // For saving data
    public static sbyte CurShowing { get; private set; } = -1;
    private static sbyte _showCounter = 0;
    public static DateTime OldDate { get; private set; } // being use by gameNotification
    private static TimeSpan _countedTime;

    // Static cuz static method will use it
    private static bool _havePromotion = false;

    private void Start()
    {
        // Load CurShowing from save file (If there is a save)
        if (GameData.IsFileExist("CurShowing"))
        {
            CurShowing = sbyte.Parse(GameData.LoadString("CurShowing"));
        }

        // Load showCounter from save file (If there is a save)
        if (GameData.IsFileExist("ShowCounter"))
        {
            _showCounter = sbyte.Parse(GameData.LoadString("ShowCounter"));
        }
        
        _targetDisplay = (GamePlay.HighScoreE < _targetScore) ? (short)120 : (short)90; // 2 hours OR 1.5 hours


        // Load OldDate from save file (If there is a save)
        if (GameData.IsFileExist("DateTime"))
        {
            OldDate = DateTime.Parse(GameData.LoadString("DateTime"));
        }
        // Save OldDate to save file (If there is no save ONCE the first time)
        else
        {
            OldDate = DateTime.Now;
            GameData.SaveString(OldDate.ToString(), "DateTime");
        }

        // Refresh here cuz CurShowing get loaded here after Start in that finished
        FirstPurchaseButton.Instance.RefreshFirstShop();

        // Check it here cuz Products in First & Second Shops already get loaded
        UpdateHavePromotion();
        // This will instancely enable or disable promotion
        StartCoroutine(UpdatePromotion());
    }

    private IEnumerator UpdatePromotion()
    {
        while (_havePromotion == true && GameState.IsGameStarted == false)
        {
            if (DateTime.Now.Subtract(OldDate) < _countedTime)
            {
                // Assign new oldDate (Making this when Subtract with CurrentTime more than countedTime)
                OldDate = DateTime.Now.Subtract(_countedTime);
                // Save OldDate to save file
                GameData.SaveString(OldDate.ToString(), "DateTime");
            }

            _difference = DateTime.Now.Subtract(OldDate);

            // IF CurShowing is -1...
            if (_difference.TotalMinutes >= _targetDisplay && CurShowing == -1)
            {
                for (sbyte i = _showCounter; i <= 11; i++)
                {
                    _showCounter = (_showCounter < 11) ? ++_showCounter : (sbyte)0;

                    if (CheckTheme(i, false))
                    {
                        SetPromotion(i);

                        // Save showCounter to save file
                        GameData.SaveString(_showCounter.ToString(), "ShowCounter");

                        _difference = DateTime.Now.Subtract(OldDate);

                        break;
                    }
                }
            }

            // IF CurShowing is not -1... (this will update time after its promotion get set)
            if (CurShowing != -1)
            {
                if (_difference.TotalMinutes < TargetClose)
                {
                    // Using .TotalSeconds to minus cuz if 1 sec pass hour,minute would get reduce too
                    byte hour = (byte)( ((TargetClose * 60) - (int)_difference.TotalSeconds) / 3600 % 24 );
                    byte minute = (byte)( ((TargetClose * 60) - (int)_difference.TotalSeconds) / 60 % 60 );

                    if (hour == 0)
                    {
                        byte second = (byte)( ((TargetClose * 60) - (int)_difference.TotalSeconds) % 60 );

                        if (minute == 0)
                        {
                            FirstPurchaseButton.Instance.AddShopInfoText(second + "s");
                        }
                        else
                        {
                            FirstPurchaseButton.Instance.AddShopInfoText((second != 0) ? minute + "m " + second + "s" : minute + "m");
                        }
                    }
                    else
                    {
                        FirstPurchaseButton.Instance.AddShopInfoText((minute != 0) ? hour + "h " + minute + "m" : hour + "h");
                    }
                }
                else
                {
                    SetPromotion(-1);
                }
            }

            // should be 0.2 cuz when get back it should refresh immediately & counting seconds accurately
            yield return new WaitForSecondsRealtime(0.2f);
        }

        yield break;
    }

    public static void SetPromotion(sbyte promotion)
    {
        CurShowing = promotion;

        // Save CurShowing to save file
        GameData.SaveString(CurShowing.ToString(), "CurShowing");

        GameTheme.CanBlink = true; // Set it only once when promotion comes or removed
        
        GameTheme.Instance.RefreshAllUI();
        GameTheme.Instance.RefreshCurSlot(); // calling this to make arrow buttons change to trial color

        FirstPurchaseButton.Instance.RefreshFirstShop();

        // Assign new oldDate
        OldDate = DateTime.Now;
        // Save OldDate to save file
        GameData.SaveString(OldDate.ToString(), "DateTime");

        // Reset countedTime
        _countedTime = TimeSpan.Zero;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // Save difference to CountedTime file
            GameData.SaveString(_difference.ToString(), "CountedTime");
        }
        else
        {
            // Load countedTime from save file (If there is a save)
            if (GameData.IsFileExist("CountedTime"))
            {
                _countedTime = TimeSpan.Parse(GameData.LoadString("CountedTime"));
            }
        }
    }

    public static bool CheckTheme(sbyte checkAt, bool cond)
    {
        switch (checkAt)
        {
            case 0:
                for (int i = 1; i < SecondPurchaseButton.HaveTheme.Length; i++)
                {
                    if (SecondPurchaseButton.HaveTheme[i] == !cond || FirstPurchaseButton.HaveJustRevive == !cond)
                    {
                        return false;
                    }
                }
                return true;
            case 1:
                return FirstPurchaseButton.HaveJustRevive == cond && SecondPurchaseButton.HaveTheme[1] == cond && SecondPurchaseButton.HaveTheme[2] == cond;
            case 2:
                return FirstPurchaseButton.HaveJustRevive == cond && SecondPurchaseButton.HaveTheme[3] == cond && SecondPurchaseButton.HaveTheme[4] == cond;
            case 3:
                return FirstPurchaseButton.HaveJustRevive == cond && SecondPurchaseButton.HaveTheme[5] == cond && SecondPurchaseButton.HaveTheme[6] == cond;
            case 4:
                return FirstPurchaseButton.HaveJustRevive == cond && SecondPurchaseButton.HaveTheme[7] == cond && SecondPurchaseButton.HaveTheme[8] == cond;

            case 5:
                for (int i = 1; i < SecondPurchaseButton.HaveTheme.Length; i++)
                {
                    if (SecondPurchaseButton.HaveTheme[i] == !cond)
                    {
                        return false;
                    }
                }
                return true;
            case 6:
                return SecondPurchaseButton.HaveTheme[3] == cond && SecondPurchaseButton.HaveTheme[4] == cond && SecondPurchaseButton.HaveTheme[5] == cond;
            case 7:
                return SecondPurchaseButton.HaveTheme[6] == cond && SecondPurchaseButton.HaveTheme[7] == cond && SecondPurchaseButton.HaveTheme[8] == cond;
            case 8:
                return SecondPurchaseButton.HaveTheme[1] == cond && SecondPurchaseButton.HaveTheme[2] == cond && SecondPurchaseButton.HaveTheme[5] == cond;
            case 9:
                return SecondPurchaseButton.HaveTheme[1] == cond && SecondPurchaseButton.HaveTheme[2] == cond && SecondPurchaseButton.HaveTheme[3] == cond;
            case 10:
                return SecondPurchaseButton.HaveTheme[4] == cond && SecondPurchaseButton.HaveTheme[7] == cond && SecondPurchaseButton.HaveTheme[8] == cond;
            case 11:
                return SecondPurchaseButton.HaveTheme[4] == cond && SecondPurchaseButton.HaveTheme[5] == cond && SecondPurchaseButton.HaveTheme[6] == cond;

            default:
                return false;
        }
    }

    public static void UpdateHavePromotion()
    {
        for (sbyte i = 0; i < 12; i++)
        {
            if (CheckTheme(i, false))
            {
                _havePromotion = true;
                return;
            }
        }
        _havePromotion = false;
    }
}
