using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialPanelController : MonoBehaviour
{
    [System.Serializable]
    public class TutorialPage
    {
        public Sprite image;
        public string title;
        [TextArea(3, 6)]
        public string description;
    }

    [Header("Referencias UI")]
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text nextButtonText;

    [Header("Páginas del tutorial")]
    [SerializeField] private TutorialPage[] pages;

    [SerializeField] private string gameSceneName;


    private int currentPage = 0;

    private void Start()
    {
        nextButton.onClick.AddListener(NextPage);

        if (pages.Length > 0)
        {
            ShowPage(0);
        }
        else
        {
            Debug.LogWarning("No hay páginas configuradas en el tutorial.");
        }
    }

    private void ShowPage(int index)
    {
        tutorialImage.sprite = pages[index].image;
        titleText.text = pages[index].title;
        descriptionText.text = pages[index].description;

        if (index == pages.Length - 1)
        {
            nextButtonText.text = "Comenzar";
        }
        else
        {
            nextButtonText.text = "Siguiente";
        }
    }

    public void NextPage()
    {
        if (currentPage == pages.Length - 1)
        {
            StartGame();
            return;
        }

        currentPage++;
        ShowPage(currentPage);
    }

    private void StartGame()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("No se asignó el nombre de la escena del juego.");
        }
    }
}
