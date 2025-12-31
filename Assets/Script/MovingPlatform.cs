using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector2 pointA;
    [SerializeField] private Vector2 pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool useLocalSpace = true;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Wait Settings")]
    [SerializeField] private float waitTimeAtPoints = 0.5f;

    [Header("Movement Type")]
    [SerializeField] private MovementType movementType = MovementType.PingPong;

    public enum MovementType
    {
        PingPong,
        Loop,
        Once
    }

    private Vector2 currentTarget;
    private Vector2 startPosition;
    private float journeyLength;
    private float startTime;
    private bool movingToB = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool hasCompletedOnce = false;

    private void Start()
    {
        startPosition = useLocalSpace ? transform.localPosition : (Vector2)transform.position;
        currentTarget = pointB;
        journeyLength = Vector2.Distance(pointA, pointB);
        startTime = Time.time;
    }

    private void Update()
    {
        if (movementType == MovementType.Once && hasCompletedOnce)
        {
            return;
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoints)
            {
                isWaiting = false;
                waitTimer = 0f;
                startTime = Time.time;
            }
            return;
        }

        MovePlatform();
    }

    private void MovePlatform()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        // Apply animation curve
        float curvedFraction = movementCurve.Evaluate(fractionOfJourney);

        Vector2 start = movingToB ? pointA : pointB;
        Vector2 end = movingToB ? pointB : pointA;

        Vector2 newPosition = Vector2.Lerp(start, end, curvedFraction);

        if (useLocalSpace)
        {
            transform.localPosition = newPosition;
        }
        else
        {
            transform.position = newPosition;
        }

        // Check if reached target
        if (fractionOfJourney >= 1f)
        {
            OnReachTarget();
        }
    }

    private void OnReachTarget()
    {
        if (movementType == MovementType.Once)
        {
            hasCompletedOnce = true;
            return;
        }

        isWaiting = true;

        if (movementType == MovementType.PingPong)
        {
            movingToB = !movingToB;
        }
        else if (movementType == MovementType.Loop)
        {
            // Reset to start position
            if (useLocalSpace)
            {
                transform.localPosition = pointA;
            }
            else
            {
                transform.position = pointA;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw movement path in editor
        Vector2 worldPointA = useLocalSpace ? (Vector2)transform.parent.TransformPoint(pointA) : pointA;
        Vector2 worldPointB = useLocalSpace ? (Vector2)transform.parent.TransformPoint(pointB) : pointB;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(worldPointA, worldPointB);
        Gizmos.DrawWireSphere(worldPointA, 0.3f);
        Gizmos.DrawWireSphere(worldPointB, 0.3f);

        // Draw platform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Make ball child of platform so it moves with it
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Unparent ball when it leaves platform
            collision.transform.SetParent(null);
        }
    }
}
