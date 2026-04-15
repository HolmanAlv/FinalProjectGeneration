using UnityEngine;

public class PowerBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player"))
        {
            //En el manager de inventario se puede agregar el poder al inventario del jugador
            Debug.Log("¡Poder recogido!");
            Destroy(gameObject);
        }
    }
    public void ExitTheFather()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }
}
