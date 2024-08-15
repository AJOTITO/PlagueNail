using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 20f;
    public float lifeTime = 5f;
    public float impactForce = 30f;

    public GameObject impactEffect;
    public AudioClip impactSound;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.gameObject, collision.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject, other.transform.position);
    }

    void HandleImpact(GameObject hitObject, Vector3 hitPoint)
    {
        // Efecto de impacto
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, hitPoint, Quaternion.LookRotation(transform.forward));
            Destroy(impact, 2f);
        }

        // Sonido de impacto
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, hitPoint);
        }

        // Aplicar daño si es un objetivo
        Target target = hitObject.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage, hitPoint);

            // Aplicar fuerza de impacto
            Rigidbody targetRb = hitObject.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                targetRb.AddForceAtPosition(transform.forward * impactForce, hitPoint, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }
}