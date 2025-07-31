using UnityEngine;

public class SelectedSongFolder : MonoBehaviour
{
    public static string folderPath;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
