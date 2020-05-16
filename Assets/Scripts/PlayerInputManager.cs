﻿using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerInputManager : MonoBehaviour
{
    public CinemachineFreeLook playerCamera;

    public CONTROL_MODE CurrentControlMode = CONTROL_MODE.KEYBOARDMOUSE;

    private Controls controls;
    
    private void Awake()
    {
        controls = new Controls();

        InputSystem.onActionChange += InputSystem_onActionChange;

        SetCursorState(false);
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnDestroy()
    {
        SetCursorState(true);
    }

    private void Update()
    {
        CinemachineCore.GetInputAxis = GetAxisName;
    }

    private float GetAxisName(string axisName)
    {
        var delta = controls.Player.Look.ReadValue<Vector2>();
        delta.Normalize();

        if (axisName.Equals("Mouse X"))
        {
            return delta.x;
        }
        else if (axisName.Equals("Mouse Y"))
        {
            return delta.y;
        }
        return 0;
    }

    private void InputSystem_onActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            var action = (InputAction)obj;
            var deviceName = action.activeControl.device.displayName;

            switch (deviceName)
            {
                case "Mouse":
                CurrentControlMode = CONTROL_MODE.KEYBOARDMOUSE;
                break;
                case "Keyboard":
                CurrentControlMode = CONTROL_MODE.KEYBOARDMOUSE;
                break;
            }
        }
    }
    private void SetCursorState(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public float GetCameraRotation()
    {
        return playerCamera.m_XAxis.Value;
    }

    public Vector3 GetMovementInput()
    {
        Vector2 raw = controls.Player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(raw.x, 0, raw.y);
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }

    public enum CONTROL_MODE
    {
        KEYBOARDMOUSE,
        GAMEPAD
    }
}
