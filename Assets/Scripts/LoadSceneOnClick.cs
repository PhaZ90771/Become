using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public string SceneName;

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();

        controls.Player.PrimaryFire.performed += PrimaryFire_performed;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void PrimaryFire_performed(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(SceneName);
    }
}
