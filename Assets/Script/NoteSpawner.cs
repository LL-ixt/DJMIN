using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
public class NoteSpawner : MonoBehaviour 
{

    private float speed = 5f;
    private NoteConfig noteConfig;
    public GameObject[] lanes; // Gán lane0 → lane3
    public GameObject tapPrefab;
    public GameObject holdPrefab;
    public AudioSource musicSource;

    private List<NoteData> notes = new List<NoteData>();

    public Queue<GameObject>[] laneQueues = new Queue<GameObject>[4];

    private float audioOffset = 0f;
    private int nextNoteIndex = 0;

    public int maxNoteCount = 0;

    public HandleScore handleScore; // Kéo HandleScore vào đây trong Inspector

    [Obsolete]
    void Awake()
    {
        string folder = SelectedSongFolder.folderPath;
        if (string.IsNullOrEmpty(folder))
        {
            Debug.LogError("Selected song folder path is empty.");
            return;
        }
        if (musicSource == null)
        {
            Debug.LogError("Lỗi ở đây");
        }
        Debug.Log("Selected song folder: " + folder);
        string notePath = Path.Combine(folder, "2.txt");
        string noteText = File.ReadAllText(notePath);
        ParseNoteFile(noteText);
        
        string audioPath = Path.Combine(folder, "audio.mp3");
        StartCoroutine(LoadAudio(audioPath));
    }
    IEnumerator LoadAudio(string audioPath)
    {
        Debug.Log("Loading audio from: " + audioPath);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load audio: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                musicSource.clip = clip;
                musicSource.loop = false; // Không lặp lại
            }
        }
        // Load NoteConfig
        noteConfig = Resources.Load<NoteConfig>("NoteConfig");
        if (noteConfig != null)
            speed = noteConfig.noteSpeed;
        for (int i = 0; i < 4; i++)
            laneQueues[i] = new Queue<GameObject>();
        
        if (handleScore != null)
        {
            handleScore.SetMaxNoteCount(maxNoteCount);
        }
        Invoke(nameof(StartMusic), 0);
    }
    void StartMusic()
    {
        musicSource.Play();
    }

    void Update()
    {
        if (!musicSource.isPlaying || musicSource.time <= 0.01f) return;
        if (nextNoteIndex >= notes.Count)
        {
            Debug.Log("musicSource.time: " + musicSource.time + ", clip length: " + musicSource.clip.length);
            // Kiểm tra nếu nhạc đã chạy xong thì chuyển scene
            if (!musicSource || musicSource.time >= musicSource.clip.length - 0.05f)
            {
                Debug.Log("All notes spawned, loading result screen.");
                LoadResultScreen();
            }
            return;
        }

        float songTime = musicSource.time * 1000f; // đổi sang milliseconds

        while (nextNoteIndex < notes.Count && notes[nextNoteIndex].startTime <= songTime + 1000f)
        {
            SpawnNote(notes[nextNoteIndex], musicSource.time);
            nextNoteIndex++;
        }
    }

    void SpawnNote(NoteData data, float songTime)
    {
        Transform laneTransform = lanes[data.lane].transform;
        float actualStartTime = data.startTime/1000f; // áp dụng offset
        Vector3 spawnPos = laneTransform.position + new Vector3(0, Math.Abs(actualStartTime-songTime)*speed-4f, 0); // spawn từ trên

        GameObject noteObj = null;
        if (data.isHold)
        {
            noteObj = Instantiate(holdPrefab, spawnPos, Quaternion.identity);
            HoldNote holdNote = noteObj.GetComponent<HoldNote>();
            if (holdNote != null)
            {
                holdNote.Init(data.startTime, data.endTime, data.lane);
            }
        }
        else
        {
            noteObj = Instantiate(tapPrefab, spawnPos, Quaternion.identity);
            TapNote tapNote = noteObj.GetComponent<TapNote>();
            if (tapNote != null)
            {
                tapNote.Init(data.startTime, data.lane);
            }
        }
        laneQueues[data.lane].Enqueue(noteObj); // Thêm note vào queue
    }

    void ParseNoteFile(string noteText)
    {
        using StringReader reader = new(noteText);
        string line;
        int noteCount = 0;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("timing"))
                continue;
            if (line.StartsWith("AudioOffset:"))
            {
                audioOffset = float.Parse(line.Replace("AudioOffset:", "").Trim());
            }
            else if (line.StartsWith("hold"))
            {
                // hold(start, end, lane);
                string trimmed = line.Replace("hold(", "").Replace(");", "");
                string[] parts = trimmed.Split(',');
                float start = float.Parse(parts[0]);
                float end = float.Parse(parts[1]);
                int lane = int.Parse(parts[2]);
                NoteData hold = new(start + audioOffset, lane - 1, true, end + audioOffset); // lane - 1 vì lane 1-4 trong file, nhưng 0-3 trong code
                notes.Add(hold);
                noteCount++;
            }
            else if (line.StartsWith("Tap") || line.StartsWith("("))
            {
                // TapNote: Tap(time, lane); hoặc (time, lane);
                string trimmed = line.Replace("Tap(", "").Replace("(", "").Replace(");", "");
                string[] parts = trimmed.Split(',');
                float time = float.Parse(parts[0]);
                int lane = int.Parse(parts[1]);
                NoteData tap = new(time + audioOffset, lane - 1);
                notes.Add(tap);
                noteCount++;
            }
        }
        maxNoteCount = noteCount;
    }

    void LoadResultScreen()
    {
        DontDestroyOnLoad(gameObject); // Giữ lại HandleScore
        SceneManager.LoadScene("Result");
        Debug.Log("All notes spawned, loading result screen.");
    }
}