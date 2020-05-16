using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerCharacterController : MonoBehaviour
{
    private CharacterController hostCharacterController;
    private PlayerInputManager playerInputManager;

    private Transform host;

    private bool isGrounded = true;

    [SerializeField]
    private Vector3 Velocity;

    [SerializeField]
    private CharacterControlConstants characterConstants = new CharacterControlConstants
    {
        GravityDownForce = 20f,
        GroundAccelerationTime = 3f,
        GroundDecelerationTime = 0.25f,
        GroundCheckDistance = 0.1f,
        GroundSpeedMax = 5f,
    };

    private void Awake()
    {
        host = transform.parent;

        hostCharacterController = host.GetComponent<CharacterController>();
        playerInputManager = GetComponent<PlayerInputManager>();

        hostCharacterController.enableOverlapRecovery = true;
    }

    private void Update()
    {
        GroundCheck();

        var rotation = Vector3.zero;
        rotation.y = playerInputManager.GetCameraRotation();
        host.rotation = Quaternion.Euler(rotation);

        var targetVelocity = host.TransformDirection(playerInputManager.GetMovementInput());

        if (isGrounded)
        {
            targetVelocity *= characterConstants.GroundSpeedMax;

            var t = targetVelocity.magnitude > 0 ? Time.deltaTime / characterConstants.GroundAccelerationTime : Time.deltaTime / characterConstants.GroundDecelerationTime;
            Velocity = Vector3.Lerp(Velocity, targetVelocity, t);
        }
        else
        {
            Velocity += Vector3.down * characterConstants.GravityDownForce * Time.deltaTime;
        }
        hostCharacterController.Move(Velocity * Time.deltaTime);
    }

    private void GroundCheck()
    {
        //isGrounded = false;

        //var start = host.position + Vector3.down * hostCharacterController.height / 2f;
        //Ray ray = new Ray(start, Vector3.down);

        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    isGrounded = hit.distance < characterConstants.GroundCheckDistance;
        //    Debug.DrawRay(start, Vector3.down, Color.green);
        //    return;
        //}

        //Debug.DrawRay(start, Vector3.down, Color.green);
    }

    [Serializable]
    struct CharacterControlConstants
    {
        public float GravityDownForce;
        public float GroundAccelerationTime;
        public float GroundDecelerationTime;
        public float GroundCheckDistance;
        public float GroundSpeedMax;
    }
}
