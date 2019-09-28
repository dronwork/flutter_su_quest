using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AHG.QuizRedux
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : Editor
    {
#if UNITY_EDITOR
        private static Config instance;

        private void OnEnable() {
            instance = (Config)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorUtils.DoStyledLabelField("Настройки викторины", GUIStyles.BoldCenterText);
            if (GUILayout.Button("Категории и вопросы...", GUILayout.Height(30))) QuestionsWindow.Open();
            if (GUILayout.Button("Перейти в документацию...", GUILayout.Height(30)))
                Application.OpenURL("https://ah-documentation.readthedocs.io/ru/latest/quizredux/api/");
            EditorGUILayout.EndVertical();
        }

        public class QuestionsWindow : EditorWindow
        {
            private Vector2 scrollPos;
            private static Vector2 windowSize = new Vector2(800, 500);

            private static List<bool> categoriesFoldouts;

            private string newCategoryName;

            public static void Open()
            {
                categoriesFoldouts = new List<bool>(new bool[instance.Categories.Count]);
                QuestionsWindow window = (QuestionsWindow)GetWindow(typeof(QuestionsWindow));
                window.maxSize = windowSize; window.minSize = windowSize;
                window.Show();
                window.titleContent.text = "Настройки";
            }

            private void OnGUI()
            {
                EditorGUILayout.BeginVertical();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(windowSize.x), GUILayout.Height(windowSize.y));
                EditorUtils.DoStyledLabelField("Настройки", GUIStyles.BoldCenterText);
                EditorGUILayout.BeginVertical("box");
                newCategoryName = EditorGUILayout.TextField("Название категории:", newCategoryName);
                if (GUILayout.Button("Добавить категорию", GUILayout.Height(30)))
                {
                    if (!string.IsNullOrEmpty(newCategoryName))
                    {
                        Category category = new Category() { Name = newCategoryName };
                        instance.Categories.Add(category); categoriesFoldouts.Add(new bool());
                        Debug.Log($"Категория \"{category.Name}\" успешно добавлена!");
                    }
                    else Debug.LogError("Не указано название категории!");
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");
                EditorUtils.DoStyledLabelField("Категории", GUIStyles.BoldCenterText);
                if (instance.Categories.Count > 0)
                {
                    for (int c = 0; c < instance.Categories.Count; c++)
                    {
                        Category category = instance.Categories[c];
                        categoriesFoldouts[c] = EditorGUILayout.Foldout(categoriesFoldouts[c], category.Name, true);
                        if (categoriesFoldouts[c])
                        {
                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField($"Категория: {category.Name}");
                            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                bool delete = category.Questions.Count > 0 ? EditorUtility.DisplayDialog("Удаление категории",
                                    "Вы уверены, что хотите удалить категорию, вместе с её вопросами?", "Да", "Отмена") : true;
                                if (delete)
                                {
                                    Debug.Log($"Категория \"{category.Name}\" успешно удалена!");
                                    instance.Categories.RemoveAt(c); categoriesFoldouts.RemoveAt(c);
                                    break;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            category.AnswerTime = EditorGUILayout.DelayedIntField("Время для ответа:", category.AnswerTime);
                            EditorGUILayout.HelpBox("Минимальное количество времени = 5 сек.", MessageType.Warning);
                            if (category.AnswerTime < 1)
                            {
                                category.AnswerTime = 5;
                                Debug.LogWarning("Минимальное количество времени = 5 сек.");
                            }
                            if (category.Questions.Count > 0)
                            {
                                for (int q = 0; q < category.Questions.Count; q++)
                                {
                                    EditorGUILayout.BeginVertical();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField($"Вопрос: {q + 1}");
                                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        category.Questions.RemoveAt(q);
                                        break;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.Space();
                                    Question question = category.Questions[q];
                                    question.Text = EditorGUILayout.TextField("Текст вопроса:", question.Text);
                                    EditorGUILayout.LabelField("Варианты ответов:");
                                    EditorGUILayout.BeginVertical("box");
                                    for (int a = 0; a < question.Answers.Length; a++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        question.Answers[a] = EditorGUILayout.TextField($"Ответ {a + 1}", question.Answers[a]);
                                        if (question.CorrectAnswerID == a) GUI.backgroundColor = Colors.Green;
                                        if (GUILayout.Button("✔", GUILayout.Width(25), GUILayout.Height(20))) question.CorrectAnswerID = a;
                                        GUI.backgroundColor = Color.white;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndVertical();
                                    EditorUtils.MakeObjectField("Изображение", ref question.Image, false);
                                    EditorGUILayout.HelpBox("Если изображения нет, то будет показан только текст вопроса.",
                                        MessageType.Info);
                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.Space();
                                }
                            }
                            else EditorGUILayout.HelpBox("Вы ещё не добавили вопросов.", MessageType.Warning);
                            if (GUILayout.Button("Добавить вопрос", GUILayout.Height(30))) category.Questions.Add(new Question());
                            EditorGUILayout.EndVertical();
                        }
                    }
                }
                else EditorGUILayout.HelpBox("У вас ещё нет категорий.", MessageType.Warning);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                if (GUI.changed) EditorUtils.SetObjectDirty(instance);
            }
        }
#endif
    }
}