using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class EnhanceButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioClip purchaseClip;
    [SerializeField] Type type;
    [SerializeField] GameObject explainationPanel;
    [SerializeField] string explaination;
    [SerializeField] Vector3 panelPos;
    [SerializeField] float panelWidth;
    [SerializeField] float panelHeight;


    [SerializeField] TextMeshProUGUI explainationText;
    [SerializeField] TextMeshProUGUI explainationValue;

    private AudioSource audioSource;

    public enum Type
    {
        WorkersNum,
        RewardPerChar,
        TypingCycle,
        MissTypeProbability,
        NoMissBonus,
        Ikasama,
        None
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        text.text = GameManager.GetLevelCost(type, GameManager.levels[type]).ToString();
        if (Input.GetKeyDown(KeyCode.Alpha1) && type == Type.WorkersNum)
        {
            OnClicked();
            OnExit();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && type == Type.RewardPerChar)
        {
            OnClicked();
            OnExit();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && type == Type.TypingCycle)
        {
            OnClicked();
            OnExit();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && type == Type.MissTypeProbability)
        {
            OnClicked();
            OnExit();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && type == Type.NoMissBonus)
        {
            OnClicked();
            OnExit();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && type == Type.Ikasama)
        {
            OnClicked();
            OnExit();
        }
    }

    public void OnEnter()
    {
        explainationPanel.SetActive(true);
        if (explaination.Contains("\\n"))
        {
            explaination = explaination.Replace("\\n", Environment.NewLine);
        }
        explainationText.text = explaination;
        float nowValue = GameManager.GetLevelValue(type, GameManager.levels[type]);
        float nextValue = GameManager.GetLevelValue(type, GameManager.levels[type] + 1);
        explainationPanel.transform.localPosition = panelPos;
        explainationPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth, panelHeight);
        switch (type)
        {
            case Type.WorkersNum:
                explainationValue.text = $"{nowValue} 人 → {nextValue} 人";
                break;
            case Type.RewardPerChar:
                explainationValue.text = $"<sprite=0>{Mathf.Floor(nowValue * 100) / 100} /文字 → <sprite=0>{Mathf.Floor(nextValue * 100) / 100} /文字";
                break;
            case Type.TypingCycle:
                explainationValue.text = $"{Mathf.Floor(10 / nowValue) / 10} 文字/s → {Mathf.Floor(10 / nextValue) / 10} 文字/s";
                break;
            case Type.MissTypeProbability:
                explainationValue.text = $"{Mathf.Floor(nowValue * 1000) / 10} % → {Mathf.Floor(nextValue * 1000) / 10} %";
                break;
            case Type.NoMissBonus:
                explainationValue.text = $"×{nowValue} → ×{nextValue}";
                break;
            case Type.Ikasama:
                explainationValue.text = $"Lv.{nowValue} → Lv.{nextValue}";
                break;
        }

    }

    public void OnExit()
    {
        explainationPanel.SetActive(false);
    }

    public void OnClicked()
    {
        if (GameManager.money >= GameManager.GetLevelCost(type, GameManager.levels[type]))
        {
            gameManager.OnButtonClicked(type, text);
            audioSource.volume = OptionUI.seLevel;
            audioSource.PlayOneShot(purchaseClip);
            OnEnter();
        }
    }
}
