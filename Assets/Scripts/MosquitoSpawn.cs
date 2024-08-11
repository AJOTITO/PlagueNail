using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    public int spawnAmount = 5;
    public float spawnInterval = 2f;
    public Transform[] spawnPoints;

    private float spawnTimer;
    private List<GameObject> enemyPool = new List<GameObject>();

    void Start()
    {
        // Inicializa la piscina de enemigos
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        int currentEnemyCount = 0;

        // Cuenta cuántos enemigos están activos
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy)
                currentEnemyCount++;
        }

        if (currentEnemyCount < maxEnemies)
        {
            int enemiesToSpawn = Mathf.Min(spawnAmount, maxEnemies - currentEnemyCount);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Encuentra un enemigo desactivado en la piscina
                foreach (GameObject enemy in enemyPool)
                {
                    if (!enemy.activeInHierarchy)
                    {
                        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                        enemy.transform.position = spawnPoint.position;
                        enemy.transform.rotation = spawnPoint.rotation;
                        enemy.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
}