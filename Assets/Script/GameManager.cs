using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private int parForLevel = 3; // Override par for this level (optional, LevelManager has defaults)
    [SerializeField] private int maxStrokesForLevel = 0; // Override max strokes for this level (0 = no limit)

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI strokeCounterText;
    [SerializeField] private TextMeshProUGUI parCounterText;
    [SerializeField] private TextMeshProUGUI maxStrokesText;
    [SerializeField] private TextMeshProUGUI remainingStrokesText;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI loseReasonText;

    private int currentStrokes = 0;
    private bool outOfStrokes = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateStrokeUI();
        UpdateParUI();
        UpdateMaxStrokesUI();
    }

    public void RecordStroke()
    {
        currentStrokes++;
        UpdateStrokeUI();

        int maxStrokes = GetMaxStrokes();
        if (maxStrokes > 0 && currentStrokes > maxStrokes && !outOfStrokes)
        {
            HandleOutOfStrokes();
        }
    }

    private void UpdateStrokeUI()
    {
        if (strokeCounterText != null)
        {
            strokeCounterText.text = $" {currentStrokes}";
        }

        UpdateRemainingStrokesUI();
    }

    private void UpdateParUI()
    {
        if (parCounterText != null)
        {
            parCounterText.text = $" {GetPar()}";
        }
    }

    private void UpdateMaxStrokesUI()
    {
        if (maxStrokesText != null)
        {
            int maxStrokes = GetMaxStrokes();
            maxStrokesText.text = maxStrokes > 0 ? $" {maxStrokes}" : " -";
        }
        UpdateRemainingStrokesUI();
    }

    private void UpdateRemainingStrokesUI()
    {
        if (remainingStrokesText == null)
        {
            return;
        }

        int maxStrokes = GetMaxStrokes();
        if (maxStrokes <= 0)
        {
            remainingStrokesText.text = " -";
            return;
        }

        int remaining = Mathf.Max(0, maxStrokes - currentStrokes);
        remainingStrokesText.text = $" {remaining}";
    }

    public int GetCurrentStrokes()
    {
        return currentStrokes;
    }

    public int GetPar()
    {
        // If LevelManager exists, use its par value, otherwise use the scene-specific override
        if (LevelManager.Instance != null)
        {
            int levelIndex = LevelManager.Instance.GetCurrentLevelIndex();
            return LevelManager.Instance.GetParForLevel(levelIndex);
        }
        return parForLevel;
    }

    public int GetMaxStrokes()
    {
        // If LevelManager exists, use its max strokes value, otherwise use the scene-specific override
        if (LevelManager.Instance != null)
        {
            int levelIndex = LevelManager.Instance.GetCurrentLevelIndex();
            return LevelManager.Instance.GetMaxStrokesForLevel(levelIndex);
        }
        return maxStrokesForLevel;
    }

    public bool IsOutOfStrokes()
    {
        return outOfStrokes;
    }

    private void HandleOutOfStrokes()
    {
        outOfStrokes = true;

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        if (loseReasonText != null)
        {
            loseReasonText.text = "Out of strokes";
        }

        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            golfScript golfScript = ball.GetComponent<golfScript>();
            if (golfScript != null)
            {
                golfScript.DisableShooting();
            }
        }
    }

    public void TryAgain()
    {
        outOfStrokes = false;

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        ResetStrokes();

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartCurrentLevel();
        }
    }

    public void ResetStrokes()
    {
        currentStrokes = 0;
        outOfStrokes = false;
        UpdateStrokeUI();
        UpdateMaxStrokesUI();

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }
}
