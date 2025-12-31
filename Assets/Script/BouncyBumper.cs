using UnityEngine;

public class BouncyBumper : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 15f;
    [SerializeField] private bool useReflection = true;
    [SerializeField] private float minimumImpactVelocity = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem bounceEffect;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private Animator bumperAnimator;
    [SerializeField] private string bounceAnimationTrigger = "Bounce";

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && bounceSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null && ballRb.linearVelocity.magnitude > minimumImpactVelocity)
            {
                ApplyBounce(ballRb, collision);
                PlayFeedback();
            }
        }
    }

    private void ApplyBounce(Rigidbody2D ballRb, Collision2D collision)
    {
        if (useReflection && collision.contactCount > 0)
        {
            // Reflect ball based on collision normal
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflection = Vector2.Reflect(ballRb.linearVelocity, normal);
            ballRb.linearVelocity = reflection.normalized * bounceForce;
        }
        else
        {
            // Simple bounce away from bumper
            Vector2 direction = (ballRb.transform.position - transform.position).normalized;
            ballRb.linearVelocity = direction * bounceForce;
        }
    }

    private void PlayFeedback()
    {
        // Play visual effect
        if (bounceEffect != null)
        {
            bounceEffect.Play();
        }

        // Play sound
        if (audioSource != null && bounceSound != null)
        {
            audioSource.PlayOneShot(bounceSound);
        }

        // Trigger animation
        if (bumperAnimator != null)
        {
            bumperAnimator.SetTrigger(bounceAnimationTrigger);
        }
    }
}
