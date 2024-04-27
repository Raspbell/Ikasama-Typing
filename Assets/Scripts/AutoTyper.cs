using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoTyper : MonoBehaviour
{

    [SerializeField] QuestionPropety questionPropety;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI romanText;
    [SerializeField] float cycle;

    private float time;
    private List<char> _roman = new List<char>();
    private int _romanIndex = 0;

    private void Start()
    {
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > cycle)
        {
            time = 0;
            _romanIndex++;
        }
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
            if (i == _romanIndex)
            {
                text += "</style><style=untyped>";
            }

            text += _roman[i];
        }
        text += "</style>";
        return text;
    }

    private void InitializeQuestion()
    {
        QuestionPropety.Question question = questionPropety.questions[UnityEngine.Random.Range(0, questionPropety.questions.Length)];

        _romanIndex = 0;
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

}
