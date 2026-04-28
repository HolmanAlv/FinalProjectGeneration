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
    private Rigidbody rbPlayer;
    #endregion

    #endregion

    #region Métodos Principales

    void Awake()
    {
        FindObjects();

        if (lifeBarEnemy == null)
        {
            lifeBarEnemy = GetComponentInChildren<LifeBarEnemy>(true); // permite buscar en hijos desactivados
        }

        if (lifeBarEnemy == null)
        {
            Debug.LogError("No se encontró LifeBarEnemy en este enemigo o sus hijos");
        }

    }

    void Start()
{
    if (waypoints.Length > 0) randomIndex = Random.Range(0, waypoints.Length);

    if (player != null) rbPlayer = player.GetComponent<Rigidbody>();

        StartCoroutine(UpdateVisibilityRoutine());
    }

    void Update()
    {
        if (isAttackingPlayer && player != null)
        {
            Debug.Log($"Persiguiendo jugador - collisionPlayer: {collisionPlayer}, isAttacking: {isAttacking}");
            if (collisionPlayer && !isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
            else if (!collisionPlayer && !isAttacking)
            {
                Debug.Log("Llamando a PathFindingPlayer()");
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
            if (randomIndex < waypoints.Length && other.gameObject == waypoints[randomIndex]) 
            {  
                endDistination = true;
            }
        }
        if (other.CompareTag("Player"))
        {
            collisionPlayer = true;
            //if (agent != null) agent.isStopped = true;
        }
        if (other.CompareTag("Arma"))
        {
            if (lifeBarEnemy == null)
            {
                lifeBarEnemy = GetComponentInChildren<LifeBarEnemy>(true);
            }

            if (lifeBarEnemy == null)
            {
                Debug.LogWarning("Este enemigo no puede recibir daño porque no tiene LifeBarEnemy: " + gameObject.name);
                return;
            }

            if (!lifeBarEnemy.puedeRecibirDaño)
                return;

            WeaponLogic weaponLogic = other.GetComponentInParent<WeaponLogic>();

            if (weaponLogic == null)
            {
                Debug.LogWarning("No se encontró WeaponLogic");
                return;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("EnemyHit");
            }

            int damageFinal = weaponLogic.CurrentDamage;

            Debug.Log("Daño recibido por enemigo: " + damageFinal);

            lifeBarEnemy.RecibirDaño(damageFinal);
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
    }

    void PathFindingPlayer()
    {
        Debug.Log($"PathFindingPlayer - player: {player != null}, agent: {agent != null}");
        if (player != null && agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            Debug.Log($"Destino asignado: {player.transform.position}");
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
            anim.SetBool("Attack", true);
            yield return new WaitForSeconds(0.3f);

            if (rbPlayer != null)
            {
                Vector3 direccionAlJugador = player.transform.position - transform.position;
                direccionAlJugador.Normalize();
                Vector3 direccionEmpuje = direccionAlJugador;
                rbPlayer.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(1.5f);
        }
        anim.SetBool("Attack", false);
        isAttacking = false;
        if (agent != null) agent.isStopped = false;
    }

    IEnumerator AttackFarm()
{
    Debug.Log("Pegandole a la granja... socio");
    if (waypoints.Length == 0)
    {
        isAttackingFarm = false;
        yield break;
    }
    
    structure = waypoints[randomIndex].GetComponent<StructureHealth>();
    
    while (isAttackingFarm && !isAttackingPlayer)
    {
        bool waypointInvalido = false;
        
        if (randomIndex >= waypoints.Length || waypoints[randomIndex] == null)
        {
            waypointInvalido = true;
        }
        else if (!waypoints[randomIndex].activeInHierarchy)
        {
            waypointInvalido = true;
        }
        
        if (waypointInvalido)
        {
            Debug.Log("Waypoint destruido, actualizando lista...");
            waypoints = GameObject.FindGameObjectsWithTag("Farm");
            
            if (waypoints.Length > 0)
            {
                randomIndex = Random.Range(0, waypoints.Length);
                structure = waypoints[randomIndex].GetComponent<StructureHealth>();
                endDistination = false;

                agent.SetDestination(waypoints[randomIndex].transform.position);
            }
            else
            {

                isAttackingFarm = false;
                break;
            }
        }

        if (structure != null && !waypointInvalido)
        {
            anim.SetBool("Attack", true);
            yield return new WaitForSeconds(0.3f);
            structure.TakeDamage(damage);
            Debug.Log("Atacando granja: " + structure.gameObject.name);
        }
        
        yield return new WaitForSeconds(1.4f);
        anim.SetBool("Attack", false);
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
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    public void StartAttackingPlayer()
    {
        Debug.Log("StartAttackingPlayer llamado");
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