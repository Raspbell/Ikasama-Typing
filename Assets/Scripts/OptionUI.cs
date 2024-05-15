using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider effectSlider;
    [SerializeField] Canvas optionCanvas;
    [SerializeField] Canvas resetCanvas;
    [SerializeField] Image resetImage;
    [SerializeField] Button closeButton;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] Volume volume;

    private float bgmLevel = 0.2f;
    private float effectLevel = 1f;

    public static float seLevel = 0.2f;

    public void OnOptionButtonClicked()
    {
        optionCanvas.gameObject.SetActive(true);
        seSlider.value = seLevel;
        bgmSlider.value = bgmLevel;
        effectSlider.value = effectLevel;
    }

    public void OnCloseButtonClicked()
    {
        optionCanvas.gameObject.SetActive(false);
    }

    public void OnResetButtonClicked()
    {
        TypingManager typingManager = FindObjectOfType<TypingManager>();
        typingManager.InitializeQuestion();
    }

    public void OnResetAllButonClicked(){
        resetCanvas.gameObject.SetActive(true);
    }

    public void OnResetAllAcceptButtonClicked(){
        resetCanvas.gameObject.SetActive(false);
        resetImage.gameObject.SetActive(true);
        optionCanvas.gameObject.SetActive(false);
        foreach (EnhanceButton.Type type in System.Enum.GetValues(typeof(EnhanceButton.Type)))
        {
            GameManager.levels[type] = 0;
        }
        GameManager.money = 0;
        GameManager.totalMoney = 0;
        GameManager.workersNum.Value = (int)GameManager.GetLevelValue(EnhanceButton.Type.WorkersNum, GameManager.levels[EnhanceButton.Type.WorkersNum]);
        GameManager.rewardPerChar = GameManager.GetLevelValue(EnhanceButton.Type.RewardPerChar, GameManager.levels[EnhanceButton.Type.RewardPerChar]);
        GameManager.typingCycle = GameManager.GetLevelValue(EnhanceButton.Type.TypingCycle, GameManager.levels[EnhanceButton.Type.TypingCycle]);
        GameManager.missTypeProbability = GameManager.GetLevelValue(EnhanceButton.Type.MissTypeProbability, GameManager.levels[EnhanceButton.Type.MissTypeProbability]);
        GameManager.noMissBonus = GameManager.GetLevelValue(EnhanceButton.Type.NoMissBonus, GameManager.levels[EnhanceButton.Type.NoMissBonus]);
        GameManager.ikasamaProbability = GameManager.GetLevelValue(EnhanceButton.Type.Ikasama, GameManager.levels[EnhanceButton.Type.Ikasama]);
        resetImage.DOFade(1, 1f)
        .OnComplete(() =>
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Title");
        });
    }

    public void OnResetAllCancelButtonClicked(){
        resetCanvas.gameObject.SetActive(false);
    }

    public void UpdateValues()
    {
        seLevel = seSlider.value;
        bgmLevel = bgmSlider.value;
        effectLevel = effectSlider.value;
        bgmAudioSource.volume = bgmLevel;
        volume.weight = effectLevel;
    }
}
