using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IrAlJuego()
    {
        SceneManager.LoadScene("Prototipo2");
    }


    public void IrAlMenuprincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
