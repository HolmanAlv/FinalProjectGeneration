using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NewOrLoad : MonoBehaviour
{
    public static NewOrLoad Instance { get; private set; }
    public bool loadGame;

    public Button loadButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            string saveFile = Application.dataPath + "/dataGame.json";
            
            if (loadButton != null)
            {
                loadButton.enabled = File.Exists(saveFile);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        loadGame = false;
    }
    
    public void LoadGame()
    {
        loadGame = true;
    }
}