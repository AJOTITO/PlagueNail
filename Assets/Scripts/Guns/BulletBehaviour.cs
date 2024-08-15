using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float damage;
    public float lifeTime;
    private float creationTime;

    private void OnEnable()
    {
        creationTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - creationTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject hitObject)
    {
        Debug.Log($"Bala colisionó con: {hitObject.name}");

        Target target = hitObject.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log($"Daño aplicado a {hitObject.name}: {damage}");
        }
        else
        {
            Debug.Log($"El objeto {hitObject.name} no tiene un componente Target");
        }

        Destroy(gameObject);
    }
}