using TMPro;
using UnityEngine;

public class HandleScore : MonoBehaviour
{
    public int score = 0;
    public int combo = 0;
    public int perfectCount = 0;
    public int goodCount = 0;
    public int missCount = 0;
    public int maxComboCount = 0;
    private int maxNoteCount = 1;
    private int perfectScore = 0;

    public TMP_Text scoreText;
    public TMP_Text comboText;
    public GameObject perfectEffectPrefab;
    public GameObject goodEffectPrefab;
    public GameObject missEffectPrefab;
    public GameObject[] lanes = new GameObject[4]; // Gán lane0 → lane3
    private Vector3[] hitLinePosition = new Vector3[4]; // Lưu vị trí hit line cho mỗi lane
    
    public void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            hitLinePosition[i] = lanes[i].transform.position + new Vector3(0, -6f, 0); // Ví dụ vị trí hit line
        }
    }
    public void SetMaxNoteCount(int count)
    {
        maxNoteCount = Mathf.Max(0, count);
        perfectScore = Mathf.RoundToInt(1000000f / maxNoteCount);
    }

    public void RegisterHit(string quality, int lane)
    {
        switch (quality)
        {
            case "Perfect":
                score += perfectScore;
                combo++;
                perfectCount++;
                ShowEffect(perfectEffectPrefab, lane);
                if (combo > maxComboCount)
                    maxComboCount = combo;
                break;
            case "Good":
                score += Mathf.RoundToInt(perfectScore * 0.33f);
                combo++;
                goodCount++;
                ShowEffect(goodEffectPrefab, lane);
                if (combo > maxComboCount)
                    maxComboCount = combo;
                break;
            case "Miss":
                combo = 0;
                missCount++;
                //Debug.Log("Missed note!, missCount = " + missCount);
                ShowEffect(missEffectPrefab, lane);
                break;
        }
        DisplayScore();
    }

    // Hiển thị hiệu ứng tại vị trí lane
    private void ShowEffect(GameObject effectPrefab, int lane)
    {
        if (effectPrefab == null || lane < 0) return;
        // Giả sử bạn có NoteSpawner hoặc lanes[] để lấy vị trí lane
        Vector3 pos = hitLinePosition[lane];
        GameObject effect = Instantiate(effectPrefab, pos, Quaternion.identity);
        Destroy(effect, 5f); // Hủy instance sau 5 giây
    }
    private void DisplayScore()
    {
        scoreText.text = $"{score}";
        comboText.text = $"{combo}";
    }

    void Update()
    {
    }
    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }
}