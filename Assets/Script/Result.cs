using UnityEngine;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("Play");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
