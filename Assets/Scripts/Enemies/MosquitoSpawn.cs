using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
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

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float currentSpawnInterval;
    private int currentDifficultyLevel = 0;

    void OnValidate()
    {
        Debug.Log("OnValidate llamado. Verificando tipos de enemigos.");
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            if (enemyTypes[i].enemyPrefab == null)
            {
                Debug.LogError($"El prefab del enemigo en el índice {i} es nulo en OnValidate.");
            }
        }
    }

    void Awake()
    {
        Debug.Log("Awake llamado. Verificando tipos de enemigos.");
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            if (enemyTypes[i].enemyPrefab == null)
            {
                Debug.LogError($"El prefab del enemigo en el índice {i} es nulo en Awake.");
            }
        }
    }

    void Start()
    {
        Debug.Log($"Start llamado. EnemySpawner iniciado. Tipos de enemigos definidos: {enemyTypes.Count}");

        for (int i = 0; i < enemyTypes.Count; i++)
        {
            if (enemyTypes[i].enemyPrefab == null)
            {
                Debug.LogError($"El prefab del enemigo en el índice {i} es nulo en Start.");
            }
            else
            {
                Debug.Log($"Enemigo {i}: {enemyTypes[i].enemyPrefab.name}, Peso: {enemyTypes[i].weight}");
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
        StartCoroutine(SpawnEnemies());
        StartCoroutine(IncreaseDifficulty());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            activeEnemies.RemoveAll(enemy => enemy == null);

            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
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
            Debug.LogWarning($"El prefab del enemigo seleccionado es nulo. Índice: {enemyTypes.IndexOf(enemyType)}");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject newEnemy = Instantiate(enemyType.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(newEnemy);

        Debug.Log($"Enemigo spawneado: {newEnemy.name} en posición {spawnPoint.position}");

        Target enemyTarget = newEnemy.GetComponent<Target>();
        if (enemyTarget != null)
        {
            enemyTarget.OnDestroyed += (target) => activeEnemies.Remove(newEnemy);
        }
        else
        {
            Debug.LogWarning($"El enemigo {newEnemy.name} no tiene un componente Target.");
        }

        Renderer enemyRenderer = newEnemy.GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            Debug.Log($"Enemigo {newEnemy.name} tiene Renderer. Visible: {enemyRenderer.isVisible}");
        }
        else
        {
            Debug.LogWarning($"El enemigo {newEnemy.name} no tiene un componente Renderer.");
        }
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