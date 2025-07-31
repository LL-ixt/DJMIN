using UnityEngine;
using System.IO;

public class SongLoader : MonoBehaviour
{
    public Transform contentPanel; // Kéo GameObject Content vào đây
    public GameObject songChoicePrefab; // Kéo prefab SongChoice vào đây

    public void Start() {
        LoadSongs();
    }

    void LoadSongs() {
        foreach (var folder in Directory.GetDirectories(Application.streamingAssetsPath + "/Songlist")) {
            string infoPath = Path.Combine(folder, "info.json");
            if (File.Exists(infoPath)) {
                string json = File.ReadAllText(infoPath);
                SongData data = JsonUtility.FromJson<SongData>(json);
                GameObject choice = Instantiate(songChoicePrefab, contentPanel);
                // Load jacket.jpg
                string jacketPath = Path.Combine(folder, "jacket.jpg");
                Sprite jacketSprite = null;
                if (File.Exists(jacketPath)) {
                    byte[] fileData = File.ReadAllBytes(jacketPath);
                    Texture2D tex = new Texture2D(2, 2);
                    if (tex.LoadImage(fileData)) {
                        jacketSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    }
                }
                choice.GetComponent<SongItemUI>().SetData
                (jacketSprite, data.title, data.artist, data.difficulty[0], data.songID, folder);
            }
        }
    }
}