using UnityEngine;

public class CenterObjectOnScreen : MonoBehaviour
{
    public Camera mainCamera;
    public float distanceFromCamera = 1f;

    void Start()
    {
        // Si no se asign� una c�mara, usar la c�mara principal
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Asegurarse de que el objeto no sea hijo de la c�mara
        transform.SetParent(null);
    }

    void Update()
    {
        if (mainCamera != null)
        {
            // Calcular la posici�n en el centro de la pantalla
            Vector3 centerPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromCamera));

            // Mover el objeto a esa posici�n
            transform.position = centerPosition;

            // Hacer que el objeto mire hacia la c�mara
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0); // Girar 180 grados para que mire hacia adelante
        }
    }
}