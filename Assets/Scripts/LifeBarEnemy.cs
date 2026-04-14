using UnityEngine;
using UnityEngine.UI;

public class LifeBarEnemy : MonoBehaviour
{
    public Camera mainCamera;//para la rotacion
    public Image lifeBar;
    public float maxHelth = 100f;
    public float currentHelth = 100f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main; //rotacion
    }
    void Update()
    {
        lifeBar.fillAmount = currentHelth / maxHelth;
    }
    void LateUpdate() //rotacion
    {
        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0;
        transform.forward = direction;
    }
}
