using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerInputManager : MonoBehaviour
{
    public CinemachineFreeLook playerCamera;

    public CONTROL_MODE CurrentControlMode = CONTROL_MODE.KEYBOARDMOUSE;

    private Controls controls;

    private PlayerCharacterController player;
    
    private void Awake()
    {
        player = GetComponent<PlayerCharacterController>();

        controls = new Controls();
        controls.Player.PrimaryFire.performed += PrimaryFire_performed;

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
        return move.normalized;
    }

    private void PrimaryFire_performed(InputAction.CallbackContext obj)
    {
        player.PrimaryAction();
    }


    public enum CONTROL_MODE
    {
        KEYBOARDMOUSE,
        GAMEPAD
    }
}
