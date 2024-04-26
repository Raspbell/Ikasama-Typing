using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TypingManager : MonoBehaviour
{
    public QuestionPropety questionPropety;

    private List<char> _roman = new List<char>();
    private int _romanIndex = 0;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI romanText;

    private void Start()
    {
        InitializeQuestion();
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            switch (InputKey(GetCharFromKeyCode(Event.current.keyCode)))
            {
                case 1:
                case 2:
                    _romanIndex++;
                    if (_roman[_romanIndex] == '@')
                    {
                        InitializeQuestion();
                    }
                    else
                    {
                        romanText.text = GenerateRomanText();
                    }
                    break;
                case 3:
                    // ここにミスタイプ時の処理を記述する
                    break;
            }
        }
    }

    private void InitializeQuestion()
    {
        QuestionPropety.Question question = questionPropety.questions[UnityEngine.Random.Range(0, questionPropety.questions.Length)];

        _romanIndex = 0;
        _roman.Clear();

        char[] characters = question.roman.ToCharArray();

        foreach (char character in characters)
        {
            _roman.Add(character);
        }

        _roman.Add('@');

        titleText.text = question.title;
        romanText.text = GenerateRomanText();
    }

    private int InputKey(char inputChar)
    {
        char currentChar = _roman[_romanIndex];

        if (inputChar == '\0')
        {
            return 0;
        }

        if (inputChar == currentChar)
        {
            return 1;
        }

        return 3;
    }

    private string GenerateRomanText()
    {
        string text = "<style=typed>";
        for (int i = 0; i < _roman.Count; i++)
        {
            if (_roman[i] == '@')
            {
                break;
            }
            if (i == _romanIndex)
            {
                text += "</style><style=untyped>";
            }

            text += _roman[i];
        }
        text += "</style>";
        return text;
    }

    private char GetCharFromKeyCode(KeyCode keyCode)
    {
        if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
            char character = (char)((int)keyCode - KeyCode.A + 'a');
            return character;
        }
        else
        {
            switch (keyCode)
            {
                case KeyCode.Alpha0:
                    return '0';
                case KeyCode.Alpha1:
                    return '1';
                case KeyCode.Alpha2:
                    return '2';
                case KeyCode.Alpha3:
                    return '3';
                case KeyCode.Alpha4:
                    return '4';
                case KeyCode.Alpha5:
                    return '5';
                case KeyCode.Alpha6:
                    return '6';
                case KeyCode.Alpha7:
                    return '7';
                case KeyCode.Alpha8:
                    return '8';
                case KeyCode.Alpha9:
                    return '9';
                case KeyCode.Minus:
                    return '-';
                case KeyCode.Caret:
                    return '^';
                case KeyCode.Backslash:
                    return '\\';
                case KeyCode.At:
                    return '@';
                case KeyCode.LeftBracket:
                    return '[';
                case KeyCode.Semicolon:
                    return ';';
                case KeyCode.Colon:
                    return ':';
                case KeyCode.RightBracket:
                    return ']';
                case KeyCode.Comma:
                    return ',';
                case KeyCode.Period:
                    return '_';
                case KeyCode.Slash:
                    return '/';
                case KeyCode.Underscore:
                    return '_';
                case KeyCode.Backspace:
                    return '\b';
                case KeyCode.Return:
                    return '\r';
                case KeyCode.Space:
                    return ' ';
                default:
                    return '\0';
            }
        }
    }
}
