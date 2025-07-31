using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SongItemUI : MonoBehaviour, IPointerClickHandler // Thêm interface
{
    public Image jacketImage;
    public TMP_Text titleText;
    public TMP_Text difficultyText;
    private string artist; // Biến để lưu thông tin artist, nếu cần
    public string songID;
    public Image selectedEffect;
    public bool isSelected = false;
    public string songFolderPath;

    public void SetData(Sprite jacket, string title, string artist, int difficulty, string songID, string folderPath)
    {
        jacketImage.sprite = jacket;
        titleText.text = title;
        difficultyText.text = difficulty.ToString();
        this.artist = artist; // Lưu thông tin artist
        this.songID = songID; // Lưu songID
        this.songFolderPath = folderPath; // Lưu đường dẫn thư mục
        //selectedEffect.gameObject.SetActive(false); // Ẩn hiệu ứng chọn ban đầu
    }

    [System.Obsolete]
    public void OnPointerClick(PointerEventData eventData) // Đổi thành public
    {
        var selected = FindObjectOfType<SelectedItemUI>();
        if (selected != null)
        {
            if (selected.selectedID == songID)
            {
                Debug.Log("Switching to Play scene");
                var SelectedSongFolder = FindObjectOfType<SelectedSongFolder>();
                if (SelectedSongFolder != null)
                {
                    //Debug.Log("songFolderPath: " + songFolderPath);
                    SelectedSongFolder.folderPath = songFolderPath;
                    SceneManager.LoadScene("Play"); // Cập nhật đường dẫn thư mục đã chọn
                }
                else
                {
                    Debug.LogError("SelectedSongFolder not found!");
                }
            }
            else
            {
                selectedEffect.gameObject.SetActive(true);
                selected.DisablePreviousSeleted();
                selected.image.sprite = jacketImage.sprite;
                selected.title.text = titleText.text;
                selected.artist.text = artist;
                selected.difficulty.text = difficultyText.text;
                selected.selectedID = songID; // Cập nhật selectedID
            }
        }
    }
}