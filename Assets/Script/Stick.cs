using UnityEngine;

public class Stick : MonoBehaviour
{
    public Transform pickupPointA;
    public Transform pickupPointB;

    private bool isCollidingNoMove = false;

    public bool IsCollidingNoMove => isCollidingNoMove;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Block_noMove"))
        {
            isCollidingNoMove = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Block_noMove"))
        {
            isCollidingNoMove = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pickupPointA != null)
            Gizmos.DrawWireSphere(pickupPointA.position, 0.1f);
        if (pickupPointB != null)
            Gizmos.DrawWireSphere(pickupPointB.position, 0.1f);
    }
}
