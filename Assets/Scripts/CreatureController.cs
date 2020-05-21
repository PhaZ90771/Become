using Boo.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    [SerializeField]
    private HashSet<int> isGroundedOn = new HashSet<int>();

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
    }

    private void Update()
    {
        var movement = new Vector3();
        if (isGroundedOn.Any())
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
            if (!isGroundedOn.Any() && jumpsLeft > 0 && characterConstants.JumpsInAir > 0)
            {
                jumpsLeft -= 1;
            }
            
            movement.y = characterConstants.JumpVelocity;
        }
        jumpAttempt = false;
        
        creatureRigidBody.AddForce(movement);
        
        LimitGroundSpeed();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var id = collision.gameObject.GetInstanceID();
        foreach (ContactPoint cp in collision.contacts)
        {
            if (cp.normal.y > 0.5)
            {
                isGroundedOn.Add(id);
                return;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var id = collision.gameObject.GetInstanceID();
        foreach (ContactPoint cp in collision.contacts)
        {
            if (cp.normal.y > 0.5)
            {
                isGroundedOn.Add(id);
                return;
            }
        }
        isGroundedOn.Remove(collision.gameObject.GetInstanceID());
    }

    private void OnCollisionExit(Collision collision)
    {
        var id = collision.gameObject.GetInstanceID();
        isGroundedOn.Remove(id);
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
        if (isGroundedOn.Any())
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
