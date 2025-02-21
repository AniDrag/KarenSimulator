using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipBTN : MonoBehaviour
{
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SkipAdd);
    }
    public void SkipAdd()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
