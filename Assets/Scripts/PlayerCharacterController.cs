using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerCharacterController : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerInputManager playerInputManager;

    private Transform host;

    private bool isGrounded = true;

    private Vector3 Velocity;

    private void Awake()
    {
        host = transform.parent;

        characterController = host.GetComponent<CharacterController>();
        playerInputManager = GetComponent<PlayerInputManager>();

        characterController.enableOverlapRecovery = true;
    }

    private void Update()
    {
        //GroundCheck();

        var rotation = Vector3.zero;
        rotation.y = playerInputManager.GetCameraRotation();
        host.rotation = Quaternion.Euler(rotation);

        var movementInput = playerInputManager.GetMovementInput();
    }

    private void GroundCheck()
    {
        throw new NotImplementedException();
        //isGrounded = false;

        //var start = transform.position + Vector3.down * characterController.height / 2f;
        //Ray ray = new Ray(start, Vector3.down);

        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    isGrounded = hit.distance < characterConstants.GroundCheckDistance;
        //    Debug.DrawRay(start, Vector3.down, Color.green);
        //    return;
        //}

        //Debug.DrawRay(start, Vector3.down, Color.green);
    }
}
