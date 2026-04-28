using System.Collections.Generic;
using UnityEngine;

public class AttackRangeDetector : MonoBehaviour
{
    
    public PlayerMove2 player;


    private List<GameObject> enemiesInRange = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }

            UpdatePlayerRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            UpdatePlayerRange();
        }
    }

    private void Update()
    {
        enemiesInRange.RemoveAll(enemy => enemy == null);
        UpdatePlayerRange();
    }

    private void UpdatePlayerRange()
    {
        if (player != null)
        {
            player.SetEnemyInRange(enemiesInRange.Count > 0);
        }
    }

}

