using System.Collections.Generic;
using System;

namespace AHG.QuizRedux
{
    [Serializable]
    public class Category
    {
        /// <summary>
        /// Название категории
        /// </summary>
        public string Name;
        /// <summary>
        /// Список вопросов
        /// </summary>
        public List<Question> Questions;
        /// <summary>
        /// Время для ответа на вопрос
        /// </summary>
        public int AnswerTime = 15;

        public Category() {
            Questions = new List<Question>();
        }
    }
}