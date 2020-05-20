using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerCharacterController : MonoBehaviour
{
    private PlayerInputManager playerInputManager;

    private Transform host;
    private Rigidbody hostRigidbody;

    private bool isGrounded = true;
    private Transform target;
    private Vector3 relativeBottom;
    private Vector3 bottom;

    private Vector3 groundCheckMargin = new Vector3(0f, 0.01f, 0f);

    [SerializeField]
    private Vector3 Velocity;

    [SerializeField]
    private CharacterControlConstants characterConstants = new CharacterControlConstants
    {
        GroundAccelerationTime = 3f,
        GroundDecelerationTime = 0.25f,
        GroundCheckDistance = 0.1f,
        GroundSpeedMax = 5f,
    };

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        SetHost(transform.parent);
    }

    private void Update()
    {
        GroundCheck();
        TargetCheck();

        var rotation = Vector3.zero;
        rotation.y = playerInputManager.GetCameraRotation();
        host.rotation = Quaternion.Euler(rotation);

        var direction = host.TransformDirection(playerInputManager.GetMovementInput());

        if (isGrounded)
        {
            var targetVelocity = direction * characterConstants.GroundSpeedMax;
            var t = Time.deltaTime;
            t /= direction.magnitude > 0 ? characterConstants.GroundAccelerationTime : characterConstants.GroundDecelerationTime;
            Velocity = Vector3.Lerp(Velocity, targetVelocity, t);
        }

        hostRigidbody.velocity = Velocity;
    }

    private void GroundCheck()
    {
        isGrounded = false;
        bottom = host.position + relativeBottom;

        Ray ray = new Ray(bottom + groundCheckMargin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            isGrounded = hit.distance < characterConstants.GroundCheckDistance;
            Debug.DrawRay(bottom, Vector3.down, Color.green);
            return;
        }

        Debug.DrawRay(bottom, Vector3.down, Color.red);
    }

    private void TargetCheck()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity);
        if (hasHit)
        {
            target = hit.transform;
        }
        else
        {
            target = null;
        }
    }

    private Vector3 GetRelativeBottomPoint()
    {
        var filter = host.GetComponent<MeshFilter>();

        if (filter == null || filter.mesh == null)
            return Vector3.zero;

        var centerY = filter.mesh.bounds.center.y;
        var relativeY = centerY - filter.mesh.bounds.extents.y;

        return new Vector3(0, relativeY, 0);
    }

    private bool SetHost(Transform newHost)
    {
        if (newHost.tag.Equals("Creature"))
        {
            var oldHost = host;
            host = newHost;

            if (oldHost != null)
            {
                oldHost.tag = "Creature";
                oldHost.gameObject.layer = 0;
            }

            host.tag = "Player";
            host.gameObject.layer = 8;

            hostRigidbody = host.GetComponent<Rigidbody>();
            relativeBottom = GetRelativeBottomPoint();

            transform.SetParent(host, worldPositionStays: false);

            playerInputManager.playerCamera.m_XAxis.Value = host.rotation.eulerAngles.y;
            Velocity = hostRigidbody.velocity;

            return true;
        }
        return false;
    }

    internal void PrimaryAction()
    {
        if (target != null)
        {
            SetHost(target);
        }
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Debug.DrawLine(Camera.main.transform.position, target.position, Color.green);
        }
        else
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward);
        }
    }

    [Serializable]
    struct CharacterControlConstants
    {
        public float GroundAccelerationTime;
        public float GroundDecelerationTime;
        public float GroundCheckDistance;
        public float GroundSpeedMax;
    }
}
