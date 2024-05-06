using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using unityroom.Api;

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

    public long money;
    public long totalMoney;
    public ReactiveProperty<int> workersNum = new ReactiveProperty<int>();
    public float rewardPerChar;
    public float typingCycle;
    public float missTypeProbability;
    public float noMissBonus;
    public float ikasamaProbability;

    public Dictionary<EnhanceButton.Type, int> levels = new Dictionary<EnhanceButton.Type, int>();
    public Dictionary<EnhanceButton.Type, float> maxLevels = new Dictionary<EnhanceButton.Type, float>();
    public Dictionary<EnhanceButton.Type, float> growthRates = new Dictionary<EnhanceButton.Type, float>();
    public Dictionary<EnhanceButton.Type, long> baseCosts = new Dictionary<EnhanceButton.Type, long>();

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
        foreach (EnhanceButton.Type type in System.Enum.GetValues(typeof(EnhanceButton.Type)))
        {
            levels[type] = 0;
            growthRates[type] = 1.1f;
            baseCosts[type] = 10;
        }
        maxLevels[EnhanceButton.Type.WorkersNum] = 1000;
        maxLevels[EnhanceButton.Type.RewardPerChar] = 1000;
        maxLevels[EnhanceButton.Type.TypingCycle] = 1000;
        maxLevels[EnhanceButton.Type.MissTypeProbability] = 1000;
        maxLevels[EnhanceButton.Type.NoMissBonus] = 1000;
        maxLevels[EnhanceButton.Type.Ikasama] = 1000;
        workersNum.Value = 0;
        rewardPerChar = 1;
        typingCycle = 1;
        missTypeProbability = 0.5f;
        noMissBonus = 1;
        ikasamaProbability = 0;
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

        // Orthographic size、位置、回転のアニメーションを Sequence に追加
        mySequence
            .Join(Camera.main.DOOrthoSize(5, cameraMoveDuration)) // カメラのサイズを変更
            .Join(Camera.main.transform.DOMove(cameraPosition, cameraMoveDuration)) // カメラの位置を移動
            .Join(Camera.main.transform.DORotate(cameraRotation, cameraMoveDuration))
            .SetEase(Ease.InOutSine)
            .OnComplete(() => allUI.transform.DOScale(Vector3.one, 1f)); // カメラの回転を変更
    }

    private void Update()
    {
        moneyText.text = money.ToString();
        time += Time.deltaTime;
        if (time >= 20)
        {
            if (totalMoney > 0)
            {
                UnityroomApiClient.Instance.SendScore(1, totalMoney, ScoreboardWriteMode.Always);
            }
        }
    }

    public void OnButtonClicked(EnhanceButton.Type type, TextMeshProUGUI text)
    {
        int currentLevel = levels[type];
        long currentCost = GetLevelCost(type, currentLevel);
        Debug.Log(currentCost);
        money -= currentCost;
        levels[type]++;
        UpdateStats(type);
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
            case EnhanceButton.Type.NoMissBonus:
                return "ボーナス増加";
            case EnhanceButton.Type.Ikasama:
                return "イカサマ";
            default:
                return "";
        }
    }

    public long GetLevelCost(EnhanceButton.Type type, int level)
    {
        long baseCost = baseCosts[type];
        // Mathf.Powを使用してレベルコストの増加を計算
        double costMultiplier = Mathf.Pow(1.5f, level);
        // 結果をdoubleからlongにキャスト
        return (long)(baseCost * costMultiplier);
    }


    public float GetLevelValue(EnhanceButton.Type type, int level)
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
                workersNum.Value = (int)GetLevelValue(type, currentLevel);
                if (workersNum.Value < workerPositions.Length)
                {
                    GameObject obj = Instantiate(worker);
                    obj.transform.position = workerPositions[workersNum.Value - 1] + new Vector3(0, 10, 0);
                    obj.transform.DOLocalMove(workerPositions[workersNum.Value - 1], 1f).SetEase(Ease.OutBounce);
                    obj.transform.localScale = Vector3.one;
                }
                else
                {
                    GameObject obj = Instantiate(additionalWorker);
                    obj.transform.position = new Vector3(-7.739688f, 8.918709f, -11.76969f);
                    obj.transform.rotation = Camera.main.transform.rotation;
                    additionalWorkers.transform.DOLocalMoveY(190f, 1f);
                    additionalWorkersText.text = $"+ {workersNum.Value - workerPositions.Length + 1}";
                }
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
            case EnhanceButton.Type.NoMissBonus:
                noMissBonus = GetLevelValue(type, currentLevel);
                break;
            case EnhanceButton.Type.Ikasama:
                ikasamaProbability = GetLevelValue(type, currentLevel);
                break;
        }
    }
}