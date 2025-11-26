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
    
    private Vector3[] wayPoint;
    private float viewAngle;
    private Transform player;
    private float timeToSpotPlayer = .2f;
    private float playerVisibleTimer;
    private Color originalSpotlightColor = Color.yellow;
    private Color detectedSpotlightColor =  Color.red;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = pathHolder.GetChild(0).position;
        transform.forward = pathHolder.GetChild(1).forward;
        viewAngle = spotlight.spotAngle;
        spotlight.color = originalSpotlightColor;
        wayPoint = new Vector3[pathHolder.childCount];
        
        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPoint[i] = pathHolder.GetChild(i).position;
        }
    
        StartCoroutine(guardPath(wayPoint));
    }

    private void Update()
    {
        if (canSeePlayer(player.position))
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, detectedSpotlightColor, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            OnGuardHasSpottedPlayer?.Invoke();
        }
    }

    private bool canSeePlayer(Vector3 playerPos)
    {
        Vector3 directionToTarget = (playerPos - spotlight.transform.position).normalized;
        float angle = Vector3.Angle(directionToTarget, spotlight.transform.forward);
        int obstacleLayer = LayerMask.GetMask("Obstacle");
        bool notBlockedByObstacle =
            !Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit,viewDistance, obstacleLayer);
        
        if (Vector3.Distance(player.position, transform.position) < viewDistance)
        {
            if (angle <= viewAngle / 2 &&  notBlockedByObstacle)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private IEnumerator guardPath(Vector3[] wayPoints)
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