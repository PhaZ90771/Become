using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerCharacterController : MonoBehaviour
{
    static readonly int layerMask = ~(1 << 9);

    private PlayerInputManager playerInputManager;

    private CreatureController host;

    private Transform target;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        SetHost(transform.parent);
    }

    private void Update()
    {
        TargetCheck();

        var rotation = Vector3.zero;
        rotation.y = playerInputManager.GetCameraRotation();
        host.SetRotation(Quaternion.Euler(rotation));

        host.SetInputDirection(playerInputManager.GetMovementInput());
    }

    private void TargetCheck()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
        if (hasHit)
        {
            target = hit.transform;
        }
        else
        {
            target = null;
        }
    }

    private bool SetHost(Transform newHost)
    {
        if (newHost.CompareTag("Creature"))
        {
            if (host != null) host.HostRelease();
            host = newHost.GetComponent<CreatureController>();
            playerInputManager.playerCamera.LookAt = host.HostTakeOver().transform;

            transform.SetParent(host.transform, worldPositionStays: false);

            playerInputManager.playerCamera.m_XAxis.Value = host.GetRotation().eulerAngles.y;

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
}
