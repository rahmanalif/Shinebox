using UnityEngine;

public class BoostPad : MonoBehaviour
{
    [Header("Boost Settings")]
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private Vector2 boostDirection = Vector2.right;
    [SerializeField] private bool useDirectionalBoost = false;
    [SerializeField] private float fixedBoostForce = 15f;

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem boostEffect;
    [SerializeField] private AudioClip boostSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && boostSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ApplyBoost(ballRb);
            }
        }
    }

    private void ApplyBoost(Rigidbody2D ballRb)
    {
        if (useDirectionalBoost)
        {
            // Apply boost in specific direction
            Vector2 normalizedDirection = boostDirection.normalized;
            ballRb.linearVelocity = normalizedDirection * fixedBoostForce;
        }
        else
        {
            // Multiply current velocity
            ballRb.linearVelocity *= boostMultiplier;
        }

        // Play visual effect
        if (boostEffect != null)
        {
            boostEffect.Play();
        }

        // Play sound
        if (audioSource != null && boostSound != null)
        {
            audioSource.PlayOneShot(boostSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw boost direction arrow in editor
        if (useDirectionalBoost)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position;
            Vector3 direction = boostDirection.normalized;
            Gizmos.DrawRay(start, direction * 2f);
            Gizmos.DrawSphere(start + direction * 2f, 0.2f);
        }
    }
}
