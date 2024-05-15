using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using unityroom.Api;
using System;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject worker;
    [SerializeField] GameObject additionalWorker;
    [SerializeField] GameObject workerPanel;
    [SerializeField] GameObject workersNumButton;
    [SerializeField] GameObject rewardPerCharButton;
    [SerializeField] GameObject typingCycleButton;
    [SerializeField] GameObject missTypeProbabilityButton;
    [SerializeField] GameObject noMissBonusButton;
    [SerializeField] GameObject ikasamaButton;
    [SerializeField] GameObject additionalWorkers;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI additionalWorkersText;
    [SerializeField] float targetPositionX;
    [SerializeField] Vector3 cameraPosition;
    [SerializeField] Vector3 cameraRotation;
    [SerializeField] float cameraMoveDuration;
    [SerializeField] GameObject allUI;

    private float time = 0;

    public static long money;
    public static long totalMoney;
    public static ReactiveProperty<int> workersNum = new ReactiveProperty<int>();
    public static float rewardPerChar;
    public static float typingCycle;
    public static float missTypeProbability;
    public static float noMissBonus;
    public static float ikasamaProbability;

    public static Dictionary<EnhanceButton.Type, int> levels = new Dictionary<EnhanceButton.Type, int>();
    public static Dictionary<EnhanceButton.Type, float> maxLevels = new Dictionary<EnhanceButton.Type, float>();
    public static Dictionary<EnhanceButton.Type, float> growthRates = new Dictionary<EnhanceButton.Type, float>();
    public static Dictionary<EnhanceButton.Type, long> baseCosts = new Dictionary<EnhanceButton.Type, long>();

    private Vector3[] workerPositions = new Vector3[]
    {
        new Vector3(0, 0, 3),
        new Vector3(0, 0, 6),
        new Vector3(3, 0, 0),
        new Vector3(3, 0, 3),
        new Vector3(3, 0, 6),
        new Vector3(3, 0, 9),
        new Vector3(6, 0, 0),
        new Vector3(6, 0, 3),
        new Vector3(6, 0, 6),
        new Vector3(6, 0, 9),
        new Vector3(6, 0, 12),
        new Vector3(9, 0, 3),
        new Vector3(9, 0, 6),
        new Vector3(9, 0, 9)
    };

    void Start()
    {
        foreach (EnhanceButton.Type type in Enum.GetValues(typeof(EnhanceButton.Type)))
        {
            growthRates[type] = 1.1f;
            baseCosts[type] = 10;
            maxLevels[type] = 1000;
        }
        workersNum.Value = (int)GetLevelValue(EnhanceButton.Type.WorkersNum, levels[EnhanceButton.Type.WorkersNum]);
        rewardPerChar = GetLevelValue(EnhanceButton.Type.RewardPerChar, levels[EnhanceButton.Type.RewardPerChar]);
        typingCycle = GetLevelValue(EnhanceButton.Type.TypingCycle, levels[EnhanceButton.Type.TypingCycle]);
        missTypeProbability = GetLevelValue(EnhanceButton.Type.MissTypeProbability, levels[EnhanceButton.Type.MissTypeProbability]);
        noMissBonus = GetLevelValue(EnhanceButton.Type.NoMissBonus, levels[EnhanceButton.Type.NoMissBonus]);
        ikasamaProbability = GetLevelValue(EnhanceButton.Type.Ikasama, levels[EnhanceButton.Type.Ikasama]);
        workersNum.Subscribe(num =>
        {
            if (num >= 2)
            {
                rewardPerCharButton.SetActive(true);
                typingCycleButton.SetActive(true);
                rewardPerCharButton.transform.DOLocalMoveX(targetPositionX, 1f);
                typingCycleButton.transform.DOLocalMoveX(targetPositionX, 1f);
            }
            if (num >= 4)
            {
                missTypeProbabilityButton.SetActive(true);
                missTypeProbabilityButton.transform.DOLocalMoveX(targetPositionX, 1f);
            }
            if (num >= 6)
            {
                noMissBonusButton.SetActive(true);
                noMissBonusButton.transform.DOLocalMoveX(targetPositionX, 1f);
            }
            if (num >= 8)
            {
                ikasamaButton.SetActive(true);
                ikasamaButton.transform.DOLocalMoveX(targetPositionX, 1f);
            }
        });
        Sequence mySequence = DOTween.Sequence();
        mySequence
            .Join(Camera.main.DOOrthoSize(5, cameraMoveDuration))
            .Join(Camera.main.transform.DOMove(cameraPosition, cameraMoveDuration))
            .Join(Camera.main.transform.DORotate(cameraRotation, cameraMoveDuration))
            .SetEase(Ease.InOutSine)
            .OnComplete(() => Initialize());
    }

    private void Update()
    {
        moneyText.text = money.ToString();
        time += Time.deltaTime;
        if (time >= 10)
        {
            if (totalMoney > 0)
            {
                UnityroomApiClient.Instance.SendScore(1, totalMoney, ScoreboardWriteMode.Always);
            }
            time = 0;
            StartTyping.SavePrefs();
        }
    }

    private void Initialize()
    {
        allUI.transform.DOScale(Vector3.one, 1f)
            .OnComplete(() =>
            {
                StartCoroutine(InstantiateWorkersCoroutine());
            });
    }

    private IEnumerator InstantiateWorkersCoroutine()
    {
        for (int i = 1; i <= workersNum.Value; i++)
        {
            InstantiateWorker(i);
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void OnButtonClicked(EnhanceButton.Type type, TextMeshProUGUI text)
    {
        int currentLevel = levels[type];
        long currentCost = GetLevelCost(type, currentLevel);
        money -= currentCost;
        levels[type]++;
        UpdateStats(type);
    }

    public static string GetTypeName(EnhanceButton.Type type)
    {
        switch (type)
        {
            case EnhanceButton.Type.WorkersNum:
                return "人員追加";
            case EnhanceButton.Type.RewardPerChar:
                return "報酬増加";
            case EnhanceButton.Type.TypingCycle:
                return "スピードアップ";
            case EnhanceButton.Type.MissTypeProbability:
                return "精度上昇";
            case EnhanceButton.Type.NoMissBonus:
                return "ボーナス増加";
            case EnhanceButton.Type.Ikasama:
                return "イカサマ";
            default:
                return "";
        }
    }

    public static long GetLevelCost(EnhanceButton.Type type, int level)
    {
        long baseCost = baseCosts[type];
        // Mathf.Powを使用してレベルコストの増加を計算
        double costMultiplier = Mathf.Pow(1.5f, level);
        // 結果をdoubleからlongにキャスト
        return (long)(baseCost * costMultiplier);
    }


    public static float GetLevelValue(EnhanceButton.Type type, int level)
    {
        switch (type)
        {
            case EnhanceButton.Type.WorkersNum:
                return level;
            case EnhanceButton.Type.RewardPerChar:
                return Mathf.Pow(1.15f, level);
            case EnhanceButton.Type.TypingCycle:
                return Mathf.Max(0.001f, Mathf.Pow(0.85f, level));
            case EnhanceButton.Type.MissTypeProbability:
                return Mathf.Max(0f, 0.5f * Mathf.Pow(0.9f, level));
            case EnhanceButton.Type.NoMissBonus:
                return 1 + (level * 0.2f);
            case EnhanceButton.Type.Ikasama:
                return level;
            default:
                return level;
        }
    }

    private void UpdateStats(EnhanceButton.Type type)
    {
        int currentLevel = levels[type];
        switch (type)
        {
            case EnhanceButton.Type.WorkersNum:
                InstantiateWorker(currentLevel);
                workersNum.Value = (int)GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.RewardPerChar:
                rewardPerChar = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.TypingCycle:
                typingCycle = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.MissTypeProbability:
                missTypeProbability = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.NoMissBonus:
                noMissBonus = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.Ikasama:
                ikasamaProbability = GetLevelValue(type, currentLevel);
                break;
        }
    }

    public void InstantiateWorker(int level)
    {
        if (level < workerPositions.Length)
        {
            GameObject obj = Instantiate(worker);
            obj.transform.position = workerPositions[level - 1] + new Vector3(0, 10, 0);
            obj.transform.DOLocalMove(workerPositions[level - 1], 1f).SetEase(Ease.OutBounce);
            obj.transform.localScale = Vector3.one;
        }
        else
        {
            GameObject obj = Instantiate(additionalWorker);
            obj.transform.position = new Vector3(-7.739688f, 8.918709f, -11.76969f);
            obj.transform.rotation = Camera.main.transform.rotation;
            additionalWorkers.transform.DOLocalMoveY(190f, 1f);
            additionalWorkersText.text = $"+ {level - workerPositions.Length + 1}";
        }
    }
}