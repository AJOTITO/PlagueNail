using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public UnityEvent OnDestroyed;
    public UnityEvent<int> OnScoreChanged;

    [SerializeField] private int scoreValue = 10;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Salud actual: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} ha sido destruido.");
        OnScoreChanged.Invoke(scoreValue);
        OnDestroyed.Invoke();
        Destroy(gameObject);
    }
}