using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AHG.QuizRedux
{
    public class QuizManager : MonoBehaviour
    {
#pragma warning disable CS0649
        [Header("Панель проведения викторины")]
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private GameObject questionImgObj;
        [SerializeField] private Text questionText;
        [SerializeField] private Text timerText;
        [SerializeField] private CanvasGroup answerBtnsGroup;
        [SerializeField] private Button[] answerBtns;
        [SerializeField] private Image questionImage;
        [SerializeField] private GameObject questionImageFullPanel;
        [SerializeField] private Image questionImageFull;
        [SerializeField] private Text questionImgText;
        [Header("Панель правильности ответа")]
        [SerializeField] private Image answerPanelImg;
        [SerializeField] private Text answerStateText;
        [SerializeField] private Image answerStateImage;
        [SerializeField] private Text questionCounterText;
        [SerializeField] private Sprite[] answerStateIcons;
#pragma warning restore CS0649

        private const string ANIM_SHOW_QUESTION = "ShowQuestion", ANIM_HIDE_QUESTION = "HideQuestion", ANIM_SHOW_ANSWER = "ShowAnswer",
            ANIM_HIDE_ANSWER = "HideAnswer", ANIM_SHOW_FIMAGE = "ShowFullImage", ANIM_HIDE_FIMAGE = "HideFullImage";
        private readonly string[] ANSWER_TYPES_TITLES = { "Правильный ответ", "Неправильный ответ", "Время вышло" };

        private Category currentCategory;
        private Question currentQuestion;

        private Coroutine timerCoroutine;

        private Queue<Question> questionsQueue = new Queue<Question>();

        #region Singleton
        public static QuizManager Instance { get; private set; }
        private void InitSingleton()
        {
            if (Instance == null)
                Instance = this;
        }
        #endregion

        private void Awake() {
            InitSingleton();
        }
        /// <summary>
        ///Старт игры с указанной категорией.
        /// </summary>
        /// <param name="category">Категория для старта игры.</param>
        public void Play(Category category)
        {
            if (currentCategory != null) return;
            currentCategory = category;
            GenerateQuestions();
        }
        /// <summary>
        /// Переключить показ изображения в большем размере.
        /// </summary>
        /// <param name="full">Показать изображение в большем размере?</param>
        public void ToggleImageView(bool full)
        {
            if (full) questionImageFull.sprite = currentQuestion.Image;
            GameManager.Instance.PlayAnimation(full ? ANIM_SHOW_FIMAGE : ANIM_HIDE_FIMAGE, 3);
        }
        /// <summary>
        /// Сгенерировать новый список вопросов.
        /// </summary>
        private void GenerateQuestions()
        {
            List<Question> questions = new List<Question>(currentCategory.Questions);
            for (int i = 0; i < currentCategory.Questions.Count; i++)
            {
                int id = Random.Range(0, questions.Count);
                Question q = new Question(questions[id].Text, questions[id].Image, new string[3], -1);
                int correctAnswerID = questions[id].CorrectAnswerID;
                List<string> answers = new List<string>(questions[id].Answers);
                for (int a = 0; a < q.Answers.Length; a++)
                {
                    int answerID = Random.Range(0, answers.Count);
                    q.Answers[a] = answers[answerID];
                    if (q.CorrectAnswerID == -1)
                    {
                        if (answerID == correctAnswerID) q.CorrectAnswerID = a;
                        else if (answerID < correctAnswerID) correctAnswerID--;
                    }
                    answers.RemoveAt(answerID);
                }
                questionsQueue.Enqueue(q);
                questions.RemoveAt(id);
            }
            ShowQuestion();
        }
        /// <summary>
        /// Показать случайный вопрос из списка.
        /// </summary>
        private void ShowQuestion()
        {
            Time = currentCategory.AnswerTime;
            answerBtnsGroup.interactable = false;
            if (questionsQueue.Count > 0)
            {
                currentQuestion = questionsQueue.Dequeue();
                if (currentQuestion.Image != null) questionImage.sprite = currentQuestion.Image;
                questionImgObj.SetActive(currentQuestion.Image != null);
                questionText.gameObject.SetActive(currentQuestion.Image == null);
                questionText.text = questionImgText.text = currentQuestion.Text;
                for (int i = 0; i < currentQuestion.Answers.Length; i++)
                    answerBtns[i].GetComponentInChildren<Text>().text = currentQuestion.Answers[i];
                GameManager.Instance.PlayAnimation(ANIM_SHOW_QUESTION);
                GameManager.Instance.WaitForSeconds(GameManager.Instance.GetAnimationLength(ANIM_SHOW_QUESTION), () => {
                    answerBtnsGroup.interactable = true;
                    ToggleTimer(true);
                });
            }
            else
            {
                // Тут можно добавить действия при окончании категории.
                ResetQuiz();
            }
        }
        /// <summary>
        /// Выбрать вариант ответа.
        /// </summary>
        /// <param name="id">Идентификатор ответа.</param>
        public void SelectAnswer(int id) => StartCoroutine(
            IESelectAnswer(Time > 0 && id == currentQuestion.CorrectAnswerID ? AnswerType.Correct : AnswerType.Wrong));

        private IEnumerator IESelectAnswer(AnswerType type)
        {
            ToggleTimer(false);
            answerBtnsGroup.interactable = false;
            if (type != AnswerType.TimeOver) yield return new WaitForSeconds(1);
            answerStateImage.sprite = answerStateIcons[type == AnswerType.Correct ? 0 : 1];
            GameManager.Instance.PlayAnimation(ANIM_HIDE_QUESTION);
            if (questionImageFullPanel.activeSelf) ToggleImageView(false);
            answerStateText.text = ANSWER_TYPES_TITLES[(int)type];
            int crntQuestionIndex = currentCategory.Questions.Count - questionsQueue.Count;
            questionCounterText.text = $"{crntQuestionIndex}/{currentCategory.Questions.Count}";
            answerPanelImg.color = type == AnswerType.Correct ? Colors.Green : Colors.LightRed;
            GameManager.Instance.PlayAnimation(ANIM_SHOW_ANSWER, 2);
            yield return new WaitForSeconds(GameManager.Instance.GetAnimationLength(ANIM_SHOW_ANSWER));
            yield return new WaitForSeconds(2);
            // Это сделано лишь для удобного разделения по методам.
            switch (type)
            {
                case AnswerType.Wrong: WrongAnswer(); break;
                case AnswerType.TimeOver: TimeOver(); break;
                case AnswerType.Correct: CorrectAnswer(); break;
            }
        }
        /// <summary>
        /// Метод, который вызывается при окончании времени.
        /// </summary>
        private void TimeOver()
        {
            // Тут можно добавить действия при окончании времени.
            ResetQuiz();
        }
        /// <summary>
        /// Метод, который вызывается при неправильном ответе.
        /// </summary>
        private void WrongAnswer()
        {
            // Тут можно добавить действия при неправильном ответе.
            ResetQuiz();
        }
        /// <summary>
        /// Метод, который вызывается при правильном ответе.
        /// </summary>
        private void CorrectAnswer()
        {
            // Тут можно добавить действия при правильном ответе.
            GameManager.Instance.PlayAnimation(ANIM_HIDE_ANSWER, 2);
            ShowQuestion();
        }
        /// <summary>
        /// Восстановить викторину к начальному состоянию.
        /// </summary>
        private void ResetQuiz()
        {
            GameManager.Instance.PlayAnimation(ANIM_HIDE_ANSWER, 2);
            GameManager.Instance.WaitForSeconds(GameManager.Instance.GetAnimationLength(ANIM_SHOW_ANSWER), () => {
                GameManager.Instance.ToggleHeader(false);
                questionsQueue.Clear();
                currentQuestion = null; currentCategory = null;
            });
        }
        /// <summary>
        /// Переключить таймер.
        /// </summary>
        /// <param name="start">Запустить таймер?</param>
        private void ToggleTimer(bool start)
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
            if (start) timerCoroutine = StartCoroutine(IETimer());
        }

        private IEnumerator IETimer()
        {
            while (Time > 0)
            {
                yield return new WaitForSeconds(1);
                Time--;
            }
            StartCoroutine(IESelectAnswer(AnswerType.TimeOver));
        }

        private int _time;
        private int Time
        {
            get { return _time; }
            set
            {
                _time = value;
                timerText.text = _time.ToString();
            }
        }
    }
}