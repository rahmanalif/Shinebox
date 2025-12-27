using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private int parForLevel = 3;
    [SerializeField] private string nextLevelName = "";

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI strokeCounterText;

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
            strokeCounterText.text = $"Strokes: {currentStrokes}";
        }
    }

    public int GetCurrentStrokes()
    {
        return currentStrokes;
    }

    public int GetPar()
    {
        return parForLevel;
    }

    public string GetNextLevelName()
    {
        return nextLevelName;
    }
}
