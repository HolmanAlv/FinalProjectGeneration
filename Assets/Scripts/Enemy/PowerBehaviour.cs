using UnityEngine;

public class PowerBehaviour : MonoBehaviour
{
    public int energyAmount = 1;

     private void OnTriggerEnter(Collider other) 
     {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerEnergy playerEnergy = other.gameObject.GetComponent<PlayerEnergy>();

            if (playerEnergy != null)
            {
                AudioManager.Instance.PlaySFX("collect_energy");
                playerEnergy.AddEnergy(energyAmount);
                Debug.Log("energiarecoelctada" + energyAmount);
            }
            else
            {
                Debug.LogWarning("No hay playerEnergy asignado");
            }
            Destroy(gameObject );
           
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
