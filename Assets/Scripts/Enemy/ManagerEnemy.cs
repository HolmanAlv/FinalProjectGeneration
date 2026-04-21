using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ManagerEnemy : MonoBehaviour
{
    #region Variables

    public EnemyBehaviour enemyBehaviour;

    [Header("Implementación del cambio de estado entre dia y nnoche")]
    public DayNiightManager dayNiightManager;
    private Coroutine spawnCoroutine;


    #region Configuración de Spawn

    [Header("=== CONFIGURACIÓN DE SPAWN ===")]
    public GameObject enemyPrefab;
    public GameObject[] spawnPoints;
    public float radioSpawn = 5f;

    #endregion

    #region Estadísticas del Juego

    [Header("=== ESTADÍSTICAS DEL JUEGO ===")]
    public float speedAnim = 0.3f;
    public int maxEnemies = 10;
    public float enemySpeed = 4f;
    public float spawnInterval = 2f;
    public int weaponDamage = 100;

    #endregion

    #region Límites Máximos

    [Header("=== LÍMITES MÁXIMOS ===")]
    public int topeMaxEnemies = 100;
    public float topeMaxSpeed = 15f;
    public float topeMinSpawnTime = 0.3f;
    public int topeMinDamage = 20;
    public float topeMaxSpeedAnim = 2f;

    #endregion

    #region Incrementos Progresivos

    [Header("=== INCREMENTOS PROGRESIVOS (Debug) ===")]
    [SerializeField]
    private float incrementoEnemies = 14f;

    [SerializeField]
    private float incrementoSpeed = 1.5f;

    [SerializeField]
    private float incrementoSpawn = -0.3f;

    [SerializeField]
    private float factorReduccion = 0.80f;

    [SerializeField]
    private float incrementoDamage = -20f;

    [SerializeField]
    private float incrementoSpeedAnim = 0.2f;

    #endregion
    // Variables privadas
    #region Variable privadas

    private bool enemyAttack = false;
    private int enemiesInScene = 0;
    private List<GameObject> enemiesList = new List<GameObject>();

    #endregion


    #endregion


    #region Metodos Spawn, Ataque, Contador enemigos
    #region Metodos Spwan

    

    private void OnEnable()
    {
        if (dayNiightManager != null)
        {
            dayNiightManager.OnNightStarted += HandleNightStarted;
            dayNiightManager.OnTransitionToDayStarted += StopSpawn;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            enemiesInScene = enemiesList.Count;

            if (enemiesInScene < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {

        if (spawnPoints.Length == 0)
            return;

        int spawnRandom = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[spawnRandom].transform.position;
        float randomX = spawnPosition.x + Random.Range(-radioSpawn, radioSpawn);
        float randomZ = spawnPosition.z + Random.Range(-radioSpawn, radioSpawn);

        GameObject newEnemy = Instantiate(
            enemyPrefab,
            new Vector3(randomX, 0, randomZ),
            Quaternion.identity
        );
        enemiesList.Add(newEnemy);
        ApplySpeedToEnemy(newEnemy);
        EnemyBehaviour behaviour = newEnemy.GetComponent<EnemyBehaviour>();

        if (behaviour != null)
        {
            behaviour.anim.speed = speedAnim;
            behaviour.dañoArma = weaponDamage;
        }
    }

    #endregion
    #region Metodos atacar al jugador

    void Update()
    {

        if (dayNiightManager == null) return;
        if (!dayNiightManager.IsNight) return;

        enemiesInScene = enemiesList.Count;

        if (!enemyAttack && enemiesInScene == maxEnemies)
        {
            EnemyAttack();
        }
        else if (enemyAttack && enemiesInScene < maxEnemies )
        {
            enemyAttack = false;
        }
    }


    private void OnDisable()
    {
        if (dayNiightManager != null)
        {
            dayNiightManager.OnNightStarted -= HandleNightStarted;
            dayNiightManager.OnTransitionToDayStarted -= StopSpawn;
        }
    }

    void StartSpawn()
    {
        if (spawnCoroutine != null) return;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void HandleNightStarted(int nightNumber)
    {
        ApplyDifficultyForNight(nightNumber);
        Debug.Log("⚔️ ManagerEnemy recibe noche: " + nightNumber);

        StartSpawn();
    }

    void StopSpawn()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        List<GameObject> enemiesCopy = new List<GameObject>(enemiesList);

        enemiesList.Clear();
        enemiesInScene = 0;
        enemyAttack = false;

        foreach (GameObject enemy in enemiesCopy)
        {
            if (enemy == null)
                continue;

            LifeBarEnemy lifeBar = enemy.GetComponentInChildren<LifeBarEnemy>();

            if (lifeBar != null)
            {
                lifeBar.RemoveByDayTransition();
            }
            else
            {
                Destroy(enemy);
            }
        }
    }


    public void EnemyAttack()
    {
        if (enemiesInScene == 0)
            return;

        int randomEnemyIndex1 = Random.Range(0, enemiesInScene);
        int randomEnemyIndex2 = Random.Range(0, enemiesInScene);

        EnemyBehaviour enemy1 = enemiesList[randomEnemyIndex1].GetComponent<EnemyBehaviour>();
        EnemyBehaviour enemy2 = enemiesList[randomEnemyIndex2].GetComponent<EnemyBehaviour>();

        enemy1.StartAttackingPlayer();
        enemy2.StartAttackingPlayer();
        enemyAttack = true;
    }

    #endregion

    public void RemoveEnemy(GameObject enemy)
    {
        enemiesList.Remove(enemy);
        enemiesInScene = enemiesList.Count;
    }

    #endregion


    #region Metodos para configurar la dificultad del juego


    #region Metodo para actualizar velocidad de enemigos

    void ApplySpeedToEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.speed = enemySpeed;

        EnemyBehaviour behaviour = enemy.GetComponent<EnemyBehaviour>();
        if (behaviour != null && behaviour.anim != null)
        {
            behaviour.anim.speed = speedAnim;
        }
    }

    #endregion

    private void ApplyDifficultyForNight(int nightNumber)
    {
        ResetearTodo();

        int increments = Mathf.Max(0, nightNumber - 1);

        for (int i = 0; i < increments; i++)
        {
            AumentarDificultad();
        }
    }

    #region Aumento de Dificultad

    [ContextMenu("▲ Aumentar Dificultad")]
    public void AumentarDificultad()
    { 
        AumentarLimiteEnemigos();
        AumentarVelocidadEnemigos();
        ReducirTiempoSpawn();
        ReducirDañoArma();
    }

    #endregion

    #region Reducir daño del arma

    [ContextMenu("▼ Reducir Daño del Arma")]
    public void ReducirDañoArma()
    {
        int anterior = weaponDamage;
        int restar = Mathf.RoundToInt(Mathf.Abs(incrementoDamage));

        weaponDamage = Mathf.Max(weaponDamage - restar, topeMinDamage);

        incrementoDamage *= factorReduccion;

        Debug.Log($"Daño del arma: {anterior} → {weaponDamage} (-{restar})");

        // Actualizar daño en todos los enemigos existentes
        UpdateAllEnemiesDamage();
    }

    #endregion

    #region Actualizar daño en enemigos

    void UpdateAllEnemiesDamage()
    {
        enemiesList.RemoveAll(enemy => enemy == null);

        foreach (GameObject enemy in enemiesList)
        {
            if (enemy != null)
            {
                EnemyBehaviour behaviour = enemy.GetComponent<EnemyBehaviour>();
                if (behaviour != null)
                {
                    behaviour.dañoArma = weaponDamage;
                }
            }
        }
    }

    #endregion

    #region Aumentar limite de enemigos

    [ContextMenu("▲ Aumentar solo Límite de Enemigos")]
    public void AumentarLimiteEnemigos()
    {
        int sumar = Mathf.Max(1, Mathf.RoundToInt(incrementoEnemies));
        maxEnemies = Mathf.Min(maxEnemies + sumar, topeMaxEnemies);
        incrementoEnemies *= factorReduccion;
    }

    #endregion

    #region Aumentar velocidad de enemigos

    [ContextMenu("▲ Aumentar Velocidad de Enemigos")]
    public void AumentarVelocidadEnemigos()
    {
        enemySpeed = Mathf.Min(enemySpeed + incrementoSpeed, topeMaxSpeed);
        incrementoSpeed *= factorReduccion;
        speedAnim = Mathf.Min(speedAnim + incrementoSpeedAnim, topeMaxSpeedAnim);
        UpdateAllEnemiesSpeed();
    }

    #endregion

    #region Reducir tiempo de spawn

    [ContextMenu("▼ Reducir Tiempo de Spawn")]
    public void ReducirTiempoSpawn()
    {
        spawnInterval = Mathf.Max(spawnInterval - Mathf.Abs(incrementoSpawn), topeMinSpawnTime);
        incrementoSpawn *= factorReduccion;
    }

    #endregion

    #region Actualizar velocidad de enemigos en escena

    void UpdateAllEnemiesSpeed()
    {
        enemiesList.RemoveAll(enemy => enemy == null);

        foreach (GameObject enemy in enemiesList)
        {
            ApplySpeedToEnemy(enemy);
        }
    }

    #endregion

    #region Resetear todas las configuraciones a valores iniciales

    [ContextMenu("⟳ Resetear Todo")]
    public void ResetearTodo()
    {
        maxEnemies = 10;
        enemySpeed = 4f;
        spawnInterval = 2f;
        weaponDamage = 100;
        incrementoEnemies = 14f;
        incrementoSpeed = 1.5f;
        incrementoSpawn = -0.3f;
        incrementoDamage = -20f;
        UpdateAllEnemiesSpeed();
        UpdateAllEnemiesDamage();
    }

    #endregion


    #endregion
}
