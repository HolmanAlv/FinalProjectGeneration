using UnityEngine;

public class AttackRangeDetector : MonoBehaviour
{
    
    public PlayerMove2 player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy detectado en rango");
            player.SetEnemyInRange(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy salió del rango");
            player.SetEnemyInRange(false);
        }
    }
}

