using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider effectSlider;
    [SerializeField] Canvas optionCanvas;
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

    public void UpdateValues()
    {
        seLevel = seSlider.value;
        bgmLevel = bgmSlider.value;
        effectLevel = effectSlider.value;
        bgmAudioSource.volume = bgmLevel;
        volume.weight = effectLevel;
    }
}
