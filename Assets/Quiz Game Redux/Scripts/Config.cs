using System.Collections.Generic;
using UnityEngine;

namespace AHG.QuizRedux
{
    [CreateAssetMenu(fileName = "New config", menuName = "Quiz/New Config...")]
    public class Config : ScriptableObject
    {
        /// <summary>
        /// Список категорий
        /// </summary>
        public List<Category> Categories;

        public Config() {
            Categories = new List<Category>();
        }
    }
}