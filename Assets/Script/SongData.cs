[System.Serializable]
public class SongData
{
    public string title;
    public string artist;
    public int bpm;
    public int[] difficulty;
    public string folderPath; // để lưu đường dẫn thư mục
    public string songID; // Đề phòng tên trùng
}