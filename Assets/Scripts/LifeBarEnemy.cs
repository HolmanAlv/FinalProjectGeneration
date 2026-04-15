using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeBarEnemy : MonoBehaviour
{
    [Header("Referencias")]
    public EnemyBehaviour enemyBehaviour;
    public GameObject enemyHeader;
    public Canvas canvas;
    public Camera mainCamera;
    public Image lifeBar;
    public GameObject power;
    
    [Header("Vida")]
    public float maxHelth = 100f;
    public float currentHelth = 100f;
    
    [Header("Rotación de la Barra")]
    public Vector3 customRotation = new Vector3(0, 0, 0); // ← AJUSTA ESTO EN EL INSPECTOR
    public bool usarRotacionPersonalizada = true; // ← Activar/desactivar
    
    [Header("Estado")]
    public bool isVisible = true;
    public bool puedeRecibirDaño = true;

    void Start()
    {
        power.SetActive(false);
        if (mainCamera == null) mainCamera = Camera.main;
    }
    void LateUpdate()
    {
        if (isVisible)
        {
            lifeBar.fillAmount = currentHelth / maxHelth;
            
            if (canvas != null)
            {
                if (usarRotacionPersonalizada)
                {
                    //Rotación personalizada (ajustable en tiempo real)
                    canvas.transform.rotation = Quaternion.Euler(customRotation);
                }
                else
                {
                    //Rotación fija (si no quieres personalizada)
                    canvas.transform.rotation = Quaternion.identity;
                }
            }
        }
        
        if (currentHelth <= 0)
        {
            power.SetActive(true);
            PowerBehaviour powerBehaviour = power.GetComponent<PowerBehaviour>();
            powerBehaviour.ExitTheFather();
            Destroy(enemyHeader);
        }
    }

    public void RecibirDaño()
    {
        if (!puedeRecibirDaño) return;
        
        puedeRecibirDaño = false;

        if (enemyBehaviour != null && enemyBehaviour.anim != null)
        {
            enemyBehaviour.anim.SetTrigger("Damage");
        }

        currentHelth -= enemyBehaviour.dañoArma;
        Debug.Log("Vida enemigo: " + currentHelth);

        Invoke(nameof(ResetDaño), 0.75f);
    }

    private void ResetDaño()
    {
        puedeRecibirDaño = true;
    }
}