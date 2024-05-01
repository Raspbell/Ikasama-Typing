using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnhanceButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameManager gameManager;
    [SerializeField] Type type;

    public enum Type
    {
        WorkersNum,
        RewardPerChar,
        TypingCycle,
        MissTypeProbability,
        MissTypePenalty,
        Ikasama,
        None
    }

    private void Update()
    {
        text.text = gameManager.GetLevelCost(type, gameManager.levels[type]).ToString();
    }

    public void OnClicked()
    {
        gameManager.OnButtonClicked(type, text);
    }
}
