using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace DemoKitStylizedAnimatedDogs
{
    public class GridPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveDistance = 1f;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 360f;
        [SerializeField] private Animator animator;

        private bool isMoving = false;
        private bool isRotating = false;

        private void Update()
        {
            if (isMoving || isRotating) return;

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                TryMove(Vector3.forward);
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                TryMove(Vector3.back);
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                TryMove(Vector3.right);
            }

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                TryMove(Vector3.left);
            }

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                StartCoroutine(RotateByAngle(90f));
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(RotateByAngle(-90f));
            }
        }

        private void TryMove(Vector3 direction)
        {
            Vector3 targetPosition = transform.position + direction * moveDistance;
            if (IsBlockNormalAt(targetPosition))
            {
                StartCoroutine(MoveStep(targetPosition));
            }
        }

        private bool IsBlockNormalAt(Vector3 position)
        {
            Collider[] hits = Physics.OverlapSphere(position, 0.1f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Block_Normal"))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator MoveStep(Vector3 targetPosition)
        {
            isMoving = true;
            animator.SetInteger("AnimationID", 4);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
            animator.SetInteger("AnimationID", 0);
            isMoving = false;
        }

        private IEnumerator RotateByAngle(float angle)
        {
            isRotating = true;
            animator.SetInteger("AnimationID", 3);

            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
            float t = 0f;

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
            {
                t += Time.deltaTime * rotateSpeed / 90f;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            transform.rotation = targetRotation;
            animator.SetInteger("AnimationID", 0);
            isRotating = false;
        }
    }
}
