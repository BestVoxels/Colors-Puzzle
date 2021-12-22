using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypeThree : MainType
{
    // Pre assign's colors, amount, lists use in randoming
    private Color32[] _colorOfPre = new Color32[2];
    private byte[] _amountOfPre = new byte[2];
    private List<byte> _positionsOfPre = new List<byte>();
    private List<byte> _positionsOfColors = new List<byte>();

    // Contructor
    public TypeThree(float[] timers, float[] minMax) : base(timers, minMax)
    {
    }

    // Method will get call in GamePlay
    public override void NextLevel()
    {
        RefreshTheLevel("increase");
        GamePlay.Instance.OpenUpGrids();
        RefreshGridsColor();
        GenerateChoices();
    }
    public override void RepeatLevel()
    {
        RefreshTheLevel("stay the same");
        RefreshGridsColor();
        GenerateChoices();
    }

    // Type One's methods
    private void RefreshTheLevel(string levelChecker)
    {
        _positionsOfPre.Clear();
        _positionsOfColors.Clear();

        GamePlay.Instance.RefreshTheLevel(levelChecker, Timers[2], Timers[3]);
    }

    // OpenUpGrids will call directly from GamePlay cuz no difference between game types

    private void RefreshGridsColor()
    {
        // Clear all the OpenedImages (waiting for the new one to be assign)
        foreach (Image img in GamePlay.OpenedImages)
        {
            img.color = Color.clear;
        }

        // Add numbers that use to pick up for randomding
        for (byte a = 0; a < GamePlay.OpenedImages.Count; a++)
        {
            _positionsOfPre.Add(a);
        }
        for (byte a = 0; a < GamePlay.GridColorsManaged.Length; a++)
        {
            _positionsOfColors.Add(a);
        }

        // Will both images amount equal or not
        byte equalOrNot = (byte)Random.Range(0, 2); // 0 means both equal, 1 isn't

        // Assign 2 times
        for (byte a = 0; a < _colorOfPre.Length; a++)
        {
            // Random Color for PreAssign & NOT equal among 2 colors, can use in Random OpenedImages colors also
            byte randomedNumber1 = (byte)Random.Range(0, _positionsOfColors.Count);
            byte element1 = _positionsOfColors[randomedNumber1];

            _positionsOfColors.RemoveAt(randomedNumber1);

            _colorOfPre[a] = GamePlay.GridColorsManaged[element1];


            // Random Amount of PreAssign & NOT equal to fisrt one
            float min = (GamePlay.OpenedImages.Count * _minMax[0]) / 100f;
            float max = (GamePlay.OpenedImages.Count * _minMax[1]) / 100f;
        RandomAgain:
            byte temp = (byte)Random.Range(Mathf.CeilToInt(min), Mathf.CeilToInt(max) + 1);

            if (a == 1)
            {
                switch (equalOrNot)
                {
                    case 0:
                        temp = _amountOfPre[0];
                        break;
                    case 1:
                        if (temp == _amountOfPre[0])
                            goto RandomAgain;
                        break;
                }
            }
            _amountOfPre[a] = temp;


            // Assign colorOfPre to question images
            GamePlay.Instance.QuestionImages[a].color = _colorOfPre[a];


            // Random number from the list, removed, and assign the color
            for (byte b = 0; b < _amountOfPre[a]; b++)
            {
                byte randomedNumber2 = (byte)Random.Range(0, _positionsOfPre.Count);
                byte element2 = _positionsOfPre[randomedNumber2];

                _positionsOfPre.RemoveAt(randomedNumber2);

                GamePlay.OpenedImages[element2].color = _colorOfPre[a];
            }
        }

        // Random OpenedImages colors
        foreach (Image img in GamePlay.OpenedImages)
        {
            // Only generate color when its is empty
            if (img.color == Color.clear)
            {
                byte randomedNumber1 = (byte)Random.Range(0, _positionsOfColors.Count);
                byte element1 = _positionsOfColors[randomedNumber1];

                img.color = GamePlay.GridColorsManaged[element1];
            }
        }
    }

    private void GenerateChoices()
    {
        // Assign 2 choices with fix text, T & F
        GamePlay.Instance.AnsButtonsText[0].text = "T";
        GamePlay.Instance.AnsButtonsText[1].text = "F";

        // Assign the correct button
        GamePlay.CorrectButton = (_amountOfPre[0] == _amountOfPre[1]) ? (byte)1 : (byte)2;
    }
}
