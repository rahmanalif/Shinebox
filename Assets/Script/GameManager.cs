using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private int parForLevel = 3; // Override par for this level (optional, LevelManager has defaults)

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI strokeCounterText;
    [SerializeField] private TextMeshProUGUI parCounterText;

    private int currentStrokes = 0;

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
    }

    public void RecordStroke()
    {
        currentStrokes++;
        UpdateStrokeUI();
    }

    private void UpdateStrokeUI()
    {
        if (strokeCounterText != null)
        {
            strokeCounterText.text = $" {currentStrokes}";
        }
    }

    private void UpdateParUI()
    {
        if (parCounterText != null)
        {
            parCounterText.text = $" {GetPar()}";
        }
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

    public void ResetStrokes()
    {
        currentStrokes = 0;
        UpdateStrokeUI();
    }
}
