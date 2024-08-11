using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
    {
        public float MovimientoHorizontal;
        public float MovimientoVertical;
        private Vector3 PlayerInput;

        public CharacterController Player;

        public float PlayerSpeed = 5f;
        private Vector3 MovimientoPlayer;
        public float gravedad = 9.8f;
        private float velocidadVertical = 0f;
        public float FuerzaSalto = 2.5f;

        void Start()
        {
            Player = GetComponent<CharacterController>();
        }

        void Update()
        {
            MovimientoHorizontal = Input.GetAxis("Horizontal");
            MovimientoVertical = Input.GetAxis("Vertical");

            PlayerInput = new Vector3(MovimientoHorizontal, 0, MovimientoVertical);
            PlayerInput = Vector3.ClampMagnitude(PlayerInput, 1);

            // Mover en la dirección hacia la que el jugador está mirando
            MovimientoPlayer = transform.TransformDirection(PlayerInput) * PlayerSpeed;

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
    }