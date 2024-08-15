using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    [System.Serializable]
    public class BulletType
    {
        public string name;
        public float damage;
        public float speed;
        public float lifeTime = 5f;
        public GameObject bulletPrefab;
        public ParticleSystem muzzleFlash;
    }

    public List<BulletType> availableBullets = new List<BulletType>();
    public int currentBulletIndex = 0;

    public float assaultFireRate = 10f;
    public float hipFireSpread = 5f;
    public float aimFireSpread = 1f;

    public Transform firePoint;
    public Camera fpsCam;

    private float nextAssaultFireTime = 0f;
    private bool isAiming = false;

    void Update()
    {
        isAiming = Input.GetMouseButton(1);

        if (Input.GetMouseButton(0) && Time.time >= nextAssaultFireTime)
        {
            nextAssaultFireTime = Time.time + 1f / assaultFireRate;
            ShootSingle();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleBulletType();
        }
    }

    void ShootSingle()
    {
        if (availableBullets.Count == 0) return;

        BulletType currentBullet = availableBullets[currentBulletIndex];

        if (currentBullet.muzzleFlash != null)
        {
            currentBullet.muzzleFlash.Play();
        }

        float currentSpread = isAiming ? aimFireSpread : hipFireSpread;
        Quaternion spreadRotation = CalculateSpread(currentSpread);
        FireBullet(currentBullet, firePoint.rotation * spreadRotation);
    }

    Quaternion CalculateSpread(float spread)
    {
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        return Quaternion.Euler(spreadX, spreadY, 0);
    }

    void FireBullet(BulletType bulletType, Quaternion rotation)
    {
        if (bulletType.bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletType.bulletPrefab, firePoint.position, rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = bullet.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.velocity = bullet.transform.forward * bulletType.speed;

            BulletBehavior bulletScript = bullet.GetComponent<BulletBehavior>();
            if (bulletScript == null)
            {
                bulletScript = bullet.AddComponent<BulletBehavior>();
            }
            bulletScript.damage = bulletType.damage;
            bulletScript.lifeTime = bulletType.lifeTime;

            Debug.Log("Bullet fired with damage: " + bulletScript.damage);

            Destroy(bullet, bulletType.lifeTime);
        }
    }

    void CycleBulletType()
    {
        if (availableBullets.Count == 0) return;

        currentBulletIndex = (currentBulletIndex + 1) % availableBullets.Count;
        Debug.Log($"Cambiado a bala: {availableBullets[currentBulletIndex].name}");
    }
}