using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string name;
        public GameObject enemyPrefab;
        public int weight = 1;
    }

    public List<EnemyType> enemyTypes = new List<EnemyType>();
    public int maxEnemies = 5;
    public float initialSpawnInterval = 2f;
    public float minSpawnInterval = 0.5f;
    public float spawnIntervalDecreaseRate = 0.1f;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Difficulty Settings")]
    public float difficultyIncreaseInterval = 30f;
    public int maxDifficultyLevel = 10;

    public ScoreManager scoreManager;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float currentSpawnInterval;
    private int currentDifficultyLevel = 0;
    private Coroutine spawnCoroutine;
    private bool isSpawning = true;

    void Start()
    {
        Debug.Log($"EnemySpawner iniciado. Tipos de enemigos definidos: {enemyTypes.Count}");

        for (int i = 0; i < enemyTypes.Count; i++)
        {
            if (enemyTypes[i].enemyPrefab == null)
            {
                Debug.LogError($"El prefab del enemigo '{enemyTypes[i].name}' en el índice {i} es nulo.");
            }
            else
            {
                Debug.Log($"Enemigo {i}: {enemyTypes[i].name}, Peso: {enemyTypes[i].weight}");
            }
        }

        if (enemyTypes.Count == 0 || enemyTypes.All(et => et.enemyPrefab == null))
        {
            Debug.LogError("No hay tipos de enemigos válidos definidos en el EnemySpawner. Por favor, asigna al menos un prefab de enemigo en el Inspector.");
            enabled = false;
            return;
        }

        if (spawnPoints.Count == 0)
        {
            spawnPoints.Add(transform);
            Debug.LogWarning("No se definieron puntos de spawn. Usando la posición del spawner como punto de spawn predeterminado.");
        }

        currentSpawnInterval = initialSpawnInterval;
        spawnCoroutine = StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseDifficulty());
    }

    IEnumerator SpawnEnemies()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            try
            {
                CleanupDestroyedEnemies();

                if (activeEnemies.Count < maxEnemies)
                {
                    SpawnEnemy();
                }
                else
                {
                    Debug.Log($"No se generan más enemigos. Activos: {activeEnemies.Count}, Máximo: {maxEnemies}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error durante la generación de enemigos: {e.Message}");
            }
        }
    }

    void CleanupDestroyedEnemies()
    {
        int removedCount = activeEnemies.RemoveAll(enemy => enemy == null);
        if (removedCount > 0)
        {
            Debug.Log($"Se eliminaron {removedCount} enemigos destruidos de la lista activa.");
        }
    }

    void SpawnEnemy()
    {
        if (enemyTypes.Count == 0)
        {
            Debug.LogWarning("No hay tipos de enemigos definidos.");
            return;
        }

        EnemyType enemyType = GetRandomEnemyType();

        if (enemyType.enemyPrefab == null)
        {
            Debug.LogWarning($"El prefab del enemigo '{enemyType.name}' es nulo. No se puede spawner.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject newEnemy = Instantiate(enemyType.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(newEnemy);

        Debug.Log($"Enemigo spawneado: {newEnemy.name} en posición {spawnPoint.position}");

        Target enemyTarget = newEnemy.GetComponent<Target>();
        if (enemyTarget != null)
        {
            enemyTarget.OnDestroyed.AddListener(() => OnEnemyDestroyed(newEnemy));

            if (scoreManager != null)
            {
                enemyTarget.OnScoreChanged.AddListener(scoreManager.AddScore);
            }
            else
            {
                Debug.LogWarning("ScoreManager no está asignado en EnemySpawner.");
            }
        }
        else
        {
            Debug.LogWarning($"El enemigo {newEnemy.name} no tiene un componente Target.");
        }

        Debug.Log($"Enemigo generado. Total activos: {activeEnemies.Count}");
    }

    void OnEnemyDestroyed(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        Debug.Log($"Enemigo destruido. Total activos restantes: {activeEnemies.Count}");
    }

    EnemyType GetRandomEnemyType()
    {
        int totalWeight = 0;
        foreach (var enemyType in enemyTypes)
        {
            totalWeight += enemyType.weight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int weightSum = 0;

        foreach (var enemyType in enemyTypes)
        {
            weightSum += enemyType.weight;
            if (randomWeight < weightSum)
            {
                return enemyType;
            }
        }

        return enemyTypes[0];
    }

    IEnumerator IncreaseDifficulty()
    {
        while (currentDifficultyLevel < maxDifficultyLevel)
        {
            yield return new WaitForSeconds(difficultyIncreaseInterval);

            currentDifficultyLevel++;
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnIntervalDecreaseRate);
            maxEnemies++;

            Debug.Log($"Dificultad aumentada. Nivel: {currentDifficultyLevel}, Intervalo de spawn: {currentSpawnInterval}, Max enemigos: {maxEnemies}");
        }
    }

    void OnDisable()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}