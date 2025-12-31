using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI resultText;

    [Header("Hole Animation Settings")]
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private float shrinkScale = 0.1f;
    [SerializeField] private float pullForce = 5f;
    [SerializeField] private AudioClip holeSound;
    [SerializeField] private ParticleSystem holeParticleEffect;

    private bool goalReached = false;
    private AudioSource audioSource;

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

        // Update score text
        if (scoreText != null)
        {
            scoreText.text = $"{strokes}\nPar: {par}";
        }

        // Update result text based on performance
        if (resultText != null)
        {
            if (strokes == 1)
            {
                resultText.text = "HOLE IN ONE!";
                resultText.color = Color.yellow;
            }
            else if (difference <= -2)
            {
                resultText.text = "EAGLE!";
                resultText.color = Color.yellow;
            }
            else if (difference == -1)
            {
                resultText.text = "BIRDIE!";
                resultText.color = Color.green;
            }
            else if (difference == 0)
            {
                resultText.text = "PAR";
                resultText.color = Color.white;
            }
            else if (difference == 1)
            {
                resultText.text = "BOGEY";
                resultText.color = new Color(1f, 0.5f, 0f);
            }
            else
            {
                resultText.text = "DOUBLE BOGEY+";
                resultText.color = Color.red;
            }
        }

        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        string nextLevel = GameManager.Instance?.GetNextLevelName();

        if (!string.IsNullOrEmpty(nextLevel))
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            Debug.Log("No next level set!");
        }
    }
}
