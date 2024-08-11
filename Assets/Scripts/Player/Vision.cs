using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public float Velocidad = 100f;
    float RotacionX = 0f;

    public Transform Player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Opcional: Bloquear el cursor
    }

    void Update()
    {
        // Obtener el movimiento del rat�n
        float RatonX = Input.GetAxis("Mouse X") * Velocidad * Time.deltaTime;
        float RatonY = Input.GetAxis("Mouse Y") * Velocidad * Time.deltaTime;

        // Rotaci�n vertical de la c�mara
        RotacionX -= RatonY;
        RotacionX = Mathf.Clamp(RotacionX, -90f, 90f);
        transform.localRotation = Quaternion.Euler(RotacionX, 0f, 0f);

        // Rotaci�n horizontal del jugador
        Player.Rotate(Vector3.up * RatonX);
    }
}