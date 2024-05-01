using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject worker;
    [SerializeField] GameObject workerPanel;
    [SerializeField] GameObject workersNumButton;
    [SerializeField] GameObject rewardPerCharButton;
    [SerializeField] GameObject typingCycleButton;
    [SerializeField] GameObject missTypeProbabilityButton;
    [SerializeField] GameObject missTypePenaltyButton;
    [SerializeField] GameObject ikasamaButton;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GridItemSizeSetter gridItemSizeSetter;
    [SerializeField] float targetPositionX;

    public int money;
    public int totalMoney;
    public ReactiveProperty<int> workersNum = new ReactiveProperty<int>();
    public int rewardPerChar;
    public float typingCycle;
    public float missTypeProbability;
    public float missTypePenalty;
    public float ikasamaProbability;

    public Dictionary<EnhanceButton.Type, int> levels = new Dictionary<EnhanceButton.Type, int>();
    public Dictionary<EnhanceButton.Type, float> growthRates = new Dictionary<EnhanceButton.Type, float>();
    public Dictionary<EnhanceButton.Type, int> baseCosts = new Dictionary<EnhanceButton.Type, int>();

    void Start()
    {
        foreach (EnhanceButton.Type type in System.Enum.GetValues(typeof(EnhanceButton.Type)))
        {
            levels[type] = 1;
            growthRates[type] = 1.1f;
            baseCosts[type] = 10;
        }
        workersNum.Subscribe(num =>
        {
            int xSqrt = Mathf.RoundToInt(Mathf.Sqrt(num));
            if (num == xSqrt * xSqrt)
            {
                gridItemSizeSetter.rowCount = xSqrt + 1;
                gridItemSizeSetter.columnCount = xSqrt + 1;
            }
        });
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
                missTypePenaltyButton.SetActive(true);
                missTypePenaltyButton.transform.DOLocalMoveX(targetPositionX, 1f);
            }
            if (num >= 8)
            {
                ikasamaButton.SetActive(true);
                ikasamaButton.transform.DOLocalMoveX(targetPositionX, 1f);
            }
        });
    }

    private void Update()
    {
        moneyText.text = money.ToString();
    }

    public void OnButtonClicked(EnhanceButton.Type type, TextMeshProUGUI text)
    {
        int currentLevel = levels[type];
        int currentCost = GetLevelCost(type, currentLevel);
        if (money >= currentCost)
        {
            money -= currentCost;
            levels[type]++;
            UpdateStats(type);
        }
    }

    public string GetTypeName(EnhanceButton.Type type)
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
            case EnhanceButton.Type.MissTypePenalty:
                return "ペナルティ軽減";
            case EnhanceButton.Type.Ikasama:
                return "イカサマ";
            default:
                return "";
        }
    }


    public int GetLevelCost(EnhanceButton.Type type, int level)
    {
        int baseCost = baseCosts[type];
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.5f, level - 1));
    }


    public float GetLevelValue(EnhanceButton.Type type, int level)
    {
        switch (type)
        {
            case EnhanceButton.Type.WorkersNum:
                return level - 1;
            case EnhanceButton.Type.RewardPerChar:
                return 1 + (level * 0.1f);
            case EnhanceButton.Type.TypingCycle:
                return Mathf.Max(0.1f, 1 - (level * 0.1f)); // Decreases typing cycle time
            case EnhanceButton.Type.MissTypeProbability:
                return Mathf.Max(0f, 0.5f - (level * 0.05f)); // Decreases miss type probability
            case EnhanceButton.Type.MissTypePenalty:
                return 1 - (level * 0.1f);
            case EnhanceButton.Type.Ikasama:
                return 0;
            default:
                return 0;
        }
    }

    private void UpdateStats(EnhanceButton.Type type)
    {
        int currentLevel = levels[type];
        switch (type)
        {
            case EnhanceButton.Type.WorkersNum:
                workersNum.Value = (int)GetLevelValue(type, currentLevel);
                var obj = Instantiate(worker);
                obj.transform.parent = workerPanel.transform;
                obj.transform.localScale = Vector3.one;
                break;
            case EnhanceButton.Type.RewardPerChar:
                rewardPerChar = (int)GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.TypingCycle:
                typingCycle = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.MissTypeProbability:
                missTypeProbability = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.MissTypePenalty:
                missTypePenalty = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.Ikasama:
                ikasamaProbability = GetLevelValue(type, currentLevel);
                break;
        }
    }
}