using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Progression")]
    [SerializeField] private string[] levelSceneNames;
    [SerializeField] private int[] parForEachLevel;

    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelSceneNames.Length)
        {
            currentLevelIndex = levelIndex;
            SceneManager.LoadScene(levelSceneNames[levelIndex]);
        }
        else
        {
            Debug.LogWarning($"Level index {levelIndex} out of range!");
        }
    }

    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < levelSceneNames.Length)
        {
            LoadLevel(nextIndex);
        }
        else
        {
            Debug.Log("All levels completed!");
            LoadLevel(0); // Loop back to first level
        }
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public int GetTotalLevels()
    {
        return levelSceneNames.Length;
    }

    public string GetCurrentLevelName()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelSceneNames.Length)
        {
            return levelSceneNames[currentLevelIndex];
        }
        return "";
    }

    public int GetParForLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < parForEachLevel.Length)
        {
            return parForEachLevel[levelIndex];
        }
        return 3; // Default par
    }
}
