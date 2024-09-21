using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    private Inputs inputs;

    public Transform target; // El objeto que la c�mara seguir� (el personaje)
    public Vector3 offset; // Desplazamiento desde el objetivo
    public float sensitivity = 5f; // Sensibilidad de rotaci�n
    public float gamepadSensitivityMultiplier = 2f; // Multiplicador para la sensibilidad del joystick
    public float distance = 5f; // Distancia de la c�mara al objetivo
    public float minYAngle = -20f; // �ngulo m�nimo en el eje Y (vertical)
    public float maxYAngle = 80f; // �ngulo m�ximo en el eje Y (vertical)
    public Transform character; // Referencia al personaje que debe rotar con la c�mara

    private float currentX = 0f; // Rotaci�n en el eje X (horizontal)
    private float currentY = 0f; // Rotaci�n en el eje Y (vertical)
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

        // Rotaci�n de la c�mara
        currentX += mouseDelta.x * effectiveSensitivity * Time.deltaTime;
        currentY -= mouseDelta.y * effectiveSensitivity * Time.deltaTime;

        // Limitar el �ngulo vertical
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        // Calcular la direcci�n y la posici�n de la c�mara
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.back * distance;
        transform.position = target.position + direction + offset;

        // La c�mara siempre mira hacia el objetivo
        transform.LookAt(target.position + offset);

        // Hacer que el personaje mire hacia la direcci�n en que gira la c�mara (eje horizontal)
        if (character != null)
        {
            Vector3 lookDirection = new Vector3(transform.forward.x, 0, transform.forward.z); // Evitar inclinaci�n en el eje Y
            if (lookDirection.magnitude > 0.1f) // Evitar rotaci�n por peque�os movimientos
            {
                character.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}
