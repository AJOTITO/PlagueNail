using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float MovimientoHorizontal;
    public float MovimientoVertical;
    private Vector3 PlayerInput;

    public CharacterController Player;

    [Header("Movement")]
    public float PlayerSpeed = 5f;  // Velocidad base
    public float maxSpeed = 10f;  // Velocidad máxima
    public float acceleration = 0.5f;  // Incremento de velocidad por segundo
    private float currentSpeed;
    private Vector3 MovimientoPlayer;
    public float gravedad = 9.8f;
    private float velocidadVertical = 0f;
    public float FuerzaSalto = 2.5f;

    [Header("Dash")]
    public float dashForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;  // Tiempo de espera entre dashes
    public KeyCode dashKey = KeyCode.E;
    private bool isDashing = false;
    private Vector3 dashDirection;
    private float lastDashTime;  // Tiempo del último dash

    void Start()
    {
        Player = GetComponent<CharacterController>();
        currentSpeed = PlayerSpeed; // Inicializamos la velocidad actual a la velocidad base
        lastDashTime = -dashCooldown; // Para permitir dash al inicio del juego
    }

    void Update()
    {
        MovimientoHorizontal = Input.GetAxis("Horizontal");
        MovimientoVertical = Input.GetAxis("Vertical");

        PlayerInput = new Vector3(MovimientoHorizontal, 0, MovimientoVertical);
        PlayerInput = Vector3.ClampMagnitude(PlayerInput, 1);

        if (PlayerInput.magnitude > 0)
        {
            // Aumentar la velocidad si el jugador está moviéndose
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed); // Limitar la velocidad al máximo permitido
        }
        else
        {
            // Reiniciar la velocidad si el jugador se detiene
            currentSpeed = PlayerSpeed;
        }

        // Mover en la dirección hacia la que el jugador está mirando
        MovimientoPlayer = transform.TransformDirection(PlayerInput) * currentSpeed;

        // Aplicar dash si es necesario
        if (Input.GetKeyDown(dashKey) && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash());
        }

        // Aplicar gravedad
        SetGravity();

        // Manejar el salto
        Salto();

        // Aplicar movimiento y velocidad vertical
        MovimientoPlayer.y = velocidadVertical;

        // Mover al jugador
        Player.Move(MovimientoPlayer * Time.deltaTime);
    }

    void SetGravity()
    {
        if (Player.isGrounded)
        {
            if (velocidadVertical < 0)
            {
                velocidadVertical = -gravedad * Time.deltaTime;
            }
        }
        else
        {
            velocidadVertical -= gravedad * Time.deltaTime;
        }
    }

    void Salto()
    {
        if (Player.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocidadVertical = FuerzaSalto;
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        lastDashTime = Time.time;  // Registrar el tiempo del último dash
        dashDirection = transform.TransformDirection(PlayerInput);

        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            Player.Move(dashDirection * dashForce * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }
}
