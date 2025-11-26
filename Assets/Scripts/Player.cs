using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public event System.Action OnReachFinish;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput input;

    private Rigidbody rb;
    private float playerHeight = 2f;
    private float playerRadius = .7f;
    private Vector3 inputVector;
    private Vector3 moveDir;
    private bool disabled;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    private void Update()
    {
        inputVector = Vector3.zero;

        if (!disabled)
        {
            inputVector = input.GetMovementVectorNormalized();
        }
        else
        {
            moveDir = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        moveDir = inputVector * moveSpeed * Time.deltaTime;
        HandleMovement();

        if (moveDir != Vector3.zero)
        {
            rb.MovePosition(rb.position + moveDir);
            Quaternion targetRotation = Quaternion.LookRotation(inputVector);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, moveSpeed * Time.fixedDeltaTime));
        }
    }

    private void HandleMovement()
    {
        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(
            rb.position,
            rb.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance
        );

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f);
            canMove = !Physics.CapsuleCast(
                rb.position,
                rb.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX,
                moveDistance
            );

            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z);
                canMove = !Physics.CapsuleCast(
                    rb.position,
                    rb.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance
                );

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
                else
                {
                    moveDir = Vector3.zero;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Finish"))
        {
            Disable();
            OnReachFinish?.Invoke();
        }
    }

    private void Disable()
    {
        disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}
