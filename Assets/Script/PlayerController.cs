using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace DemoKitStylizedAnimatedDogs
{
    public class GridPlayerController : MonoBehaviour
    {
        [SerializeField] float moveDistance = 1f;
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float rotateSpeed = 360f;
        [SerializeField] Animator animator;
        [SerializeField] DogMouthPickup pickupDetector;
        [SerializeField] LevelManager levelManager;




        bool isMoving, isRotating, isInteracting;
        Vector3 moveStartPos;
        Quaternion rotateStartRot;
        

        private void Start()
        {
            if (levelManager == null)
                levelManager = FindFirstObjectByType<LevelManager>();
        }

        void Update()
        {
            if (isMoving || isRotating || isInteracting || isWinning) return;

            if (Keyboard.current.wKey.wasPressedThisFrame) StartMove(Vector3.forward);
            if (Keyboard.current.sKey.wasPressedThisFrame) StartMove(Vector3.back);
            if (Keyboard.current.dKey.wasPressedThisFrame) StartMove(Vector3.right);
            if (Keyboard.current.aKey.wasPressedThisFrame) StartMove(Vector3.left);

            if (Keyboard.current.qKey.wasPressedThisFrame) StartRotate(90f);
            if (Keyboard.current.eKey.wasPressedThisFrame) StartRotate(-90f);

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (isInteracting) return;
                if (pickupDetector.IsHolding)
                    StartCoroutine(PlayDropAnimationThenRelease());

                else
                    StartCoroutine(HandleRotateAndPickup());

            }

            if (pickupDetector.IsHolding && CheckBlockWinAtPosition(transform.position))
            {
                StartCoroutine(PlayWinAnimationAndLoadNextLevel());
            }
        }

        void StartMove(Vector3 dir)
        {
            moveStartPos = transform.position;
            Vector3 target = transform.position + dir * moveDistance;
            if (IsBlockNormalAt(target)) StartCoroutine(MoveStep(target));
        }

        void StartRotate(float angle)
        {
            rotateStartRot = transform.rotation;
            StartCoroutine(RotateByAngle(angle));
        }

        bool StickHitsNoMove()
        {
            if (!pickupDetector.IsHolding) return false;
            var stick = pickupDetector.GetHeldStickTransform();
            return stick != null && stick.GetComponent<Stick>().IsCollidingNoMove;
        }

        IEnumerator MoveStep(Vector3 target)
        {
            isMoving = true;
            animator.SetInteger("AnimationID", 3);

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                if (StickHitsNoMove())
                {
                    StartCoroutine(MoveBackTo(moveStartPos));
                    yield break;
                }

                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            animator.SetInteger("AnimationID", 0);
            isMoving = false;
        }

        IEnumerator MoveBackTo(Vector3 backPosition)
        {
            animator.SetInteger("AnimationID", 3);
            while (Vector3.Distance(transform.position, backPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, backPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = backPosition;
            animator.SetInteger("AnimationID", 0);
            isMoving = false;
        }

        IEnumerator RotateByAngle(float angle)
        {
            isRotating = true;
            animator.SetInteger("AnimationID", 4);

            Quaternion start = transform.rotation;
            Quaternion target = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
            float t = 0f;

            while (Quaternion.Angle(transform.rotation, target) > 0.5f)
            {
                if (StickHitsNoMove())
                {
                    StartCoroutine(RotateBackTo(rotateStartRot));
                    yield break;
                }

                t += Time.deltaTime * rotateSpeed / 90f;
                transform.rotation = Quaternion.Slerp(start, target, t);
                yield return null;
            }

            transform.rotation = target;
            animator.SetInteger("AnimationID", 0);
            isRotating = false;
        }

        IEnumerator RotateBackTo(Quaternion backRotation)
        {
            animator.SetInteger("AnimationID", 4);
            Quaternion current = transform.rotation;
            float t = 0f;

            while (Quaternion.Angle(transform.rotation, backRotation) > 0.5f)
            {
                t += Time.deltaTime * rotateSpeed / 90f;
                transform.rotation = Quaternion.Slerp(current, backRotation, t);
                yield return null;
            }

            transform.rotation = backRotation;
            animator.SetInteger("AnimationID", 0);
            isRotating = false;
        }


        bool IsBlockNormalAt(Vector3 pos)
        {
            foreach (var c in Physics.OverlapSphere(pos, 0.1f))
                if (c.CompareTag("Block_Normal") || c.CompareTag("Block_Win")) return true;
            return false;
        }

        float GetAngleToTargetRounded90(Vector3 p)
        {
            Vector3 dir = p - transform.position;
            dir.y = 0; dir.Normalize();
            if (dir == Vector3.zero) return 0;
            return Mathf.Round(Vector3.SignedAngle(transform.forward, dir, Vector3.up) / 90f) * 90f;
        }

        IEnumerator HandleRotateAndPickup()
        {
            if (pickupDetector.CurrentPickupPoint == null) yield break;
            isInteracting = true;
            float angle = GetAngleToTargetRounded90(pickupDetector.CurrentPickupPoint.position);
            if (Mathf.Abs(angle) > 1f)
                yield return StartCoroutine(RotateByAngle(angle));

            animator.SetInteger("AnimationID", 5);
            yield return new WaitForSeconds(0.2f);
            pickupDetector.TryPickup();
            animator.SetInteger("AnimationID", 0);
            yield return new WaitForSeconds(1.8f);
            isInteracting = false;
        }

        IEnumerator PlayDropAnimationThenRelease()
        {
            isInteracting = true;
            animator.SetInteger("AnimationID", 5);
            yield return new WaitForSeconds(0.2f);
            pickupDetector.TryDrop();
            animator.SetInteger("AnimationID", 0);
            yield return new WaitForSeconds(1.8f);
            isInteracting = false;
        }
        bool CheckBlockWinAtPosition(Vector3 pos)
        {
            Collider[] hits = Physics.OverlapSphere(pos, 0.3f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Block_Win"))
                    return true;
            }
            return false;
        }
        private bool isWinning;
        IEnumerator PlayWinAnimationAndLoadNextLevel()
        {

            isWinning = true;
            animator.SetInteger("AnimationID", 6);
            yield return new WaitForSeconds(3f); 
            animator.SetInteger("AnimationID", 0);
            levelManager.NextLevel();
            isWinning = false;
        }


    }

}
