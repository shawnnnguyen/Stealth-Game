using System.Collections;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardHasSpottedPlayer;
    [SerializeField] private Transform pathHolder;
    [SerializeField] private Light spotlight;
    
    [SerializeField] private float movingSpeed;
    [SerializeField] private float waitTime;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float viewDistance;
    
    private Vector3[] _wayPoint;
    private float _viewAngle;
    private Transform _player;
    private float _timeToSpotPlayer = .2f;
    private float _playerVisibleTimer;
    private Color _originalSpotlightColor = Color.yellow;
    private Color _detectedSpotlightColor =  Color.red;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = pathHolder.GetChild(0).position;
      
        
        transform.forward = pathHolder.GetChild(1).forward;
        _viewAngle = spotlight.spotAngle;
        spotlight.color = _originalSpotlightColor;
        
        _wayPoint = new Vector3[pathHolder.childCount];
        
        for (int i = 0; i < _wayPoint.Length; i++)
        {
            _wayPoint[i] = pathHolder.GetChild(i).position;
            _wayPoint[i].y = 0.5f;
        }
    
        StartCoroutine(GuardPath(_wayPoint));
    }

    private void Update()
    {
        if (CanSeePlayer(_player.position))
        {
            _playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            _playerVisibleTimer -= Time.deltaTime;
        }
        
        _playerVisibleTimer = Mathf.Clamp(_playerVisibleTimer, 0, _timeToSpotPlayer);
        spotlight.color = Color.Lerp(_originalSpotlightColor, _detectedSpotlightColor, _playerVisibleTimer / _timeToSpotPlayer);

        if (_playerVisibleTimer >= _timeToSpotPlayer)
        {
            OnGuardHasSpottedPlayer?.Invoke();
        }
    }

    private bool CanSeePlayer(Vector3 playerPos)
    {
        Vector3 directionToTarget = (playerPos - spotlight.transform.position).normalized;
        float angle = Vector3.Angle(directionToTarget, spotlight.transform.forward);
        int obstacleLayer = LayerMask.GetMask("Obstacle");
        bool notBlockedByObstacle =
            !Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit,viewDistance, obstacleLayer);
        
        if (Vector3.Distance(_player.position, transform.position) < viewDistance)
        {
            if (angle <= _viewAngle / 2 &&  notBlockedByObstacle)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private IEnumerator GuardPath(Vector3[] wayPoints)
    {
        int currentWayPointIndex = 1;
        Vector3 targetPos = wayPoints[currentWayPointIndex];

        while (true)
        {
            transform.position = 
                Vector3.MoveTowards(transform.position, targetPos, movingSpeed * Time.deltaTime);
            
            if (transform.position == targetPos)
            {
                yield return new WaitForSeconds(waitTime);
                
                currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Length;
                targetPos = wayPoints[currentWayPointIndex];
                
                Vector3 movingDir = targetPos - transform.position;
                    
                if (movingDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movingDir);
                    
                    while (targetRotation != transform.rotation)
                    { 
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
                        yield return null;
                    }
                }
            }
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPos = pathHolder.GetChild(0).position;
        Vector3 prevPos = startPos;
        
        foreach (Transform wayPoint in pathHolder)
        {
            Gizmos.DrawSphere(wayPoint.position, 0.5f);
            Gizmos.DrawLine(prevPos, wayPoint.position);
            prevPos = wayPoint.position;
        }
        Gizmos.DrawLine(prevPos, startPos);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}