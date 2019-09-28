using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AHG.QuizRedux
{
    public enum AnswerType { Correct, Wrong, TimeOver }

    public class GameManager : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private Config config;
        [SerializeField] private Animator headerAnimator;
#pragma warning restore CS0649

        public Animator HeaderAnimator => headerAnimator;

        public static Config GetConfig() => Instance.config;

        private const string ANIM_DROP_HEADER = "DropHeader", ANIM_RAISE_HEADER = "RaiseHeader";

        #region Singleton
        public static GameManager Instance { get; private set; }
        private void InitSingleton()
        {
            if (Instance == null)
                Instance = this;
        }
        #endregion

        #region Fps Settings Init
        private void InitFPSSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 120;
        }
        #endregion

        private void Awake()
        {
            InitSingleton();
            InitFPSSettings();
        }
        /// <summary>
        /// Метод при нажатии на кнопку Play.
        /// </summary>
        public void Play()
        {
            if (config != null && config.Categories.Count > 0)
            {
                if (config.Categories.Count > 1) CategoriesManager.Instance.Open();
                else
                {
                    ToggleHeader(true);
                    QuizManager.Instance.Play(config.Categories[0]);
                }
            }
            else Debug.LogError(config == null ? "Не указана ссылка на конфигурационный файл!" : "Список категорий равен нулю!");
        }
        /// <summary>
        /// Метод для запуска анимации из главного аниматора.
        /// </summary>
        /// <param name="name">Название анимации.</param>
        /// <param name="layer">Идентификатор слоя в аниматоре.</param>
        public void PlayAnimation(string name, int layer = 0) => headerAnimator.Play(name, layer);

        /// <summary>
        /// Опустить либо поднять шапку игры.
        /// </summary>
        /// <param name="drop">Опустить шапку игры?</param>
        public void ToggleHeader(bool drop) => headerAnimator.Play(drop ? ANIM_DROP_HEADER : ANIM_RAISE_HEADER, 1);
        /// <summary>
        /// Получить длительность анимации в главном аниматоре.
        /// </summary>
        /// <param name="name">Название анимации.</param>
        /// <returns>Длительность анимации в главном аниматоре.</returns>
        public float GetAnimationLength(string name)
        {
            foreach (AnimationClip anim in headerAnimator.runtimeAnimatorController.animationClips)
                if (anim.name == name) return anim.length;
            return 0;
        }
        /// <summary>
        /// Выполнить действие через указанное количество секунд.
        /// </summary>
        /// <param name="seconds">Длительность ожидания в секундах.</param>
        /// <param name="onEnd">Действие при окончании ожидания.</param>
        public void WaitForSeconds(float seconds, UnityAction onEnd) => StartCoroutine(IEWaitForSeconds(seconds, onEnd));
        private IEnumerator IEWaitForSeconds(float seconds, UnityAction onEnd)
        {
            yield return new WaitForSeconds(seconds);
            onEnd();
        }
    }
}