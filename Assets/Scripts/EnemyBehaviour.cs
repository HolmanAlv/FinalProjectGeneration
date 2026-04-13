using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject player;
    public GameObject[] waypoints;
    
    [Header("Estados")]
    public bool isAttackingPlayer = false;
    public bool isAttackingFarm = false;
    
    [Header("Configuración (Debug)")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int randomIndex;
    [SerializeField] private bool endDistination = false;
    
    // Variables privadas
    private bool attackPlayer = false;
    int example = 10;
    
    void Awake()
    {
        FindObjects();
    }
    
    void Start()
    {
        if (waypoints.Length > 0)
            randomIndex = Random.Range(0, waypoints.Length);
    }
    
    void Update()
    {
        if (isAttackingPlayer && player != null && attackPlayer == false)
        {
            StartCoroutine(AttackPlayer());
        }
        if (!endDistination && !isAttackingPlayer)
        {
            PathFindingFarm();
        }
        else if (endDistination && !isAttackingPlayer)
        {
            isAttackingFarm = true;
            StartCoroutine(AttackFarm());
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Farm"))
        {
            endDistination = true;
        }
    }
    
    void PathFindingFarm()
    {
        if (waypoints.Length > 0 && randomIndex < waypoints.Length)
            agent.SetDestination(waypoints[randomIndex].transform.position);
        endDistination = true;
    }
    
    void PathFindingPlayer()
    {
        if (player != null)
            agent.SetDestination(player.transform.position);
    }
    
    IEnumerator AttackPlayer()
    {
        while (isAttackingPlayer && player != null && !isAttackingFarm)
        {
            PathFindingPlayer();
            //Access the life variable of the other script, and subtract the damage.
            example/*lifePlayer*/ = example/*lifePlayer*/ - example/*damage*/;
            //Animation of the attack.
            attackPlayer = true;
            yield return new WaitForSeconds(2f);
            attackPlayer = false;
        }
    }
    
    IEnumerator AttackFarm()
    {
        while (isAttackingFarm && !isAttackingPlayer)
        {
            //Access the life variable of the other script, and subtract the damage.
            example/*lifeFarm*/ = example/*lifeFarm*/ - example/*damage*/;
            //Animation of the attack.
            yield return new WaitForSeconds(2f);
        }
    }
    
    public void FindObjects()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Farm");
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }
}