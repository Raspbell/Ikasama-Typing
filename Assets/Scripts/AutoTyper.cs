using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class AutoTyper : MonoBehaviour
{

    [SerializeField] QuestionPropety questionPropety;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI romanText;

    public float time;
    private QuestionPropety.Question question;
    private List<char> roman = new List<char>();
    private GameManager gameManager;
    private int romanIndex = 0;

    private int workersNum;
    private int rewardPerChar;
    private float typingCycle;
    private float missTypeProbability;
    private float missTypePenalty;

    private void Start()
    {
        time = 0;
        gameManager = FindObjectOfType<GameManager>();
        InitializeQuestion();
    }

    private void Update()
    {
        UpdateStatus();
        time += Time.deltaTime;
        if(time > typingCycle)
        {
            time = 0;
            if(missTypeProbability < Random.Range(0f, 1f)) {
                romanIndex++;
                if (roman[romanIndex] == '@') {
                    int reward = rewardPerChar * question.title.Length;
                    gameManager.money += reward;
                    gameManager.totalMoney += reward;
                    InitializeQuestion();
                }
                else {
                    romanText.text = GenerateRomanText();
                }
            }
            else {
                time -= missTypePenalty;
                MissTypeAnimation();
            }
        }
    }

    private void UpdateStatus() {
        workersNum = gameManager.workersNum.Value;
        rewardPerChar = gameManager.rewardPerChar;
        typingCycle = gameManager.typingCycle;
        missTypeProbability = gameManager.missTypeProbability;
        missTypePenalty = gameManager.missTypePenalty;
    }
    private void MissTypeAnimation()
    {
        transform.DOShakePosition(0.1f, 5f, 30, 1, false, true);
    }

    private string GenerateRomanText()
    {
        string text = "<style=typed>";
        for (int i = 0; i < roman.Count; i++)
        {
            if (roman[i] == '@')
            {
                break;
            }
            if (i == romanIndex)
            {
                text += "</style><style=untyped>";
            }

            text += roman[i];
        }
        text += "</style>";
        return text;
    }

    private void InitializeQuestion()
    {
        question = questionPropety.questions[UnityEngine.Random.Range(0, questionPropety.questions.Length)];

        romanIndex = 0;
        roman.Clear();

        char[] characters = question.roman.ToCharArray();

        foreach (char character in characters)
        {
            roman.Add(character);
        }

        roman.Add('@');

        titleText.text = question.title;
        romanText.text = GenerateRomanText();
    }

}
