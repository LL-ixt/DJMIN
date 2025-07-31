using UnityEngine;

public class HoldNote : Note
{
    public Transform head;
    public Transform body; // body_idle
    public Transform tail;
    public Transform bodyActive;

    public float endTime;
    public bool isPressed = false;
    public int isMissed = 0;
    public int headScore = -1;
    public void Init(float start, float end, int lane)
    {
        base.Init(start, lane);
        endTime = end;

        float duration = end - start;
        float bodyLength = duration / 1000f * speed;
        float imgLength = 1f;
        var sr = body.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            imgLength = sr.sprite.bounds.size.y;
        }
        float scaleY = bodyLength / imgLength;
        body.localScale = new Vector3(1, scaleY, 1);
        if (body != null)
        {
            body.localScale = new Vector3(1, scaleY, 1);
            body.localPosition = new Vector3(0, bodyLength / 2f, 0); // body_idle nằm dưới head
            body.gameObject.SetActive(true);
        }
        if (bodyActive != null)
        {
            bodyActive.localScale = new Vector3(1, scaleY, 1);
            bodyActive.localPosition = new Vector3(0, bodyLength / 2f, 0); // body_active nằm dưới head
            bodyActive.gameObject.SetActive(false);
        }
        if (tail != null)
        {
            tail.localPosition = new Vector3(0, bodyLength, 0); // tail ở cuối thân
            tail.gameObject.SetActive(true);
        }
        if (head != null)
        {
            head.localPosition = Vector3.zero;
            head.gameObject.SetActive(true);
        }
    }

    protected override void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        // Kiểm tra vị trí tail, chỉ destroy khi tail đi qua hitline (y = -4f)
        float headWorldY = head.position.y;
        float tailWorldY = tail.position.y;
        if (headWorldY < -5f)
            if (tailWorldY < -5f)
            {
                var scoreHandler = FindAnyObjectByType<HandleScore>();
                if (scoreHandler != null)
                {
                    if (isMissed == 0)
                    {
                        scoreHandler.RegisterHit("Miss", lane); //for hold note with short duration
                    }
                }
                Destroy(gameObject);
            }
    }
    public void HandleMissEffect()
    {
        if (body != null) body.gameObject.SetActive(true);
        if (bodyActive != null) bodyActive.gameObject.SetActive(false);
        var sr = body.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color color = sr.color;
            color.a = 0.5f; // Giảm độ trong suốt
            sr.color = color;
        }
    }
}