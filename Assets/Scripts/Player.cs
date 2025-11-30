using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachFinish;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameInput input;

    private Rigidbody _rb;
    private float _playerHeight = 2f;
    private float _playerRadius = .7f;
    private Vector3 _inputVector;
    private Vector3 _moveDir;
    private bool _disabled;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    private void Update()
    {
        _inputVector = Vector3.zero;

        if (!_disabled)
        {
            _inputVector = input.GetMovementVectorNormalized();
        }
        else
        {
            _moveDir = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        _moveDir = _inputVector * moveSpeed * Time.deltaTime;
        HandleMovement();

        if (_moveDir != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + _moveDir);
            Quaternion targetRotation = Quaternion.LookRotation(_inputVector);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, moveSpeed * Time.fixedDeltaTime));
        }
    }

    private void HandleMovement()
    {
        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(
            _rb.position,
            _rb.position + Vector3.up * _playerHeight,
            _playerRadius,
            _moveDir,
            moveDistance
        );

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(_moveDir.x, 0f, 0f);
            canMove = !Physics.CapsuleCast(
                _rb.position,
                _rb.position + Vector3.up * _playerHeight,
                _playerRadius,
                moveDirX,
                moveDistance
            );

            if (canMove)
            {
                _moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0f, 0f, _moveDir.z);
                canMove = !Physics.CapsuleCast(
                    _rb.position,
                    _rb.position + Vector3.up * _playerHeight,
                    _playerRadius,
                    moveDirZ,
                    moveDistance
                );

                _moveDir = canMove ? moveDirZ : Vector3.zero;
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
        _disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}
