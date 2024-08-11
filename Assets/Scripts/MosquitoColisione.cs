using UnityEngine;

public class MosquitoColisiona : MonoBehaviour
{
    private bool isDeactivated = false;  // Variable para controlar si el objeto ha sido desactivado

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDeactivated)
        {
            isDeactivated = true; // Marca el objeto como desactivado

            // Realiza cualquier otra acci�n ANTES de desactivar el objeto

            // Desactiva este objeto enemigo
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDeactivated)
        {
            // Aseg�rate de que no est�s realizando acciones adicionales si el objeto ha sido desactivado
            return;
        }

        // Aqu� puedes continuar con cualquier otra l�gica mientras el objeto no est� desactivado
    }
}