using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // Ensures the cursor is visible when the menu loads
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Function to start the game (loads the comic scene)
    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1); // Loads the first scene (comic scene)
    }

    
    // Function to quit the application
    public void QuitApplication()
    {

        Application.Quit();//
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor NIk_ also isnt called when a build is made! use with caution.
#else
            Application.Quit(); // Quits the application in a build
#endif
    }
}