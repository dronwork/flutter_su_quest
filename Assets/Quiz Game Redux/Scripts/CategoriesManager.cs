using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AHG.QuizRedux
{
    public class CategoriesManager : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] private GameObject categoriesPanel;
        [SerializeField] private CanvasGroup categoriesCanvasGroup;
        [SerializeField] private GameObject categoryBtnPrefab;
        [SerializeField] private Transform categoryBtnsContent;
        [SerializeField] private Button nextPageBtn;
        [SerializeField] private Button prevPageBtn;
#pragma warning restore CS0649

        private const string ANIM_SHOW_CATEGORIES = "ShowCategories", ANIM_HIDE_CATEGORIES = "HideCategories";
        private const int PAGE_BTTNS_COUNT = 4;

        private int currentPage;

        private List<Button> categoryBtnsPool = new List<Button>();

        #region Singleton
        public static CategoriesManager Instance { get; private set; }
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
        /// Открыть панель выбора категорий.
        /// </summary>
        public void Open()
        {
            categoriesCanvasGroup.interactable = true;
            if (categoriesPanel.activeSelf) return;
            currentPage = 0;
            GameManager.Instance.ToggleHeader(true);
            GameManager.Instance.PlayAnimation(ANIM_SHOW_CATEGORIES);
            LoadPage();
        }
        /// <summary>
        /// Закрыть панель выбора категорий.
        /// </summary>
        public void Close()
        {
            GameManager.Instance.PlayAnimation(ANIM_HIDE_CATEGORIES);
            GameManager.Instance.ToggleHeader(false);
        }
        /// <summary>
        /// Переключить страницу списка категорий, в зависимости от направления.
        /// </summary>
        /// <param name="dir">Направление смены страницы.</param>
        public void ChangePage(int dir)
        {
            currentPage += dir;
            LoadPage();
        }
        /// <summary>
        /// Загрузить страницу категории.
        /// </summary>
        private void LoadPage()
        {
            int startIndex = currentPage * PAGE_BTTNS_COUNT;
            UnityAction<int> onClick = (id) => {
                categoriesCanvasGroup.interactable = false;
                GameManager.Instance.PlayAnimation(ANIM_HIDE_CATEGORIES);
                float length = GameManager.Instance.GetAnimationLength(ANIM_SHOW_CATEGORIES);
                GameManager.Instance.WaitForSeconds(length, () => QuizManager.Instance.Play(GameManager.GetConfig().Categories[id]));
            };
            for (int i = 0; i < PAGE_BTTNS_COUNT; i++)
            {
                Button btn;
                if (categoryBtnsPool.Count < i + 1)
                {
                    btn = Instantiate(categoryBtnPrefab, categoryBtnsContent).GetComponent<Button>();
                    categoryBtnsPool.Add(btn);
                }
                else btn = categoryBtnsPool[i];
                if (startIndex + i >= GameManager.GetConfig().Categories.Count)
                {
                    btn.gameObject.SetActive(false);
                    continue;
                }
                else btn.gameObject.SetActive(true);
                int id = startIndex + i;
                btn.GetComponentInChildren<Text>().text = GameManager.GetConfig().Categories[id].Name;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => onClick(id));
            }
            SetPageBttns();
        }
        /// <summary>
        /// Изменить кликабельность кнопок переключения страниц, в зависимости от текущей страницы.
        /// </summary>
        private void SetPageBttns()
        {
            int totalPages = Mathf.CeilToInt((float)GameManager.GetConfig().Categories.Count / PAGE_BTTNS_COUNT);
            nextPageBtn.interactable = currentPage + 1 <= totalPages - 1;
            prevPageBtn.interactable = currentPage - 1 >= 0;
        }
        /// <summary>
        /// Очистить пул кнопок, и сбросить текущую страницу на 0.
        /// </summary>
        private void ClearList()
        {
            foreach (Button btn in categoryBtnsPool)
                Destroy(btn.gameObject);
            categoryBtnsPool.Clear();
            currentPage = 0;
        }
    }
}
