using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class SelectedItemUI : MonoBehaviour
{
    public Image image;
    public TMP_Text title;
    public TMP_Text artist;
    public TMP_Text difficulty;
    public string selectedID;

    [Obsolete]
    public void DisablePreviousSeleted()
    {
        var allItems = FindObjectsOfType<SongItemUI>();
        foreach (var item in allItems)
        {
            if (item.songID == selectedID)
            {
                // Ví dụ: đổi màu, bo viền, hiệu ứng
                //item.ResetVisual();
                item.selectedEffect.gameObject.SetActive(false);
            }
        }
    }
}