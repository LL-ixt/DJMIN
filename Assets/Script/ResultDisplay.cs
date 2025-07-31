using UnityEngine;
using TMPro;
public class ResultDisplay : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text perfectText;
    public TMP_Text goodText;
    public TMP_Text missText;

    [System.Obsolete]
    void Start()
    {
        HandleScore scoreData = FindObjectOfType<HandleScore>();
        if (scoreData != null)
        {
            scoreText.text = $"Score: {scoreData.score}";
            comboText.text = $"Max Combo: {scoreData.maxComboCount}";
            perfectText.text = $"Perfect: {scoreData.perfectCount}";
            goodText.text = $"Good: {scoreData.goodCount}";
            missText.text = $"Miss: {scoreData.missCount}";
            Destroy(scoreData.gameObject);
        }
        else
        {
            Debug.LogError("Không tìm thấy HandleScore!");
        }
    }
}