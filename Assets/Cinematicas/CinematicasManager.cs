using UnityEngine;
using UnityEngine.Playables;

public class CinematicasManager : MonoBehaviour
{
    public DayNiightManager dayNightManager;

    [Tooltip("Objetos que se desactivan durante las cinemáticas, no todos deben ser desactivados, solo los que afectan a la visualización de las cinemáticas")]
    public GameObject[] noView;

    [Header("Cinemática Game Over (primera)")]
    public PlayableDirector sceneGameOver;
    public GameObject sceneObjectGameOver;
    public float durationSceneGameOver;
    private float timerSceneGameOver;

    [Header("Cinemática Win (segunda)")]
    public PlayableDirector sceneWin;
    public GameObject sceneObjectWin;
    public float durationSceneWin;
    private float timerSceneWin;

    [Header("Cinemática Lose (segunda)")]
    public PlayableDirector sceneLose;
    public GameObject sceneObjectLose;
    public float durationSceneLose;
    private float timerSceneLose;

    [Tooltip("Obligatorio true, se activa la cinemática de Game Over o Win dependiendo del otro booleano (GameWin)")]
    public bool GameOver = false;
    [Tooltip("Si es true, se activa la cinemática de Win, si es false, se activa la cinemática de Lose")]
    public bool GameWin = false;

    private bool isPlaying = false;
    private int step = 0;

    void Start()
    {
        sceneObjectGameOver.SetActive(true);
        durationSceneGameOver = (float)sceneGameOver.duration;
        sceneObjectGameOver.SetActive(false);

        sceneObjectWin.SetActive(true);
        durationSceneWin = (float)sceneWin.duration;
        sceneObjectWin.SetActive(false);

        sceneObjectLose.SetActive(true);
        durationSceneLose = (float)sceneLose.duration;
        sceneObjectLose.SetActive(false);
    }

    void Update()
    {
        if ((GameOver || GameWin) && !isPlaying && step == 0)
        {
            isPlaying = true;
            step = 1;
            
            dayNightManager.enabled = false;
            // Desactivar objetos del juego
            foreach (GameObject obj in noView)
            {
                if (obj != null) obj.SetActive(false);
            }
            
            sceneObjectGameOver.SetActive(true);
            sceneGameOver.Play();
        }

        if (step == 1)
        {
            timerSceneGameOver += Time.deltaTime;
            if (timerSceneGameOver >= durationSceneGameOver)
            {
                sceneObjectGameOver.SetActive(false);
                timerSceneGameOver = 0;
                step = 2;

                if (GameWin)
                {
                    sceneObjectWin.SetActive(true);
                    sceneWin.Play();
                }
                else
                {
                    sceneObjectLose.SetActive(true);
                    sceneLose.Play();
                }
            }
        }
        else if (step == 2)
        {
            if (GameWin)
            {
                timerSceneWin += Time.deltaTime;
                if (timerSceneWin >= durationSceneWin)
                {
                    sceneObjectWin.SetActive(false);
                    isPlaying = false;
                    step = 0;
                    Debug.Log("Cinemáticas terminadas, el jugador ha ganado");
                    //Aqui activar el menu de win

                }
            }
            else
            {
                timerSceneLose += Time.deltaTime;
                if (timerSceneLose >= durationSceneLose)
                {
                    sceneObjectLose.SetActive(false);
                    isPlaying = false;
                    step = 0;
                    Debug.Log("Cinemáticas terminadas, el jugador ha perdido");
                    //Aqui activar el menu de lose
                }
            }
        }
    }
}