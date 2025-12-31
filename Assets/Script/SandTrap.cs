using UnityEngine;

public class SandTrap : MonoBehaviour
{
    [Header("Sand Settings")]
    [SerializeField] private float frictionMultiplier = 0.5f;
    [SerializeField] private float slowDownRate = 0.9f;
    [SerializeField] private float minimumVelocity = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private Color sandColor = new Color(0.96f, 0.87f, 0.70f);
    [SerializeField] private ParticleSystem sandEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            // Play sand effect when ball enters
            if (sandEffect != null)
            {
                sandEffect.Play();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ApplySandFriction(ballRb);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            // Stop sand effect when ball exits
            if (sandEffect != null)
            {
                sandEffect.Stop();
            }
        }
    }

    private void ApplySandFriction(Rigidbody2D ballRb)
    {
        // Slow down the ball while in sand
        ballRb.linearVelocity *= slowDownRate;
        ballRb.angularVelocity *= slowDownRate;

        // Stop completely if too slow
        if (ballRb.linearVelocity.magnitude < minimumVelocity)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }
}
