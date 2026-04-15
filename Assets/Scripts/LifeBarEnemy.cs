using UnityEngine;
using UnityEngine.UI;

public class LifeBarEnemy : MonoBehaviour
{
    public EnemyBehaviour enemyBehaviour;
    public GameObject enemyHeader;
    public Canvas canvas;
    public Camera mainCamera;
    public Image lifeBar;
    public float maxHelth = 100f;
    public float currentHelth = 100f;
    public bool isVisible = true;
    public bool puedeRecibirDaño = true;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }
    void Update()
    {
        if (isVisible)
        {
            lifeBar.fillAmount = currentHelth / maxHelth;
        }
        if (currentHelth <= 0)
        {
            Destroy(enemyHeader);
        }
    }

    public void RecibirDaño()
    {
        puedeRecibirDaño = false;

        // Animación
        if (enemyBehaviour.anim != null)
        {
            enemyBehaviour.anim.SetTrigger("Damage");
        }

        // Restar vida
        currentHelth -= enemyBehaviour.dañoArma;
        Debug.Log("Vida enemigo: " + currentHelth);



        // Evitar daño continuo
        Invoke(nameof(ResetDaño), 0.75f);
    }

    private void ResetDaño()
    {
        puedeRecibirDaño = true;
    }








//======================================================
    void OnBecameInvisible()
    {
        isVisible = false;
        if (canvas != null && canvas.gameObject != null) 
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void OnBecameVisible()
    {
        isVisible = true;
        if (canvas != null && canvas.gameObject != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }
}
