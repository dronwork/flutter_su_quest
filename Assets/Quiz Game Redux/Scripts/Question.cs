using UnityEngine;
using System;

namespace AHG.QuizRedux
{
    [Serializable]
    public class Question
    {
        /// <summary>
        /// Текст вопроса
        /// </summary>
        public string Text;
        /// <summary>
        /// Картинка вопроса (не обязательна)
        /// </summary>
        public Sprite Image;
        /// <summary>
        /// Список вариантов ответов
        /// </summary>
        public string[] Answers;
        /// <summary>
        /// Идентификатор правильного ответа
        /// </summary>
        public int CorrectAnswerID;

        public Question(string text, Sprite image, string[] answers, int correctAnswerID)
        {
            Text = text;
            Image = image;
            Answers = answers;
            CorrectAnswerID = correctAnswerID;
        }

        public Question() : this(null, null, new string[3], 0) { }
    }
}