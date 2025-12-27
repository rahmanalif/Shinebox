using UnityEngine;
using UnityEngine.InputSystem;

public class golfScript : MonoBehaviour
{
    // REFERENCES
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer lr;

    // ATTRIBUTES
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float power = 2f;
    [SerializeField] private float velocityDamping = 0.98f;
    [SerializeField] private float stopThreshold = 0.2f;

    // PRIVATE VARIABLES
    private bool isDragging;
    private Mouse mouse;
    private Touchscreen touchscreen;
    private Camera mainCamera;
    private int strokeCount = 0;
    private bool canShoot = true;

    void Start()
    {
        mouse = Mouse.current;
        touchscreen = Touchscreen.current;
        mainCamera = Camera.main;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerInput();
    }

    void FixedUpdate()
    {
        // Apply damping to slow down the ball
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            rb.linearVelocity *= velocityDamping;
            rb.angularVelocity *= velocityDamping;
        }

        // Stop the ball completely when it's moving very slowly
        if (rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Prevent ball from sticking to walls at low speeds
        if (rb.linearVelocity.magnitude < 1f)
        {
            // Add a small bounce force away from the wall
            Vector2 bounceDirection = collision.contacts[0].normal;
            rb.AddForce(bounceDirection * 0.5f, ForceMode2D.Impulse);
        }
    }

    private void PlayerInput()
    {
        if (!IsReady())
        {
            return;
        }

        Vector2 inputPosition = GetInputPosition();
        if (inputPosition == Vector2.zero) return;

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(inputPosition);
        float distance = Vector2.Distance(transform.position, worldPos);

        if (IsInputDown() && distance <= 0.5f)
        {
            DragStart(worldPos);
        }
        else if (IsInputHeld() && isDragging)
        {
            DragChange(worldPos);
        }
        else if (IsInputUp() && isDragging)
        {
            DragRelease(worldPos);
        }
    }

    private Vector2 GetInputPosition()
    {
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            return touchscreen.primaryTouch.position.ReadValue();
        }
        else if (touchscreen != null && touchscreen.primaryTouch.press.wasReleasedThisFrame)
        {
            return touchscreen.primaryTouch.position.ReadValue();
        }
        else if (mouse != null)
        {
            return mouse.position.ReadValue();
        }
        return Vector2.zero;
    }

    private bool IsInputDown()
    {
        if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
            return true;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            return true;
        return false;
    }

    private bool IsInputHeld()
    {
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
            return true;
        if (mouse != null && mouse.leftButton.isPressed)
            return true;
        return false;
    }

    private bool IsInputUp()
    {
        if (touchscreen != null && touchscreen.primaryTouch.press.wasReleasedThisFrame)
            return true;
        if (mouse != null && mouse.leftButton.wasReleasedThisFrame)
            return true;
        return false;
    }

    private void DragStart(Vector2 position)
    {
        isDragging = true;
        lr.positionCount = 2;
    }

    private void DragChange(Vector2 position)
    {
        Vector2 direction = (Vector2)transform.position - position;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, (Vector2)transform.position + Vector2.ClampMagnitude(direction * power, maxPower) / 2);
    }

    private void DragRelease(Vector2 inputPosition)
    {
        isDragging = false;
        lr.positionCount = 0;

        float distance = Vector2.Distance((Vector2)transform.position, inputPosition);

        if (distance < 0.5f)
        {
            return;
        }

        Vector2 direction = (Vector2)transform.position - inputPosition;
        Vector2 force = Vector2.ClampMagnitude(direction * power, maxPower);
        rb.AddForce(force, ForceMode2D.Impulse);

        // Increment stroke count
        strokeCount++;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordStroke();
        }
    }

    public void DisableShooting()
    {
        canShoot = false;
    }

    private bool IsReady()
    {
        return rb.linearVelocity.magnitude < 0.2f && canShoot;
    }

    public int GetStrokeCount()
    {
        return strokeCount;
    }
}
