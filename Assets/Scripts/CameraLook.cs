using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    private Inputs inputs;

    public Transform target; // El objeto que la cámara seguirá (el personaje)
    public Vector3 offset; // Desplazamiento desde el objetivo
    public float sensitivity = 5f; // Sensibilidad de rotación
    public float gamepadSensitivityMultiplier = 2f; // Multiplicador para la sensibilidad del joystick
    public float distance = 5f; // Distancia de la cámara al objetivo
    public float minYAngle = -20f; // Ángulo mínimo en el eje Y (vertical)
    public float maxYAngle = 80f; // Ángulo máximo en el eje Y (vertical)
    public Transform character; // Referencia al personaje que debe rotar con la cámara

    private float currentX = 0f; // Rotación en el eje X (horizontal)
    private float currentY = 0f; // Rotación en el eje Y (vertical)
    private Vector2 mouseDelta;

    private DetectControls detectControls;

    private bool isGamepad = false;

    private void Awake()
    {
        inputs = new Inputs();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.Camera.Look.performed += OnMouseLook;
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.Camera.Look.performed -= OnMouseLook;
    }

    private void OnMouseLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        detectControls = FindAnyObjectByType<DetectControls>();
        detectControls.DeviceChanged += OnDeviceChanged;
    }

    private void OnDeviceChanged(DetectControls.PlayerDevice device)
    {

        Debug.Log("Se ejecutooo");
        switch (device)
        {
            case DetectControls.PlayerDevice.Keyboard:
                isGamepad = false;
                break;
            case DetectControls.PlayerDevice.Gamepad:
                isGamepad = true;
                break;
        }
    }

    void Update()
    {
        // Si hay un gamepad conectado, multiplicar la sensibilidad
        float effectiveSensitivity = isGamepad ? sensitivity * gamepadSensitivityMultiplier : sensitivity;

        //Debug.Log(effectiveSensitivity);

        // Rotación de la cámara
        currentX += mouseDelta.x * effectiveSensitivity * Time.deltaTime;
        currentY -= mouseDelta.y * effectiveSensitivity * Time.deltaTime;

        // Limitar el ángulo vertical
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        // Calcular la dirección y la posición de la cámara
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.back * distance;
        transform.position = target.position + direction + offset;

        // La cámara siempre mira hacia el objetivo
        transform.LookAt(target.position + offset);

        // Hacer que el personaje mire hacia la dirección en que gira la cámara (eje horizontal)
        if (character != null)
        {
            Vector3 lookDirection = new Vector3(transform.forward.x, 0, transform.forward.z); // Evitar inclinación en el eje Y
            if (lookDirection.magnitude > 0.1f) // Evitar rotación por pequeños movimientos
            {
                character.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}
