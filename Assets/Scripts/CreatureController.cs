using System;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    private bool isGrounded = true;

    private Vector3 relativeBottom;
    private Vector3 bottom;
    private Vector3 groundCheckMargin = new Vector3(0f, 0.01f, 0f);
    private Vector3 direction;
    private Vector3 velocity;

    private Rigidbody creatureRigidBody;

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
        creatureRigidBody = GetComponent<Rigidbody>();
        relativeBottom = GetRelativeBottomPoint();
    }

    private void Update()
    {
        GroundCheck();

        if (isGrounded)
        {
            var targetVelocity = direction * characterConstants.GroundSpeedMax;
            var t = Time.deltaTime;
            t /= direction.magnitude > 0 ? characterConstants.GroundAccelerationTime : characterConstants.GroundDecelerationTime;
            velocity = Vector3.Lerp(velocity, targetVelocity, t);
        }

        creatureRigidBody.velocity = velocity;
    }

    private Vector3 GetRelativeBottomPoint()
    {
        var filter = GetComponent<MeshFilter>();

        if (filter == null || filter.mesh == null)
            return Vector3.zero;

        var centerY = filter.mesh.bounds.center.y;
        var relativeY = centerY - filter.mesh.bounds.extents.y;

        return new Vector3(0, relativeY, 0);
    }

    private void GroundCheck()
    {
        isGrounded = false;
        bottom = transform.position + relativeBottom;

        Ray ray = new Ray(bottom + groundCheckMargin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            isGrounded = hit.distance < characterConstants.GroundCheckDistance;
            Debug.DrawRay(bottom, Vector3.down, Color.green);
            return;
        }

        Debug.DrawRay(bottom, Vector3.down, Color.red);
    }

    public void SetRotation(Quaternion newRotation)
    {
        transform.rotation = newRotation;
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public void SetInputDirection(Vector3 inputDirection)
    {
        direction = transform.TransformDirection(inputDirection);
    }

    public Vector3 GetVelocity()
    {
        return creatureRigidBody.velocity;
    }

    public LookAt HostTakeOver()
    {
        tag = "Player";
        gameObject.layer = 8;
        return GetComponentInChildren<LookAt>();
    }

    public void HostRelease()
    {
        tag = "Creature";
        gameObject.layer = 0;
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
