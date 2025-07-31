using UnityEngine;
using System.Collections.Generic;
using System;
public class HandleInput : MonoBehaviour
{
    public SpriteRenderer[] lanes; // Gán lane 0 → 3 tương ứng F, G, H, J
    private KeyCode[] keys = { KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J };

    private float normalAlpha = 27f / 255f;
    private float pressedAlpha = 100f / 255f;
    public NoteSpawner noteSpawner;
    //[SerializeField] float offset = 0f;
    void Update()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            Queue<GameObject> queue = noteSpawner.laneQueues[i];
            if (queue.Count > 0)
            {
                GameObject note = queue.Peek();
                if (note == null) { queue.Dequeue(); continue; }
                TapNote tap = note.GetComponent<TapNote>();
                HoldNote hold = note.GetComponent<HoldNote>();
                if (tap != null)
                {

                    if (noteSpawner.musicSource.time * 1000f > tap.startTime + 400f) // Too late, miss
                    {
                        noteSpawner.handleScore.RegisterHit("Miss", i);
                        queue.Dequeue();
                    }
                }
                else if (hold != null)
                {
                    if (hold.startTime + 400f < noteSpawner.musicSource.time * 1000f && !hold.isPressed) // Too late, miss
                    {
                        hold.isMissed = 1;
                        hold.HandleMissEffect();
                        noteSpawner.handleScore.RegisterHit("Miss", i);
                        queue.Dequeue();
                    }
                    else if (hold.tail.position.y < -4f)
                    {
                        if (hold.isMissed == 0)
                        {
                            if (hold.headScore == 2)
                            {
                                noteSpawner.handleScore.RegisterHit("Perfect", i);
                                Destroy(hold.gameObject);
                            }
                            else if (hold.headScore == 1)
                            {
                                noteSpawner.handleScore.RegisterHit("Good", i);
                                Destroy(hold.gameObject);
                            }
                            else
                            {
                                hold.isMissed = 1; // Đánh dấu là missed
                                noteSpawner.handleScore.RegisterHit("Miss", i);
                            }
                        }
                    }
                }
                else queue.Dequeue();
            }
            if (Input.GetKeyDown(keys[i]))
            {
                SetLaneAlpha(i, pressedAlpha);
                if (queue.Count > 0)
                {
                    GameObject note = queue.Peek();
                    if (note == null) { queue.Dequeue(); continue; }
                    TapNote tap = note.GetComponent<TapNote>();
                    HoldNote hold = note.GetComponent<HoldNote>();
                    if (tap != null)
                    {
                        if (tap.startTime - 400f < noteSpawner.musicSource.time * 1000f)
                        {
                            // Nếu tap note NẰM GẦN line thì mới xử lý
                            HandleTap(tap, i);
                            queue.Dequeue();
                            break;
                        }

                    }
                    else if (hold != null)
                    {
                        if (hold.startTime - 400f < noteSpawner.musicSource.time * 1000f && !hold.isPressed)
                        {
                            hold.headScore = HandleHold(hold);
                            //Debug.Log($"Hold note detected on lane {i}, head score: {headScores[i]}");
                            break;
                        }
                    }
                    else queue.Dequeue();
                }
            }

            if (Input.GetKeyUp(keys[i]))
            {
                SetLaneAlpha(i, normalAlpha);
                if (queue.Count > 0)
                {
                    GameObject note = queue.Peek();
                    HoldNote hold = note != null ? note.GetComponent<HoldNote>() : null;
                    if (hold != null && hold.isPressed)
                    {
                        HandleHoldRelease(hold, queue, i);
                    }
                }
            }
        }
    }

    void SetLaneAlpha(int index, float alpha)
    {
        if (index < 0 || index >= lanes.Length) return;

        SpriteRenderer sr = lanes[index];
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    void HandleTap(TapNote tapNote, int i)
    {
        if (tapNote == null) return;

        float hitTime = tapNote.startTime;
        float songTime = noteSpawner.musicSource.time * 1000f; // đổi sang milliseconds
        float delta = Mathf.Abs(songTime - hitTime);

        if (delta < 200f)
        {
            noteSpawner.handleScore.RegisterHit("Perfect", i);
        }
        else if (delta < 400f)
        {
            noteSpawner.handleScore.RegisterHit("Good", i);
        }
        else
        {
            noteSpawner.handleScore.RegisterHit("Miss", i);
        }
        Destroy(tapNote.gameObject);
    }

    int HandleHold(HoldNote holdNote)
    {
        holdNote.isPressed = true;
        if (holdNote == null) return -1;
        // Khi nhấn giữ phím: hiển thị hold_body_active, ẩn hold_body_idle
        if (holdNote.body != null && holdNote.body.gameObject != null)
            holdNote.body.gameObject.SetActive(false); // ẩn idle
        if (holdNote.bodyActive != null)
            holdNote.bodyActive.gameObject.SetActive(true); // hiện active

        // Tính điểm headScore dựa trên độ lệch head với hitline khi nhấn phím
        float songTime = noteSpawner.musicSource.time * 1000f;
        float delta = Mathf.Abs(songTime - holdNote.startTime);
       //Debug.Log($"Hold note pressed: delta={delta}, startTime={holdNote.startTime}, songTime={songTime}");
        if (delta < 200f)
            return 2; // Perfect
        else if (delta < 400f)
            return 1; // Good
        else
            Debug.LogWarning($"Hold note pressed too late: delta={delta}, startTime={holdNote.startTime}, songTime={songTime}");
            return 0; // Miss
    }

    void HandleHoldRelease(HoldNote holdNote, Queue<GameObject> queue, int i)
    {
        float songTime = noteSpawner.musicSource.time * 1000f;
        bool enough = songTime >= holdNote.endTime - 200f;
        //Xử lí khi thả phím trước thời điểm kết thúc hold note
        if (enough)
        {
            int headScore = holdNote.headScore;
            if (headScore == 2)
            {
                noteSpawner.handleScore.RegisterHit("Perfect", i);
                Destroy(holdNote.gameObject);
            }
            else if (headScore == 1)
            {
                noteSpawner.handleScore.RegisterHit("Good", i);
                Destroy(holdNote.gameObject);
            }
            else
            {
                holdNote.isMissed = 1; // Đánh dấu là missed
                noteSpawner.handleScore.RegisterHit("Miss", i);
                holdNote.HandleMissEffect();
            }

        }
        else
        {
            Debug.Log($"Hold note released too early on lane {i}, head score: {holdNote.headScore}");
            holdNote.isMissed = 1; // Đánh dấu là missed
            noteSpawner.handleScore.RegisterHit("Miss", i);
            holdNote.HandleMissEffect();
        }
        holdNote.headScore = 3;
        queue.Dequeue();
    }
}