using UnityEngine;

public class Stick : MonoBehaviour
{
    public Transform pickupPointA;
    public Transform pickupPointB;

    [SerializeField] float detectRadius = 0.15f;
    [SerializeField] float detectDistance = 0.3f;
    [SerializeField] LayerMask blockNoMoveLayer;
    [SerializeField] private Transform mapTransform;
    public bool IsCollidingNoMove { get; private set; }

    private void Update()
    {
        IsCollidingNoMove = CheckHit(pickupPointA) || CheckHit(pickupPointB);
    }

    bool CheckHit(Transform point)
    {
        if (point == null) return false;
        return Physics.CheckSphere(point.position + point.forward * detectDistance, detectRadius, blockNoMoveLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pickupPointA != null)
            Gizmos.DrawWireSphere(pickupPointA.position + pickupPointA.forward * detectDistance, detectRadius);
        if (pickupPointB != null)
            Gizmos.DrawWireSphere(pickupPointB.position + pickupPointB.forward * detectDistance, detectRadius);
    }
}
