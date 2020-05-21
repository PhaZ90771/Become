using System;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    private bool isGrounded = true;

    private Vector3 relativeBottom;
    private Vector3 bottom;
    private Vector3 groundCheckMargin = new Vector3(0f, 0.01f, 0f);

    private Vector3 direction;
    private bool jumpAttempt = false;
    private int jumpsLeft;

    private Rigidbody creatureRigidBody;

    [SerializeField]
    private CharacterControlConstants characterConstants = new CharacterControlConstants
    {
        GroundCheckDistance = 0.1f,
        GroundSpeedMax = 5f,
        JumpVelocity = 10f,
        JumpInAir = false,
        JumpsInAir = 0,
    };

    private void Awake()
    {
        creatureRigidBody = GetComponent<Rigidbody>();
        relativeBottom = GetRelativeBottomPoint();
    }

    private void Update()
    {
        GroundCheck();

        var movement = new Vector3();
        if (isGrounded)
        {
            jumpsLeft = characterConstants.JumpsInAir;

            movement = direction * characterConstants.GroundSpeedMax;

            var groundSpeed = GetGroundSpeed();
            if (direction.magnitude == 0 && groundSpeed.magnitude > 0)
            {
                movement = -groundSpeed;
            }
        }

        if (jumpAttempt && CanJump())
        {
            if (!isGrounded && jumpsLeft > 0 && characterConstants.JumpsInAir > 0)
            {
                jumpsLeft -= 1;
            }
            
            movement.y = characterConstants.JumpVelocity;
        }
        jumpAttempt = false;
        
        creatureRigidBody.AddForce(movement);
        
        LimitGroundSpeed();
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

    private Vector3 GetGroundSpeed()
    {
        return new Vector3(creatureRigidBody.velocity.x, 0, creatureRigidBody.velocity.z);
    }

    private void LimitGroundSpeed()
    {
        var groundVelocity = GetGroundSpeed();
        if (groundVelocity.magnitude > characterConstants.GroundSpeedMax)
        {
            groundVelocity = groundVelocity.normalized * characterConstants.GroundSpeedMax;
        }
        creatureRigidBody.velocity = new Vector3(groundVelocity.x, creatureRigidBody.velocity.y, groundVelocity.z);
    }

    private bool CanJump()
    {
        if (characterConstants.JumpVelocity <= 0)
        {
            return false;
        }
        if (isGrounded)
        {
            return true;
        }
        if (characterConstants.JumpInAir)
        {
            if (characterConstants.JumpsInAir < 0) // Infinite jumps in air
            {
                return true;
            }
            if (jumpsLeft > 0)
            {
                return true;
            }
        }

        return false;
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

    public void StartJump()
    {
        jumpAttempt = true;
    }

    [Serializable]
    struct CharacterControlConstants
    {
        public float GroundCheckDistance;
        public float GroundSpeedMax;
        public float JumpVelocity;
        public bool JumpInAir;
        public int JumpsInAir;
    }
}
