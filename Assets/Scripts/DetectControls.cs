using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectControls : MonoBehaviour
{
    private PlayerInput playerInput;
    private Inputs inputs;

    public PlayerDevice device;

    public enum PlayerDevice
    {
        Gamepad,
        Keyboard
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public Inputs GetInputs()
    {
        return inputs;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inputs = new Inputs();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();   
    }

    public event Action<PlayerDevice> DeviceChanged;

    public void SetPlayerDevice(PlayerInput playerInput)
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard":
                device = PlayerDevice.Keyboard;
                Debug.Log("Teclado");
                DeviceChanged?.Invoke(device);
                break;
            case "Gamepad":
                device = PlayerDevice.Gamepad;
                Debug.Log("Control");
                DeviceChanged?.Invoke(device);
                break;
        }
    }
}
