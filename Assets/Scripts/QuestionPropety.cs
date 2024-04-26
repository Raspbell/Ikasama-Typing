using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionPropety", menuName = "ScriptableObjects/QuestionPropety")]
public class QuestionPropety : ScriptableObject
{

    [Serializable]
    public class Question
    {
        public string title;
        public string roman;
    }

    public Question[] questions;
}

