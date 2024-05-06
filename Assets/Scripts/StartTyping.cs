using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartTyping : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI romanText;
    [SerializeField] GameObject player;

    private QuestionPropety.Question question;
    private List<char> _roman = new List<char>();
    private int romanIndex = 0;
    private bool isWindows;
    private bool isMac;
    private Tween shakeTween;

    private void Start()
    {

        DetectOperatingSystem();
        InitializeQuestion();
        if (SystemInfo.operatingSystem.Contains("Windows"))
        {
            isWindows = true;
        }

        if (SystemInfo.operatingSystem.Contains("Mac"))
        {
            isMac = true;
        }
    }

    private void DetectOperatingSystem()
    {
        isWindows = SystemInfo.operatingSystem.Contains("Windows");
        isMac = SystemInfo.operatingSystem.Contains("Mac");
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            print(Event.current.keyCode);
            switch (InputKey(GetCharFromKeyCode(Event.current.keyCode)))
            {
                case 0:
                    break;
                case 1:
                case 2:
                    CorrectInput();
                    break;
                case 3:
                    PlayMissTypeAnimation();
                    break;
            }
        }
    }


    private void CorrectInput()
    {
        romanIndex++;
        if (_roman[romanIndex] == '@')
        {
            player.transform.DOScale(0, 1f).OnComplete(() =>
            {
                SceneManager.LoadScene("Main");
            });
        }
        else
        {
            romanText.text = GenerateRomanText();
        }
    }

    public void InitializeQuestion()
    {
        question = new QuestionPropety.Question();
        question.title = "はじめる";
        question.roman = "hazimeru";
        question.charCount = 0;

        romanIndex = 0;
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
        char prevChar3 = romanIndex >= 3 ? _roman[romanIndex - 3] : '\0';
        char prevChar2 = romanIndex >= 2 ? _roman[romanIndex - 2] : '\0';
        char prevChar = romanIndex >= 1 ? _roman[romanIndex - 1] : '\0';
        char currentChar = _roman[romanIndex];
        char nextChar = _roman[romanIndex + 1];
        char nextChar2 = nextChar == '@' ? '@' : _roman[romanIndex + 2];

        if (inputChar == '\0')
        {
            return 0;
        }

        if (inputChar == currentChar)
        {
            return 1;
        }



        //「い」の曖昧入力判定（Windowsのみ）

        if (isWindows && inputChar == 'y' && currentChar == 'i' &&
            (prevChar == '\0' || prevChar == 'a' || prevChar == 'i' || prevChar == 'u' || prevChar == 'e' ||
             prevChar == 'o'))
        {
            _roman.Insert(romanIndex, 'y');
            return 2;
        }

        if (isWindows && inputChar == 'y' && currentChar == 'i' && prevChar == 'n' && prevChar2 == 'n' &&
            prevChar3 != 'n')
        {
            _roman.Insert(romanIndex, 'y');
            return 2;
        }

        if (isWindows && inputChar == 'y' && currentChar == 'i' && prevChar == 'n' && prevChar2 == 'x')
        {
            _roman.Insert(romanIndex, 'y');
            return 2;
        }

        //「う」の曖昧入力判定（「whu」はWindowsのみ）
        if (inputChar == 'w' && currentChar == 'u' && (prevChar == '\0' || prevChar == 'a' || prevChar == 'i' ||
                                                       prevChar == 'u' || prevChar == 'e' || prevChar == 'o'))
        {
            _roman.Insert(romanIndex, 'w');
            return 2;
        }

        if (inputChar == 'w' && currentChar == 'u' && prevChar == 'n' && prevChar2 == 'n' && prevChar3 != 'n')
        {
            _roman.Insert(romanIndex, 'w');
            return 2;
        }

        if (inputChar == 'w' && currentChar == 'u' && prevChar == 'n' && prevChar2 == 'x')
        {
            _roman.Insert(romanIndex, 'w');
            return 2;
        }

        if (isWindows && inputChar == 'h' && prevChar2 != 't' && prevChar2 != 'd' && prevChar == 'w' &&
            currentChar == 'u')
        {
            _roman.Insert(romanIndex, 'h');
            return 2;
        }

        //「か」「く」「こ」の曖昧入力判定（Windowsのみ）
        if (isWindows && inputChar == 'c' && prevChar != 'k' && currentChar == 'k' && (nextChar == 'a' || nextChar == 'u' || nextChar == 'o'))
        {
            _roman[romanIndex] = 'c';
            return 2;
        }


        //「く」の曖昧入力判定（Windowsのみ）
        if (isWindows && inputChar == 'q' && prevChar != 'k' && currentChar == 'k' && nextChar == 'u')
        {
            _roman[romanIndex] = 'q';
            return 2;
        }

        //「し」の曖昧入力判定
        if (inputChar == 'h' && prevChar == 's' && currentChar == 'i')
        {
            _roman.Insert(romanIndex, 'h');
            return 2;
        }

        //「じ」の曖昧入力判定
        if (inputChar == 'j' && currentChar == 'z' && nextChar == 'i')
        {
            _roman[romanIndex] = 'j';
            return 2;
        }

        //「しゃ」「しゅ」「しぇ」「しょ」の曖昧入力判定
        if (inputChar == 'h' && prevChar == 's' && currentChar == 'y')
        {
            _roman[romanIndex] = 'h';
            return 2;
        }

        //「じゃ」「じゅ」「じぇ」「じょ」の曖昧入力判定
        if (inputChar == 'z' && prevChar != 'j' && currentChar == 'j' &&
            (nextChar == 'a' || nextChar == 'u' || nextChar == 'e' || nextChar == 'o'))
        {
            _roman[romanIndex] = 'z';
            _roman.Insert(romanIndex + 1, 'y');
            return 2;
        }

        //「し」「せ」の曖昧入力判定（Windowsのみ）
        if (isWindows && inputChar == 'c' && prevChar != 's' && currentChar == 's' &&
            (nextChar == 'i' || nextChar == 'e'))
        {
            _roman[romanIndex] = 'c';
            return 2;
        }

        //「ち」の曖昧入力判定
        if (inputChar == 'c' && prevChar != 't' && currentChar == 't' && nextChar == 'i')
        {
            _roman[romanIndex] = 'c';
            _roman.Insert(romanIndex + 1, 'h');
            return 2;
        }

        //「ちゃ」「ちゅ」「ちぇ」「ちょ」の曖昧入力判定
        if (inputChar == 'c' && prevChar != 't' && currentChar == 't' && nextChar == 'y')
        {
            _roman[romanIndex] = 'c';
            return 2;
        }

        //「cya」=>「cha」
        if (inputChar == 'h' && prevChar == 'c' && currentChar == 'y')
        {
            _roman[romanIndex] = 'h';
            return 2;
        }

        //「つ」の曖昧入力判定
        if (inputChar == 's' && prevChar == 't' && currentChar == 'u')
        {
            _roman.Insert(romanIndex, 's');
            return 2;
        }

        //「つぁ」「つぃ」「つぇ」「つぉ」の分解入力判定
        if (inputChar == 'u' && prevChar == 't' && currentChar == 's' &&
            (nextChar == 'a' || nextChar == 'i' || nextChar == 'e' || nextChar == 'o'))
        {
            _roman[romanIndex] = 'u';
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        if (inputChar == 'u' && prevChar2 == 't' && prevChar == 's' &&
            (currentChar == 'a' || currentChar == 'i' || currentChar == 'e' || currentChar == 'o'))
        {
            _roman.Insert(romanIndex, 'u');
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        //「てぃ」の分解入力判定
        if (inputChar == 'e' && prevChar == 't' && currentChar == 'h' && nextChar == 'i')
        {
            _roman[romanIndex] = 'e';
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        //「でぃ」の分解入力判定
        if (inputChar == 'e' && prevChar == 'd' && currentChar == 'h' && nextChar == 'i')
        {
            _roman[romanIndex] = 'e';
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        //「でゅ」の分解入力判定
        if (inputChar == 'e' && prevChar == 'd' && currentChar == 'h' && nextChar == 'u')
        {
            _roman[romanIndex] = 'e';
            _roman.Insert(romanIndex + 1, 'x');
            _roman.Insert(romanIndex + 2, 'y');
            return 2;
        }

        //「とぅ」の分解入力判定
        if (inputChar == 'o' && prevChar == 't' && currentChar == 'w' && nextChar == 'u')
        {
            _roman[romanIndex] = 'o';
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        //「どぅ」の分解入力判定
        if (inputChar == 'o' && prevChar == 'd' && currentChar == 'w' && nextChar == 'u')
        {
            _roman[romanIndex] = 'o';
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        //「ふ」の曖昧入力判定
        if (inputChar == 'f' && currentChar == 'h' && nextChar == 'u')
        {
            _roman[romanIndex] = 'f';
            return 2;
        }

        //「ふぁ」「ふぃ」「ふぇ」「ふぉ」の分解入力判定（一部Macのみ）
        if (inputChar == 'w' && prevChar == 'f' &&
            (currentChar == 'a' || currentChar == 'i' || currentChar == 'e' || currentChar == 'o'))
        {
            _roman.Insert(romanIndex, 'w');
            return 2;
        }

        if (inputChar == 'y' && prevChar == 'f' && (currentChar == 'i' || currentChar == 'e'))
        {
            _roman.Insert(romanIndex, 'y');
            return 2;
        }

        if (inputChar == 'h' && prevChar != 'f' && currentChar == 'f' &&
            (nextChar == 'a' || nextChar == 'i' || nextChar == 'e' || nextChar == 'o'))
        {
            if (isMac)
            {
                _roman[romanIndex] = 'h';
                _roman.Insert(romanIndex + 1, 'w');
            }
            else
            {
                _roman[romanIndex] = 'h';
                _roman.Insert(romanIndex + 1, 'u');
                _roman.Insert(romanIndex + 2, 'x');
            }

            return 2;
        }

        if (inputChar == 'u' && prevChar == 'f' &&
            (currentChar == 'a' || currentChar == 'i' || currentChar == 'e' || currentChar == 'o'))
        {
            _roman.Insert(romanIndex, 'u');
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        if (isMac && inputChar == 'u' && prevChar == 'h' && currentChar == 'w' &&
            (nextChar == 'a' || nextChar == 'i' || nextChar == 'e' || nextChar == 'o'))
        {
            _roman[romanIndex] = 'u';
            _roman.Insert(romanIndex + 1, 'x');
            return 2;
        }

        //「ん」の曖昧入力判定（「n'」には未対応）
        if (inputChar == 'n' && prevChar2 != 'n' && prevChar == 'n' && currentChar != 'a' && currentChar != 'i' &&
            currentChar != 'u' && currentChar != 'e' && currentChar != 'o' && currentChar != 'y')
        {
            _roman.Insert(romanIndex, 'n');
            return 2;
        }

        if (inputChar == 'x' && prevChar != 'n' && currentChar == 'n' && nextChar != 'a' && nextChar != 'i' &&
            nextChar != 'u' && nextChar != 'e' && nextChar != 'o' && nextChar != 'y')
        {
            if (nextChar == 'n')
            {
                _roman[romanIndex] = 'x';
            }
            else
            {
                _roman.Insert(romanIndex, 'x');
            }

            return 2;
        }


        //「きゃ」「にゃ」などを分解する
        if (inputChar == 'i' && currentChar == 'y' &&
            (prevChar == 'k' || prevChar == 's' || prevChar == 't' || prevChar == 'n' || prevChar == 'h' ||
             prevChar == 'm' || prevChar == 'r' || prevChar == 'g' || prevChar == 'z' || prevChar == 'd' ||
             prevChar == 'b' || prevChar == 'p') &&
            (nextChar == 'a' || nextChar == 'u' || nextChar == 'e' || nextChar == 'o'))
        {
            if (nextChar == 'e')
            {
                _roman[romanIndex] = 'i';
                _roman.Insert(romanIndex + 1, 'x');
            }
            else
            {
                _roman.Insert(romanIndex, 'i');
                _roman.Insert(romanIndex + 1, 'x');
            }

            return 2;
        }

        //「しゃ」「ちゃ」などを分解する
        if (inputChar == 'i' &&
            (currentChar == 'a' || currentChar == 'u' || currentChar == 'e' || currentChar == 'o') &&
            (prevChar2 == 's' || prevChar2 == 'c') && prevChar == 'h')
        {
            if (nextChar == 'e')
            {
                _roman.Insert(romanIndex, 'i');
                _roman.Insert(romanIndex + 1, 'x');
            }
            else
            {
                _roman.Insert(romanIndex, 'i');
                _roman.Insert(romanIndex + 1, 'x');
                _roman.Insert(romanIndex + 2, 'y');
            }

            return 2;
        }

        //「しゃ」を「c」で分解する（Windows限定）
        if (isWindows && inputChar == 'c' && currentChar == 's' && prevChar != 's' && nextChar == 'y' &&
            (nextChar2 == 'a' || nextChar2 == 'u' || nextChar2 == 'e' || nextChar2 == 'o'))
        {
            if (nextChar2 == 'e')
            {
                _roman[romanIndex] = 'c';
                _roman[romanIndex + 1] = 'i';
                _roman.Insert(romanIndex + 1, 'x');
            }
            else
            {
                _roman[romanIndex] = 'c';
                _roman.Insert(romanIndex + 1, 'i');
                _roman.Insert(romanIndex + 2, 'x');
            }

            return 2;
        }

        //「っ」の分解入力判定
        if ((inputChar == 'x' || inputChar == 'l') &&
            (currentChar == 'k' && nextChar == 'k' || currentChar == 's' && nextChar == 's' ||
             currentChar == 't' && nextChar == 't' || currentChar == 'g' && nextChar == 'g' ||
             currentChar == 'z' && nextChar == 'z' || currentChar == 'j' && nextChar == 'j' ||
             currentChar == 'd' && nextChar == 'd' || currentChar == 'b' && nextChar == 'b' ||
             currentChar == 'p' && nextChar == 'p'))
        {
            _roman[romanIndex] = inputChar;
            _roman.Insert(romanIndex + 1, 't');
            _roman.Insert(romanIndex + 2, 'u');
            return 2;
        }

        //「っか」「っく」「っこ」の特殊入力（Windows限定）
        if (isWindows && inputChar == 'c' && currentChar == 'k' && nextChar == 'k' &&
            (nextChar2 == 'a' || nextChar2 == 'u' || nextChar == 'o'))
        {
            _roman[romanIndex] = 'c';
            _roman[romanIndex + 1] = 'c';
            return 2;
        }

        //「っく」の特殊入力（Windows限定）
        if (isWindows && inputChar == 'q' && currentChar == 'k' && nextChar == 'k' && nextChar2 == 'u')
        {
            _roman[romanIndex] = 'q';
            _roman[romanIndex + 1] = 'q';
            return 2;
        }

        //「っし」「っせ」の特殊入力（Windows限定）
        if (isWindows && inputChar == 'c' && currentChar == 's' && nextChar == 's' &&
            (nextChar2 == 'i' || nextChar2 == 'e'))
        {
            _roman[romanIndex] = 'c';
            _roman[romanIndex + 1] = 'c';
            return 2;
        }
        //「っちゃ」「っちゅ」「っちぇ」「っちょ」の曖昧入力判定
        if (inputChar == 'c' && currentChar == 't' && nextChar == 't' && nextChar2 == 'y')
        {
            _roman[romanIndex] = 'c';
            _roman[romanIndex + 1] = 'c';
            return 2;
        }
        //「っち」の曖昧入力判定
        if (inputChar == 'c' && currentChar == 't' && nextChar == 't' && nextChar2 == 'i')
        {
            _roman[romanIndex] = 'c';
            _roman[romanIndex + 1] = 'c';
            _roman.Insert(romanIndex + 2, 'h');
            return 2;
        }

        //「l」と「x」の完全互換性
        if (inputChar == 'x' && currentChar == 'l')
        {
            _roman[romanIndex] = 'x';
            return 2;
        }

        if (inputChar == 'l' && currentChar == 'x')
        {
            _roman[romanIndex] = 'l';
            return 2;
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
            if (i == romanIndex)
            {
                text += "</style><style=untyped_title>";
            }

            text += _roman[i];
        }
        text += "</style>";
        return text;
    }

    private void PlayMissTypeAnimation()
    {
        shakeTween.Kill();
        shakeTween = player.transform.DOShakePosition(0.1f, 5f, 30, 1, false, true);
    }

    char GetCharFromKeyCode(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.A:
                return 'a';
            case KeyCode.B:
                return 'b';
            case KeyCode.C:
                return 'c';
            case KeyCode.D:
                return 'd';
            case KeyCode.E:
                return 'e';
            case KeyCode.F:
                return 'f';
            case KeyCode.G:
                return 'g';
            case KeyCode.H:
                return 'h';
            case KeyCode.I:
                return 'i';
            case KeyCode.J:
                return 'j';
            case KeyCode.K:
                return 'k';
            case KeyCode.L:
                return 'l';
            case KeyCode.M:
                return 'm';
            case KeyCode.N:
                return 'n';
            case KeyCode.O:
                return 'o';
            case KeyCode.P:
                return 'p';
            case KeyCode.Q:
                return 'q';
            case KeyCode.R:
                return 'r';
            case KeyCode.S:
                return 's';
            case KeyCode.T:
                return 't';
            case KeyCode.U:
                return 'u';
            case KeyCode.V:
                return 'v';
            case KeyCode.W:
                return 'w';
            case KeyCode.X:
                return 'x';
            case KeyCode.Y:
                return 'y';
            case KeyCode.Z:
                return 'z';
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
                return '.';
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
            default: //上記以外のキーが押された場合は「null文字」を返す。
                return '\0';
        }
    }
}
