using System.Collections;
using UnityEngine;

public class ManagerEnemy : MonoBehaviour
{
    [Header("=== CONFIGURACIÓN DE SPAWN ===")]
    public GameObject enemyPrefab;
    public GameObject[] spawnPoints;
    public float radioSpawn = 5f;
    
    [Header("=== ESTADÍSTICAS DEL JUEGO ===")]
    public int maxEnemies = 10;
    public float enemySpeed = 4f;
    public float spawnInterval = 2f;
    
    [Header("=== LÍMITES MÁXIMOS ===")]
    public int topeMaxEnemies = 100;
    public float topeMaxSpeed = 15f;
    public float topeMinSpawnTime = 0.3f;
    
    [Header("=== INCREMENTOS PROGRESIVOS (Debug) ===")]
    [SerializeField] private float incrementoEnemies = 14f;
    [SerializeField] private float incrementoSpeed = 1.5f;
    [SerializeField] private float incrementoSpawn = -0.3f;
    [SerializeField] private float factorReduccion = 0.80f;
    
    // Variables privadas
    private bool enemyAttack = false;
    private int enemiesInScene = 0;
    
    void Awake()
    {
        StartCoroutine(SpawnRoutine());
    }
    
    void Update()
    {
        if (!enemyAttack && enemiesInScene == maxEnemies)
        {
            EnemyAttack();
        }
        else if (enemyAttack && enemiesInScene < (maxEnemies - 1))
        {
            enemyAttack = false;
        }
    }
    
    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;
        
        int spawnRandom = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[spawnRandom].transform.position;
        float randomX = spawnPosition.x + Random.Range(-radioSpawn, radioSpawn);
        float randomZ = spawnPosition.z + Random.Range(-radioSpawn, radioSpawn);
        
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(randomX, 0, randomZ), Quaternion.identity);
        ApplySpeedToEnemy(newEnemy);
    }
    
    void ApplySpeedToEnemy(GameObject enemy)
    {
        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.speed = enemySpeed;
    }
    
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy").Length;
            
            if (enemiesInScene < maxEnemies)
            {
                SpawnEnemy();
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    public void EnemyAttack()
    {
        if (enemiesInScene == 0) return;
        
        int randomEnemyIndex1 = Random.Range(0, enemiesInScene);
        int randomEnemyIndex2 = Random.Range(0, enemiesInScene);
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyBehaviour enemy1 = enemies[randomEnemyIndex1].GetComponent<EnemyBehaviour>();
        EnemyBehaviour enemy2 = enemies[randomEnemyIndex2].GetComponent<EnemyBehaviour>();
        
        enemy1.isAttackingPlayer = true;
        enemy2.isAttackingPlayer = true;
        enemyAttack = true;
    }
    
    [ContextMenu("▲ Aumentar Dificultad")]
    public void AumentarDificultad()
    {
        AumentarLimiteEnemigos();
        AumentarVelocidadEnemigos();
        ReducirTiempoSpawn();
    }
    
    [ContextMenu("▲ Aumentar solo Límite de Enemigos")]
    public void AumentarLimiteEnemigos()
    {
        int sumar = Mathf.Max(1, Mathf.RoundToInt(incrementoEnemies));
        maxEnemies = Mathf.Min(maxEnemies + sumar, topeMaxEnemies);
        incrementoEnemies *= factorReduccion;
    }
    
    [ContextMenu("▲ Aumentar Velocidad de Enemigos")]
    public void AumentarVelocidadEnemigos()
    {
        enemySpeed = Mathf.Min(enemySpeed + incrementoSpeed, topeMaxSpeed);
        incrementoSpeed *= factorReduccion;
        UpdateAllEnemiesSpeed();
    }
    
    [ContextMenu("▼ Reducir Tiempo de Spawn")]
    public void ReducirTiempoSpawn()
    {
        spawnInterval = Mathf.Max(spawnInterval - Mathf.Abs(incrementoSpawn), topeMinSpawnTime);
        incrementoSpawn *= factorReduccion;
    }
    
    void UpdateAllEnemiesSpeed()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            ApplySpeedToEnemy(enemy);
        }
    }
    
    [ContextMenu("⟳ Resetear Todo")]
    public void ResetearTodo()
    {
        maxEnemies = 10;
        enemySpeed = 4f;
        spawnInterval = 2f;
        incrementoEnemies = 14f;
        incrementoSpeed = 1.5f;
        incrementoSpawn = -0.3f;
        UpdateAllEnemiesSpeed();
    }
}