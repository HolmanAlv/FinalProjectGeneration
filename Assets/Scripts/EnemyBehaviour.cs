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
    
    [Header("Empuje")]
    public float fuerzaEmpuje = 30f;
    
    // Variables privadas
    private bool isAttacking = false;
    private bool collisionPlayer = false;
    int example = 10;
    private Rigidbody rbPlayer;
    
    void Awake()
    {
        FindObjects();
    }
    
    void Start()
    {
        if (waypoints.Length > 0)
            randomIndex = Random.Range(0, waypoints.Length);
            
        // Obtener el Rigidbody del jugador
        if (player != null)
            rbPlayer = player.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (isAttackingPlayer && player != null)
        {
            if (collisionPlayer && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
            else if (!collisionPlayer && !isAttacking)
            {
                PathFindingPlayer();
            }
        }
        if (!isAttackingPlayer)
        {
            if (!endDistination)
            {
                PathFindingFarm();
            }
            else if (endDistination && !isAttackingFarm)
            {
                isAttackingFarm = true;
                StartCoroutine(AttackFarm());
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Farm"))
        {
            endDistination = true;
        }
        if (other.CompareTag("Player"))
        {
            collisionPlayer = true;
            if (agent != null) agent.isStopped = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collisionPlayer = false;
            if (agent != null)
                agent.isStopped = false;
        }
    }
    
    void PathFindingFarm()
    {
        if (waypoints.Length > 0 && randomIndex < waypoints.Length)
        {
            agent.SetDestination(waypoints[randomIndex].transform.position);
        }
        endDistination = true;
    }
    
    void PathFindingPlayer()
    {
        if (player != null && agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
    }
    
    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        
        while (isAttackingPlayer && player != null && collisionPlayer)
        {
            if (agent != null) agent.isStopped = true;
            
            //empujar al jugador
            if (rbPlayer != null)
            {
                Vector3 direccionAlJugador = player.transform.position - transform.position;
                direccionAlJugador.Normalize();
                
                Vector3 direccionEmpuje = direccionAlJugador;
                
                rbPlayer.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode.Impulse);
            }
            
            yield return new WaitForSeconds(2f);
        }

        isAttacking = false;
        if (agent != null) agent.isStopped = false;
    }
    
    IEnumerator AttackFarm()
    {
        while (isAttackingFarm && !isAttackingPlayer)
        {
            example/*lifeFarm*/ = example/*lifeFarm*/ - example/*damage*/;
            yield return new WaitForSeconds(2f);
        }
    }
    
    public void FindObjects()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Farm");
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
    }
    
    public void StartAttackingPlayer()
    {
        if (agent != null) agent.stoppingDistance = 0f;
        isAttackingFarm = false;
        endDistination = false;
        isAttackingPlayer = true;
        isAttacking = false;
        collisionPlayer = false;
    }
}