using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeth : MonoBehaviour
{
    public bool DeadPlayer = false;

    private void Update()
    {
        if (DeadPlayer && Input.anyKeyDown)
        {
            // save latest score
            SceneManager.LoadSceneAsync(0);
        }
    }
}
