using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NewOrLoad : MonoBehaviour
{
    public static NewOrLoad Instance { get; private set; }
    public bool loadGame;
    public Button loadButton;
    public Button newButton;

    void Awake()
    {
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
            loadButton = botonObj.GetComponent<Button>();
            if (loadButton != null)
            {
                loadButton.onClick.RemoveAllListeners();
                loadButton.onClick.AddListener(LoadGame);
            }
        }
        else
        {
            Debug.Log("Botón con tag 'Continue' no encontrado en esta escena");
        }
        
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