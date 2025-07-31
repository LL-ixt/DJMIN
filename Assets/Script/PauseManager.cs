using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas; // Kéo PauseCanvas vào đây trong Inspector
    private bool isPaused = false;

    void Start()
    {
        pauseCanvas.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        isPaused = true;
        AudioListener.pause = true; // Dừng nhạc (nếu cần)
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        isPaused = false;
        AudioListener.pause = false;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load lại scene hiện tại
    }

    public void Quit()
    {
        AudioListener.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Songlist"); // Đổi tên thành scene menu của bạn
    }
}
