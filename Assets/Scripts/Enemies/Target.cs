using UnityEngine;
using System;

public class Target : MonoBehaviour
{
    [System.Serializable]
    public class TargetType
    {
        public string name;
        public float maxHealth;
        public int pointValue;
        public GameObject deathEffect;
        public AudioClip hitSound;
        public AudioClip deathSound;
    }

    public TargetType targetType;

    [Header("Health")]
    public float currentHealth;
    public GameObject healthBarPrefab;
    private GameObject healthBarInstance;

    [Header("Effects")]
    public GameObject hitEffect;
    public Transform[] hitPoints;

    [Header("Audio")]
    public AudioSource audioSource;

    public event Action<Target> OnDestroyed;
    public static event Action<int> OnScoreChanged; // Nuevo evento para manejar la puntuación

    private void Start()
    {
        currentHealth = targetType.maxHealth;
        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity, transform);
            UpdateHealthBar();
        }
    }

    public void TakeDamage(float amount, Vector3 hitPoint)
    {
        currentHealth -= amount;
        UpdateHealthBar();

        PlayHitEffect(hitPoint);
        PlayHitSound();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarInstance != null)
        {
            float healthPercentage = currentHealth / targetType.maxHealth;
            healthBarInstance.transform.localScale = new Vector3(healthPercentage, 1f, 1f);
        }
    }

    private void PlayHitEffect(Vector3 hitPoint)
    {
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, hitPoint, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    private void PlayHitSound()
    {
        if (audioSource != null && targetType.hitSound != null)
        {
            audioSource.PlayOneShot(targetType.hitSound);
        }
    }

    private void Die()
    {
        if (targetType.deathEffect != null)
        {
            Instantiate(targetType.deathEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null && targetType.deathSound != null)
        {
            AudioSource.PlayClipAtPoint(targetType.deathSound, transform.position);
        }

        OnDestroyed?.Invoke(this);
        OnScoreChanged?.Invoke(targetType.pointValue); // Invocamos el evento de puntuación

        Destroy(gameObject);
    }

    public Vector3 GetRandomHitPoint()
    {
        if (hitPoints != null && hitPoints.Length > 0)
        {
            return hitPoints[UnityEngine.Random.Range(0, hitPoints.Length)].position;
        }
        return transform.position;
    }
}