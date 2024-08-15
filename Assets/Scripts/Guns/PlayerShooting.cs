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

    private int shotsFired = 0;
    private const int MAX_SHOTS = 1000;
    private List<GameObject> activeBullets = new List<GameObject>();

    // Nuevo: Diccionario para almacenar copias de los prefabs
    private Dictionary<string, GameObject> bulletPrefabCopies = new Dictionary<string, GameObject>();

    void Start()
    {
        Debug.Log($"PlayerShooting iniciado. Balas disponibles: {availableBullets.Count}");
        for (int i = 0; i < availableBullets.Count; i++)
        {
            if (availableBullets[i].bulletPrefab == null)
            {
                Debug.LogError($"El prefab de la bala '{availableBullets[i].name}' en el índice {i} es nulo.");
            }
            else
            {
                // Nuevo: Crear una copia del prefab
                GameObject prefabCopy = Instantiate(availableBullets[i].bulletPrefab);
                prefabCopy.SetActive(false);
                DontDestroyOnLoad(prefabCopy);
                bulletPrefabCopies[availableBullets[i].name] = prefabCopy;
                Debug.Log($"Copia del prefab creada para '{availableBullets[i].name}'");
            }
        }
    }

    void Update()
    {
        isAiming = Input.GetMouseButton(1);

        if (Input.GetMouseButton(0))
        {
            TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleBulletType();
        }

        activeBullets.RemoveAll(bullet => bullet == null);
    }

    void TryShoot()
    {
        if (Time.time >= nextAssaultFireTime && shotsFired < MAX_SHOTS)
        {
            nextAssaultFireTime = Time.time + 1f / assaultFireRate;
            ShootSingle();
            shotsFired++;
        }
        else if (shotsFired >= MAX_SHOTS)
        {
            Debug.LogWarning("Límite de disparos alcanzado. Reiniciando contador.");
            shotsFired = 0;
        }
    }

    void ShootSingle()
    {
        if (availableBullets.Count == 0)
        {
            Debug.LogError("No hay balas disponibles para disparar.");
            return;
        }

        BulletType currentBullet = availableBullets[currentBulletIndex];

        // Nuevo: Usar la copia del prefab en lugar del original
        GameObject bulletPrefab = bulletPrefabCopies.ContainsKey(currentBullet.name)
            ? bulletPrefabCopies[currentBullet.name]
            : currentBullet.bulletPrefab;

        if (bulletPrefab == null)
        {
            Debug.LogError($"El prefab de la bala '{currentBullet.name}' es nulo. No se puede disparar.");
            return;
        }

        if (currentBullet.muzzleFlash != null)
        {
            currentBullet.muzzleFlash.Play();
        }

        float currentSpread = isAiming ? aimFireSpread : hipFireSpread;
        Quaternion spreadRotation = CalculateSpread(currentSpread);
        GameObject bullet = FireBullet(currentBullet, firePoint.rotation * spreadRotation, bulletPrefab);

        if (bullet != null)
        {
            activeBullets.Add(bullet);
            Debug.Log($"Bala disparada: {currentBullet.name}, Daño: {currentBullet.damage}, Disparos totales: {shotsFired}, Balas activas: {activeBullets.Count}");
        }
        else
        {
            Debug.LogError($"No se pudo crear la bala '{currentBullet.name}'.");
        }
    }

    Quaternion CalculateSpread(float spread)
    {
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        return Quaternion.Euler(spreadX, spreadY, 0);
    }

    GameObject FireBullet(BulletType bulletType, Quaternion rotation, GameObject bulletPrefab)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        if (bullet == null)
        {
            Debug.LogError($"No se pudo instanciar el prefab de la bala '{bulletType.name}'.");
            return null;
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody>();
            Debug.LogWarning($"Se añadió un Rigidbody a la bala '{bulletType.name}' porque no tenía uno.");
        }
        rb.useGravity = false;
        rb.velocity = bullet.transform.forward * bulletType.speed;

        BulletBehavior bulletScript = bullet.GetComponent<BulletBehavior>();
        if (bulletScript == null)
        {
            bulletScript = bullet.AddComponent<BulletBehavior>();
            Debug.LogWarning($"Se añadió un BulletBehavior a la bala '{bulletType.name}' porque no tenía uno.");
        }
        bulletScript.damage = bulletType.damage;
        bulletScript.lifeTime = bulletType.lifeTime;

        bullet.SetActive(true);
        return bullet;
    }

    void CycleBulletType()
    {
        if (availableBullets.Count == 0) return;

        currentBulletIndex = (currentBulletIndex + 1) % availableBullets.Count;
        Debug.Log($"Cambiado a bala: {availableBullets[currentBulletIndex].name}");
    }

    void OnDisable()
    {
        Debug.Log($"PlayerShooting desactivado. Balas activas restantes: {activeBullets.Count}");
    }
}