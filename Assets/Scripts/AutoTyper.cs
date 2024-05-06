using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using System;

public class AutoTyper : MonoBehaviour
{

    [SerializeField] QuestionPropety questionPropety;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI romanText;
    [SerializeField] AudioClip[] typeSounds;
    [SerializeField] Animator animator;
    [SerializeField] GameObject coin;
    [HideInInspector] public float time;

    private QuestionPropety.Question question;
    private List<char> roman = new List<char>();
    private bool isMissed = false;
    private GameManager gameManager;
    private AudioSource audioSource;
    private Canvas coinCanvas;
    private Vector3 defaultPos;
    private Tween shakeTween;
    private int romanIndex = 0;
    private int workersNum;
    private float rewardPerChar;
    private float typingCycle;
    private float missTypeProbability;
    private float noMissBonus;

    private void Start()
    {
        time = 0;
        transform.rotation = Camera.main.transform.rotation;
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        coinCanvas = GameObject.Find("Canvas Coin").GetComponent<Canvas>();
        InitializeQuestion();
        StartCoroutine(WaitOneFrame());
    }

    private void Update()
    {
        UpdateStatus();
        audioSource.volume = OptionUI.seLevel;
        if (animator != null)
        {
            animator.speed = Mathf.Min(1000f, 1 / typingCycle);
        }
        time += Time.deltaTime;
        if (time > typingCycle)
        {
            audioSource.PlayOneShot(typeSounds[UnityEngine.Random.Range(0, typeSounds.Length)]);
            time = 0;
            if (missTypeProbability < UnityEngine.Random.Range(0f, 1f))
            {
                romanIndex++;
                if (roman[romanIndex] == '@')
                {
                    int reward = (int)Math.Round(rewardPerChar * question.charCount, MidpointRounding.AwayFromZero);
                    if (!isMissed)
                    {
                        reward = (int)Math.Round(noMissBonus * reward, MidpointRounding.AwayFromZero);
                    }
                    gameManager.money += reward;
                    gameManager.totalMoney += reward;
                    if (coin != null)
                    {
                        var coinObj = Instantiate(coin, transform.position, Quaternion.identity);
                        coinObj.transform.SetParent(coinCanvas.transform);
                        coinObj.transform.localScale = Vector3.one;
                    }
                    isMissed = false;
                    InitializeQuestion();
                }
                else
                {
                    romanText.text = GenerateRomanText();
                }
            }
            else
            {
                MissTypeAnimation();
                isMissed = true;
            }
        }
    }

    IEnumerator WaitOneFrame()
    {
        yield return null;
        defaultPos = transform.localPosition;
    }

    private void UpdateStatus()
    {
        workersNum = gameManager.workersNum.Value;
        rewardPerChar = gameManager.rewardPerChar;
        typingCycle = gameManager.typingCycle;
        missTypeProbability = gameManager.missTypeProbability;
        noMissBonus = gameManager.noMissBonus;
    }
    private void MissTypeAnimation()
    {
        shakeTween.Kill();
        transform.localPosition = defaultPos;
        shakeTween = transform.DOShakePosition(0.1f, 5f, 30, 1, false, true);
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
