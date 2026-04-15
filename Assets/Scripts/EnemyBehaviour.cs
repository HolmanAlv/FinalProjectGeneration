using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public int damage = 1;
    public StructureHealth structure;
    #region Variables

    #region Referencias
    [Header("Referencias")]
    public GameObject player;
    public GameObject[] waypoints;
    public Animator anim;
    public LifeBarEnemy lifeBarEnemy;
    public int dañoArma = 1;
    #endregion

    #region Estados
    [Header("Estados")]
    public bool isAttackingPlayer = false;
    public bool isAttackingFarm = false;
    #endregion

    #region Configuración (Debug)
    [Header("Configuración (Debug)")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int randomIndex;
    [SerializeField] private bool endDistination = false;
    #endregion

    #region Empuje
    [Header("Empuje")]
    public float fuerzaEmpuje = 30f;
    #endregion

    #region Variables Privadas
    private bool isAttacking = false;
    private bool collisionPlayer = false;
    private bool isVisible = true;
    private int example = 10;
    private Rigidbody rbPlayer;
    #endregion

    #endregion

    #region Métodos Principales

    void Awake()
    {
        FindObjects();
    }

    void Start()
    {
        if (waypoints.Length > 0)
            randomIndex = Random.Range(0, waypoints.Length);

        if (player != null)
            rbPlayer = player.GetComponent<Rigidbody>();
        if (lifeBarEnemy != null) lifeBarEnemy = GetComponentInChildren<LifeBarEnemy>();

        StartCoroutine(UpdateVisibilityRoutine());
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

    #endregion

    #region Trigger Events

    
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
        if (other.CompareTag("Arma") && lifeBarEnemy.puedeRecibirDaño)
        {
            Debug.Log("Orale cocazo");
            lifeBarEnemy.RecibirDaño();
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

    #endregion

    #region Pathfinding

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

    #endregion

    #region Ataques

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        while (isAttackingPlayer && player != null && collisionPlayer)
        {
            if (agent != null) agent.isStopped = true;

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
            if (structure != null)
            {
                structure.TakeDamage(damage);
                //animacion atacar
            }
            if (waypoints[randomIndex].gameObject.activeInHierarchy == false)
            {
                isAttackingFarm = false;
                endDistination = false;
                randomIndex = Random.Range(0, waypoints.Length);
                structure = waypoints[randomIndex].GetComponentInParent<StructureHealth>();
            }
            yield return new WaitForSeconds(2f);
        }
    }

    #endregion

    #region Visibilidad

    IEnumerator UpdateVisibilityRoutine()
    {
        while (true)
        {
            Camera mainCamera = Camera.main;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                    renderer = renderers[0];
            }

            if (renderer != null && mainCamera != null)
            {
                bool visibleNow = GeometryUtility.TestPlanesAABB(
                    GeometryUtility.CalculateFrustumPlanes(mainCamera),
                    renderer.bounds
                );

                if (visibleNow != isVisible)
                {
                    isVisible = visibleNow;
                    SetRenderersActive(visibleNow);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SetRenderersActive(bool active)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = active;
        }
    }

    #endregion

    #region Métodos Públicos

    public void FindObjects()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Farm");
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        structure = waypoints[randomIndex].GetComponentInParent<StructureHealth>();
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

    #endregion

    #region Destrucción

    void OnDestroy()
    {
        ManagerEnemy manager = FindObjectOfType<ManagerEnemy>();
        if (manager != null) manager.RemoveEnemy(gameObject);
    }

    #endregion
}