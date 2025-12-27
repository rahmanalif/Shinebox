using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI resultText;

    private bool goalReached = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball") && !goalReached)
        {
            goalReached = true;
            ShowWinScreen();
        }
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
            scoreText.text = $"Strokes: {strokes}\nPar: {par}";
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
