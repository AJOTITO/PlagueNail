using UnityEngine;

public class DoomStyleWeapon : MonoBehaviour
{
    public Camera mainCamera;
    public float normalDistance = 0.5f;
    public float aimDistance = 0.3f;
    public float heightOffset = -0.2f;
    public Vector3 weaponScale = new Vector3(0.1f, 0.1f, 0.1f);
    public float aimSpeed = 5f; // Velocidad de transición al apuntar

    // Nuevas variables para el apuntado
    public Vector3 aimPosition = new Vector3(0f, 0.1f, 0.2f); // Posición relativa al apuntar
    public float fieldOfViewChange = 10f; // Cambio en el campo de visión al apuntar

    private Vector3 originalLocalPosition;
    private Vector3 originalLocalScale;
    private float originalFOV;
    private float currentDistance;
    private Vector3 currentAimOffset;
    private bool isAiming = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        originalLocalPosition = transform.localPosition;
        originalLocalScale = transform.localScale;
        originalFOV = mainCamera.fieldOfView;
        currentDistance = normalDistance;
        currentAimOffset = Vector3.zero;

        transform.SetParent(mainCamera.transform, false);
        transform.localScale = Vector3.Scale(originalLocalScale, weaponScale);
    }

    void Update()
    {
        // Comprobar si se está apuntando
        isAiming = Input.GetMouseButton(1); // Botón derecho para apuntar

        // Interpolar la distancia y la posición de apuntado
        currentDistance = Mathf.Lerp(currentDistance, isAiming ? aimDistance : normalDistance, Time.deltaTime * aimSpeed);
        currentAimOffset = Vector3.Lerp(currentAimOffset, isAiming ? aimPosition : Vector3.zero, Time.deltaTime * aimSpeed);

        // Calcular y aplicar la nueva posición del arma
        Vector3 newPosition = mainCamera.transform.position + mainCamera.transform.forward * currentDistance;
        newPosition += mainCamera.transform.up * heightOffset;
        newPosition += mainCamera.transform.right * currentAimOffset.x;
        newPosition += mainCamera.transform.up * currentAimOffset.y;
        newPosition += mainCamera.transform.forward * currentAimOffset.z;
        transform.position = newPosition;

        // Hacer que el arma mire en la misma dirección que la cámara
        transform.rotation = mainCamera.transform.rotation;

        // Ajustar el campo de visión de la cámara
        float targetFOV = isAiming ? originalFOV - fieldOfViewChange : originalFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * aimSpeed);
    }

    public void ChangeWeapon(GameObject newWeaponPrefab)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject newWeapon = Instantiate(newWeaponPrefab, transform);
        newWeapon.transform.localPosition = originalLocalPosition;
        newWeapon.transform.localScale = Vector3.Scale(newWeapon.transform.localScale, weaponScale);
    }
}