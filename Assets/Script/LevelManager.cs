using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Progression - Add your level scenes here")]
    [SerializeField] private string[] levelSceneNames = new string[]
    {
        "Phutt Phutt Golf",  // Level 1
        "Level_02",          // Level 2
        "Level_03",          // Level 3
        "Level_04",          // Level 4
        // Add more levels as you create them
    };

    [SerializeField] private int[] parForEachLevel = new int[]
    {
        2,  // Level 1 par
        3,  // Level 2 par
        3,  // Level 3 par
        3,  // Level 4 par
        // Add more par values as you create levels
    };

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update currentLevelIndex to match the loaded scene
        for (int i = 0; i < levelSceneNames.Length; i++)
        {
            if (levelSceneNames[i] == scene.name)
            {
                currentLevelIndex = i;
                Debug.Log($"Loaded level {i + 1}: {scene.name}");
                break;
            }
        }
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
