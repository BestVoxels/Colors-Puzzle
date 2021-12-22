using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypeOne : MainType
{
    // Pre assign's colors, amount, list use in randoming
    private Color32 _colorOfPre;
    private byte _amountOfPre;
    private List<byte> _positionsOfPre = new List<byte>();

    // For Choices
    private byte[] _choices = new byte[3];
    private List<byte> _choicesNumbers = new List<byte>();

    // Contructor
    public TypeOne(float[] timers, float[] minMax) : base(timers, minMax)
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
        _choices[0] = 0;
        _choices[1] = 0;

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

        // Random Color for PreAssign & Assign it to question
        _colorOfPre = GamePlay.GridColorsManaged[Random.Range(0, GamePlay.GridColorsManaged.Length)];
        GamePlay.Instance.QuestionImages[0].color = _colorOfPre;

        // Random Amount of PreAssign
        float min = (GamePlay.OpenedImages.Count * _minMax[0]) / 100f;
        float max = (GamePlay.OpenedImages.Count * _minMax[1]) / 100f;
        _amountOfPre = (byte)Random.Range(Mathf.CeilToInt(min), Mathf.CeilToInt(max) + 1);

        // Add numbers that use to pick up for randomding
        for (byte a = 0; a < GamePlay.OpenedImages.Count; a++)
        {
            _positionsOfPre.Add(a);
        }
        // Random number from the list, removed, and assign the color
        for (byte a = 0; a < _amountOfPre; a++)
        {
            byte randomedNumber = (byte)Random.Range(0, _positionsOfPre.Count);
            byte element = _positionsOfPre[randomedNumber];

            _positionsOfPre.RemoveAt(randomedNumber);

            GamePlay.OpenedImages[element].color = _colorOfPre;
        }

        // Random OpenedImages colors
        foreach (Image img in GamePlay.OpenedImages)
        {
            // Only generate color when its is empty
            if (img.color == Color.clear)
            {
            RandomColor:
                Color32 temp = GamePlay.GridColorsManaged[Random.Range(0, GamePlay.GridColorsManaged.Length)];

                // if answer's color equal temp
                if (_colorOfPre.Equals(temp))
                {
                    goto RandomColor;
                }

                img.color = temp;
            }
        }
    }

    private void GenerateChoices()
    {
        byte position = (byte)Random.Range(1, 4); // 1 answer is the lowest, 2 middle, 3 highest
        byte max = (position == 2) ? (byte)2 : (byte)3;

        if (position != 1 && (_amountOfPre - max) <= 0)
        {
            position = 1;
            max = 3;
        }

        for (byte a = 0; a < 2; a++)
        {
        RandomChoices:
            byte value = (byte)Random.Range(1, max + 1);

            switch (position)
            {
                case 1:
                    _choices[a] = (byte)(_amountOfPre + value);
                    break;

                case 2:
                    _choices[a] = (a == 0) ? (byte)(_amountOfPre - value) : (byte)(_amountOfPre + value);
                    break;

                case 3:
                    _choices[a] = (byte)(_amountOfPre - value);
                    break;
            }

            if (_choices[0] == _choices[1])
                goto RandomChoices;
        }

        // Assign the last Choice with correct answer
        _choices[2] = _amountOfPre;

        // Randomly place choices in each button
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

            GamePlay.Instance.AnsButtonsText[a].text = "" + _choices[element];

            // Know which button is the correct one
            if (_choices[element] == _amountOfPre)
            {
                GamePlay.CorrectButton = (byte)(a + 1);
            }
        }
    }
}