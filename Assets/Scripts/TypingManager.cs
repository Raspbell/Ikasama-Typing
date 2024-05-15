using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static QuestionPropety;

public class TypingManager : MonoBehaviour
{

    [SerializeField] QuestionPropety questionPropety;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI romanText;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerModel;
    [SerializeField] GameObject coin;
    [SerializeField] Canvas coinCanvas;

    private QuestionPropety.Question question;
    private List<char> _roman = new List<char>();
    private float rewardPerChar;
    private float noMissBonus;
    private float ikasamaProbability;
    private int romanIndex = 0;
    private bool isWindows;
    private bool isMac;
    private bool isMissed = false;
    private Tween shakeTween;
    private Vector3 defaultPos;
    private Dictionary<char, Vector2> keyPositions;

    private void Start()
    {
        InitializeQuestion();
        InitializeKeyPosition();
        if (SystemInfo.operatingSystem.Contains("Windows"))
        {
            isWindows = true;
        }

        if (SystemInfo.operatingSystem.Contains("Mac"))
        {
            isMac = true;
        }
        StartCoroutine(WaitOneFrame());
    }

    private void Update()
    {
        rewardPerChar = GameManager.rewardPerChar;
        ikasamaProbability = GameManager.ikasamaProbability;
        noMissBonus = GameManager.noMissBonus;
        string testtext = "";
        for (int i = 0; i < _roman.Count; i++)
        {
            if (_roman[i] == '@')
            {
                break;
            }

            testtext += _roman[i];
        }
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            switch (InputKey(GetCharFromKeyCode(Event.current.keyCode)))
            {
                case 1:
                case 2:
                    CorrectInput();
                    break;
                case 3:
                    Vector2 keyPosition;
                    char keyChar = GetCharFromKeyCode(Event.current.keyCode);
                    if (keyPositions.TryGetValue(keyChar, out keyPosition))
                    {
                        List<char> nearbyKeys = new List<char>();
                        List<char> subNearbyKeys = new List<char>();
                        foreach (var keyPos in keyPositions)
                        {
                            int distance = Mathf.Abs((int)keyPos.Value.x - (int)keyPosition.x) + Mathf.Abs((int)keyPos.Value.y - (int)keyPosition.y);
                            if (distance <= (int)Math.Floor(ikasamaProbability / 4))
                            {
                                nearbyKeys.Add(keyPos.Key);
                            }
                            if (distance == (int)Math.Floor(ikasamaProbability / 4) + 1)
                            {
                                subNearbyKeys.Add(keyPos.Key);
                            }
                        }
                        bool correctTypeFlag = false;
                        foreach (char nearbyKey in nearbyKeys)
                        {
                            int result = InputKey(nearbyKey);
                            if (result == 1 || result == 2)
                            {
                                CorrectInput();
                                correctTypeFlag = true;
                                break;
                            }
                        }
                        if (!correctTypeFlag && subNearbyKeys.Count > 0)  // subNearbyKeysが空でない場合のみ実行
                        {
                            foreach (char subNearbyKey in subNearbyKeys)
                            {
                                int result = InputKey(subNearbyKey);
                                if (result == 1 || result == 2)
                                {
                                    float probabilityFraction = ikasamaProbability / 4 - Mathf.Floor(ikasamaProbability / 4);
                                    if (UnityEngine.Random.Range(0f, 1f) < probabilityFraction)
                                    {
                                        CorrectInput();
                                        correctTypeFlag = true;
                                    }
                                    break;
                                }
                            }
                        }
                        if (!correctTypeFlag)
                        {
                            isMissed = true;
                            PlayMissTypeAnimation();
                        }
                    }
                    break;
            }
            if (Event.current.keyCode == KeyCode.Alpha0)
            {
                InitializeQuestion();
            }
        }
    }


    IEnumerator WaitOneFrame()
    {
        yield return null;
        defaultPos = player.transform.localPosition;
    }

    private void InitializeKeyPosition()
    {
        keyPositions = new Dictionary<char, Vector2>
        {
            {'z', new Vector2Int(0, 0)},
            {'x', new Vector2Int(1, 0)},
            {'c', new Vector2Int(2, 0)},
            {'v', new Vector2Int(3, 0)},
            {'b', new Vector2Int(4, 0)},
            {'n', new Vector2Int(5, 0)},
            {'m', new Vector2Int(6, 0)},
            {'a', new Vector2Int(0, 1)},
            {'s', new Vector2Int(1, 1)},
            {'d', new Vector2Int(2, 1)},
            {'f', new Vector2Int(3, 1)},
            {'g', new Vector2Int(4, 1)},
            {'h', new Vector2Int(5, 1)},
            {'j', new Vector2Int(6, 1)},
            {'k', new Vector2Int(7, 1)},
            {'l', new Vector2Int(8, 1)},
            {'q', new Vector2Int(0, 2)},
            {'w', new Vector2Int(1, 2)},
            {'e', new Vector2Int(2, 2)},
            {'r', new Vector2Int(3, 2)},
            {'t', new Vector2Int(4, 2)},
            {'y', new Vector2Int(5, 2)},
            {'u', new Vector2Int(6, 2)},
            {'i', new Vector2Int(7, 2)},
            {'o', new Vector2Int(8, 2)},
            {'p', new Vector2Int(9, 2)},
            {'-', new Vector2Int(10,3)}
        };
    }

    private void CorrectInput()
    {
        romanIndex++;
        if (_roman[romanIndex] == '@')
        {
            int reward = (int)Math.Round(rewardPerChar * question.charCount, MidpointRounding.AwayFromZero);
            if (!isMissed)
            {
                reward = (int)Math.Round(noMissBonus * reward, MidpointRounding.AwayFromZero);
            }
            GameManager.money += reward;
            GameManager.totalMoney += reward;
            isMissed = false;
            InitializeQuestion();
            if (coin != null)
            {
                var coinObj = Instantiate(coin, playerModel.transform.position, Quaternion.identity);
                coinObj.transform.SetParent(coinCanvas.transform);
                coinObj.transform.localScale = Vector3.one;
            }
        }
        else
        {
            romanText.text = GenerateRomanText();
        }
    }

    public void InitializeQuestion()
    {
        question = questionPropety.questions[UnityEngine.Random.Range(0, questionPropety.questions.Length)];

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

    private void UpdateStatus()
    {
        rewardPerChar = GameManager.rewardPerChar;
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
                text += "</style><style=untyped>";
            }

            text += _roman[i];
        }
        text += "</style>";
        return text;
    }

    private void PlayMissTypeAnimation()
    {
        shakeTween.Kill();
        player.transform.localPosition = defaultPos;
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
