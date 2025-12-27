using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class SceneButton
{
    [Tooltip("The button that will trigger the scene change")]
    public Button button;

    [Tooltip("Enter the exact name of the scene you want to load")]
    public string sceneToLoad;
}

public class SceneChanger : MonoBehaviour
{
    [Header("Multiple Scene Buttons")]
    [Tooltip("Add multiple buttons here, each with their own scene to load")]
    public SceneButton[] sceneButtons;

    void Start()
    {
        // Set up listeners for all buttons
        if (sceneButtons != null && sceneButtons.Length > 0)
        {
            for (int i = 0; i < sceneButtons.Length; i++)
            {
                int index = i; // Capture index for closure
                SceneButton sceneButton = sceneButtons[index];

                if (sceneButton.button != null)
                {
                    sceneButton.button.onClick.AddListener(() => ChangeScene(sceneButton.sceneToLoad));
                }
                else
                {
                    Debug.LogWarning($"SceneChanger: Button at index {index} is not assigned!");
                }
            }
        }
        else
        {
            Debug.LogError("SceneChanger: No scene buttons assigned in the Inspector!");
        }
    }

    // Load scene by name
    public void ChangeScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("SceneChanger: Scene name is empty!");
        }
    }

    // Alternative: Load scene by build index
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Reload current scene
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quit application (useful for main menu)
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
