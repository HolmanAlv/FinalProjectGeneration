using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NewOrLoad : MonoBehaviour
{
    public static NewOrLoad Instance { get; private set; }
    public bool loadGame;
    public Button loadButtonFuncional;
    public bool buttonActive;
    public GameObject loadButtonNoFuncional;
    public Button newButton;
    private string saveFile;
    private DataGame dataGame = new DataGame();

    void Awake()
    {
        saveFile = Application.dataPath + "/dataGame.json";
        if (File.Exists(saveFile))
        {
            buttonActive = true;
        }
        else
        {
            buttonActive = false;
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuscarBotonSiExiste();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void BuscarBotonSiExiste()
    {
        GameObject botonObj = GameObject.FindWithTag("Continue");
        
        if (botonObj != null)
        {
            loadButtonFuncional = botonObj.GetComponent<Button>();
            if (loadButtonFuncional != null)
            {
                loadButtonFuncional.onClick.RemoveAllListeners();
                loadButtonFuncional.onClick.AddListener(LoadGame);
            }
        }
        else
        {
            Debug.Log("Botón con tag 'Continue' no encontrado en esta escena");
        }

        loadButtonNoFuncional = GameObject.FindWithTag("ImgContinue");
        
        GameObject botonNewObj = GameObject.FindWithTag("NewGame");
        
        if (botonNewObj != null)
        {
            newButton = botonNewObj.GetComponent<Button>();
            if (newButton != null)
            {
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(NewGame);
            }
        }
        else
        {
            Debug.Log("Botón con tag 'NewGame' no encontrado en esta escena Debuglog desde NewOrLoad");
        }

        if (loadButtonNoFuncional != null && loadButtonFuncional != null && !buttonActive)
        {
            botonObj.SetActive(false);
            loadButtonNoFuncional.SetActive(true);
        }
        else if (loadButtonNoFuncional != null && loadButtonFuncional != null && buttonActive)
        {
            botonObj.SetActive(true);
            loadButtonNoFuncional.SetActive(false);
        }
    }
    
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        BuscarBotonSiExiste();
    }
    
    public void NewGame()
    {
        loadGame = false;
        Debug.Log("Nuevo juego - loadGame = false Debuglog desde NewOrLoad");
    }
    
    public void LoadGame()
    {
        loadGame = true;
        Debug.Log("Cargar juego - loadGame = true Debuglog desde NewOrLoad");
    }
}