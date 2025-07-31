using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    public float targetTime; // thời gian đến vị trí hit line
    public int lane; // lane của note
    public float speed = 5f; // tốc độ ban đầu (hoặc để tự tính từ targetTime)

    private Vector3 targetPos;
    private float spawnTime;

    public void Init(float t, int l)
    {
        targetTime = t;
        lane = l; // Lưu lane nếu cần
        targetPos = new Vector3(transform.position.x, -3.5f, 0); // ví dụ vị trí hitline
    }

    void Update()
    {
        // Tự di chuyển xuống theo thời gian thực
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (transform.position.y <= targetPos.y)
        {
            Destroy(gameObject);
        }
    }
}
