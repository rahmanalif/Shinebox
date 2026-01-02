using UnityEngine;

/// <summary>
/// Helper script for UI buttons to interact with LevelManager.
/// Attach this to any GameObject in the scene (like Canvas or EventSystem).
/// Then connect UI buttons to these public methods.
/// </summary>
public class UILevelButtons : MonoBehaviour
{
    /// <summary>
    /// Loads the next level. Call this from a UI Button's OnClick event.
    /// </summary>
    public void LoadNextLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogWarning("LevelManager.Instance is null! Make sure LevelManager exists in your first level scene.");
        }
    }

    /// <summary>
    /// Restarts the current level. Call this from a UI Button's OnClick event.
    /// </summary>
    public void RestartCurrentLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartCurrentLevel();
        }
        else
        {
            Debug.LogWarning("LevelManager.Instance is null! Make sure LevelManager exists in your first level scene.");
        }
    }

    /// <summary>
    /// Loads a specific level by index. Call this from a UI Button's OnClick event.
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(levelIndex);
        }
        else
        {
            Debug.LogWarning("LevelManager.Instance is null! Make sure LevelManager exists in your first level scene.");
        }
    }
}
