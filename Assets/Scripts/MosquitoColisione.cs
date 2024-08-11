using UnityEngine;

public class MosquitoColisiona : MonoBehaviour
{
    private bool isDeactivated = false;  // Variable para controlar si el objeto ha sido desactivado

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDeactivated)
        {
            isDeactivated = true; // Marca el objeto como desactivado

            // Realiza cualquier otra acción ANTES de desactivar el objeto

            // Desactiva este objeto enemigo
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDeactivated)
        {
            // Asegúrate de que no estás realizando acciones adicionales si el objeto ha sido desactivado
            return;
        }

        // Aquí puedes continuar con cualquier otra lógica mientras el objeto no esté desactivado
    }
}