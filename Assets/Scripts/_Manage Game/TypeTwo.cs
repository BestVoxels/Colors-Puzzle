using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypeTwo : MainType
{
    // Pre assign's colors, amount, lists use in randoming
    private Color32[] _colorOfPre = new Color32[3];
    private byte[] _amountOfPre = new byte[3];
    private List<byte> _positionsOfPre = new List<byte>();
    private List<byte> _positionsOfColors = new List<byte>();

    // For Assign color to randomly to choice
    private List<byte> _choicesNumbers = new List<byte>();

    // Contructor
    public TypeTwo(float[] timers, float[] minMax) : base(timers, minMax)
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
        _choicesNumbers.Clear();
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

        // Assign 3 times
        for (byte a = 0; a < _colorOfPre.Length; a++)
        {
            // Random Color for PreAssign & NOT equal among 3 colors, can use in Random OpenedImages colors also
            byte randomedNumber1 = (byte)Random.Range(0, _positionsOfColors.Count);
            byte element1 = _positionsOfColors[randomedNumber1];

            _positionsOfColors.RemoveAt(randomedNumber1);

            _colorOfPre[a] = GamePlay.GridColorsManaged[element1];


            // Random Amount of PreAssign & NOT equal to fisrt one
            float min = (GamePlay.OpenedImages.Count * _minMax[0]) / 100f;
            float max = (GamePlay.OpenedImages.Count * _minMax[1]) / 100f;
        RandomAgain:
            byte temp = (byte)Random.Range(Mathf.CeilToInt(min), Mathf.CeilToInt(max) + 1);

            if (temp == _amountOfPre[0])
            {
                goto RandomAgain;
            }
            _amountOfPre[a] = temp;


            // Random number from the list, removed, and assign the color
            for (byte b = 0; b < _amountOfPre[a]; b++)
            {
                byte randomedNumber2 = (byte)Random.Range(0, _positionsOfPre.Count);
                byte element2 = _positionsOfPre[randomedNumber2];

                _positionsOfPre.RemoveAt(randomedNumber2);

                GamePlay.OpenedImages[element2].color = _colorOfPre[a];
            }
        }

        // Assign first amountOfPre to question text
        GamePlay.Instance.QuestionText.text = _amountOfPre[0] + " = ?";

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
        // Randomly place PreAssign in each button
        // Assign chicesNumbers with 0,1,2
        for (byte a = 0; a < 3; a++)
        {
            _choicesNumbers.Add(a);
        }

        // Get random number from choicesNumbers 3 times
        for (byte a = 0; a < 3; a++)
        {
            byte randomedNumber = (byte)Random.Range(0, _choicesNumbers.Count);
            byte element = _choicesNumbers[randomedNumber];

            _choicesNumbers.RemoveAt(randomedNumber);

            byte indexer = (byte)(a * 2);
            GamePlay.Instance.AnsPanelsImage[indexer].color = _colorOfPre[element];

            // Know which button is the correct one
            if (GamePlay.Instance.AnsPanelsImage[indexer].color.Equals(_colorOfPre[0]))
            {
                GamePlay.CorrectButton = (byte)(a + 1);
            }
        }
    }
}
