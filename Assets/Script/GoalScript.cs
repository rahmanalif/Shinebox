using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMPro.TextMeshProUGUI resultText; // Title (e.g., "BIRDIE!")
    [SerializeField] private TMPro.TextMeshProUGUI subtitleText; // Subtitle (e.g., "Great Job!")
    [SerializeField] private TMPro.TextMeshProUGUI strokesText; // Shows strokes number
    [SerializeField] private TMPro.TextMeshProUGUI parText; // Shows par number

    [Header("Hole Animation Settings")]
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private float shrinkScale = 0.1f;
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private AudioClip holeSound;
    [SerializeField] private ParticleSystem holeParticleEffect;

    private bool goalReached = false;
    private AudioSource audioSource;
    private Vector3 originalBallScale;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && holeSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball") && !goalReached)
        {
            goalReached = true;
            StartCoroutine(BallFallIntoHole(collision.gameObject));
        }
    }

    private IEnumerator BallFallIntoHole(GameObject ball)
    {
        // Save the original ball scale before shrinking
        originalBallScale = ball.transform.localScale;

        // Play hole sound
        if (audioSource != null && holeSound != null)
        {
            audioSource.PlayOneShot(holeSound);
        }

        // Get ball components
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        golfScript golfScript = ball.GetComponent<golfScript>();

        // Disable ball shooting
        if (golfScript != null)
        {
            golfScript.DisableShooting();
        }

        // Stop ball movement and make it kinematic
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }

        // Animate ball falling into hole
        Vector3 startPosition = ball.transform.position;
        Vector3 holePosition = transform.position;
        Vector3 startScale = ball.transform.localScale;
        Vector3 endScale = startScale * shrinkScale;

        float elapsed = 0f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;

            // Ease-in curve for smooth fall
            float smoothT = t * t;

            // Move ball to center of hole
            ball.transform.position = Vector3.Lerp(startPosition, holePosition, smoothT);

            // Shrink ball as it falls in
            ball.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);

            yield return null;
        }

        // Ensure final position and scale
        ball.transform.position = holePosition;
        ball.transform.localScale = endScale;

        // Optional: Hide the ball completely
        SpriteRenderer ballSprite = ball.GetComponent<SpriteRenderer>();
        if (ballSprite != null)
        {
            ballSprite.enabled = false;
        }

        // Trigger particle effect when ball is fully in hole
        if (holeParticleEffect != null)
        {
            holeParticleEffect.Play();
        }

        // Small delay before showing win screen
        yield return new WaitForSeconds(0.3f);

        ShowWinScreen();
    }

    private void ShowWinScreen()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager not found!");
            return;
        }

        // Disable ball shooting
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            golfScript golfScript = ball.GetComponent<golfScript>();
            if (golfScript != null)
            {
                golfScript.DisableShooting();
            }
        }

        int strokes = GameManager.Instance.GetCurrentStrokes();
        int par = GameManager.Instance.GetPar();
        int difference = strokes - par;

        // Show win panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Update strokes and par display
        if (strokesText != null)
        {
            strokesText.text = strokes.ToString();
        }

        if (parText != null)
        {
            parText.text = par.ToString();
        }

        // Update result title and subtitle based on performance
        if (resultText != null)
        {
            if (strokes == 1)
            {
                resultText.text = "HOLE IN ONE!";
                resultText.color = new Color(0.25f, 0.82f, 0.54f); // Green color #3FD18A
                if (subtitleText != null) subtitleText.text = "Amazing!";
            }
            else if (difference <= -2)
            {
                resultText.text = "EAGLE!";
                resultText.color = new Color(0.25f, 0.82f, 0.54f); // Green color
                if (subtitleText != null) subtitleText.text = "Spectacular!";
            }
            else if (difference == -1)
            {
                resultText.text = "BIRDIE!";
                resultText.color = new Color(0.25f, 0.82f, 0.54f); // Green color #3FD18A
                if (subtitleText != null) subtitleText.text = "Great Job!";
            }
            else if (difference == 0)
            {
                resultText.text = "PAR";
                resultText.color = new Color(0.25f, 0.82f, 0.54f); // Green color
                if (subtitleText != null) subtitleText.text = "Well Done!";
            }
            else if (difference == 1)
            {
                resultText.text = "BOGEY";
                resultText.color = new Color(1f, 0.5f, 0f); // Orange
                if (subtitleText != null) subtitleText.text = "Not Bad!";
            }
            else
            {
                resultText.text = "DOUBLE BOGEY+";
                resultText.color = Color.red;
                if (subtitleText != null) subtitleText.text = "Keep Trying!";
            }
        }

        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        // Reset the goal state so it can be triggered again
        goalReached = false;

        // Hide the win panel
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Reset the GameManager stroke count
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStrokes();
        }

        // Find and reset the ball
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            // Re-enable ball shooting
            golfScript golfScript = ball.GetComponent<golfScript>();
            if (golfScript != null)
            {
                golfScript.EnableShooting();
            }

            // Reset ball visual
            SpriteRenderer ballSprite = ball.GetComponent<SpriteRenderer>();
            if (ballSprite != null)
            {
                ballSprite.enabled = true;
            }

            // Reset ball scale to original size
            if (originalBallScale != Vector3.zero)
            {
                ball.transform.localScale = originalBallScale;
            }

            // Reset ball physics
            Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }

            // Move ball back to start position
            GameObject startPoint = GameObject.FindGameObjectWithTag("BallStart");
            if (startPoint != null)
            {
                ball.transform.position = startPoint.transform.position;
            }
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogWarning("LevelManager not found! Make sure LevelManager exists in the scene.");
        }
    }
}
