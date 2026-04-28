using UnityEngine;

public class pasueManager : MonoBehaviour
{
    [Header("Canvas de pausa")]
    [SerializeField] private GameObject pauseCanvas;

    [Header("Canvas que se deben ocultar al pausar")]
    [SerializeField] private GameObject[] canvasToHide;

    private bool isPaused = false;

    private void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        foreach (GameObject canvas in canvasToHide)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        foreach (GameObject canvas in canvasToHide)
        {
            if (canvas != null)
                canvas.SetActive(true);
        }
    }
}
