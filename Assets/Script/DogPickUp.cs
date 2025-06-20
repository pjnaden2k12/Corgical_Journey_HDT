using UnityEngine;

public class DogMouthPickup : MonoBehaviour
{
    [SerializeField] private Transform mouthPosition;

    public Transform CurrentPickupPoint { get; private set; }

    private Transform heldStick = null;

    public bool IsHolding => heldStick != null;

    public Transform GetHeldStickTransform() => heldStick;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickupA") || other.CompareTag("PickupB"))
            CurrentPickupPoint = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == CurrentPickupPoint)
            CurrentPickupPoint = null;
    }

    public void TryPickup()
    {
        if (heldStick != null) return;
        if (CurrentPickupPoint == null) return;

        Transform stick = CurrentPickupPoint.parent;
        heldStick = stick;

        stick.SetParent(mouthPosition);
        stick.localRotation = Quaternion.Euler(0, 90, 0);
        Vector3 offset = mouthPosition.InverseTransformPoint(CurrentPickupPoint.position);
        stick.localPosition -= offset;
    }

    public void TryDrop()
    {
        if (heldStick == null) return;

        Vector3 rayOrigin = mouthPosition.position;
        Vector3 rayDirection = mouthPosition.forward;

        Transform stick = heldStick;
        heldStick.SetParent(null);
        heldStick = null;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, 1f))
        {
            if (hit.collider.CompareTag("Block_Normal"))
            {
                stick.position = hit.point;
                return;
            }
        }
    }
}
