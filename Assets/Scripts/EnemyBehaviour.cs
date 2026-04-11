using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    int example = 10;

    public GameObject[] waypoints;
    [SerializeField] private NavMeshAgent agent;
    
    
    [SerializeField] private int randomIndex;
    [SerializeField] private bool endDistination = false;
    

    void Start()
    {
        randomIndex = Random.Range(0, waypoints.Length);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!endDistination)
        {
            PathFinding();
        }
        
    }

    void PathFinding()
    {
        agent.SetDestination(waypoints[randomIndex].transform.position);
        endDistination = true;
    }
    IEnumerator AttackFarm()
    {
        //Access the life variable of the other script, and subtract the damage.
        example/*lifeFarm*/ = example/*lifeFarm*/ - example/*damage*/;
        //Animation of the attack.
        yield return new WaitForSeconds(2f);
    }
}
