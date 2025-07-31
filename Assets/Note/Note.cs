using UnityEngine;
public abstract class Note : MonoBehaviour
{
    public static NoteConfig config;
    public float startTime;
    public int lane;
    public float speed = 5f;
    protected Vector3 targetPos;

    public virtual void Init(float time, int lane)
    {
        if (config == null)
            config = Resources.Load<NoteConfig>("NoteConfig"); // hoặc gán từ NoteSpawner
        speed = config.noteSpeed;
        this.startTime = time;
        this.lane = lane;
        this.targetPos = new Vector3(transform.position.x, -100f, 0); // HitLine Y
    }

    protected virtual void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            var scoreHandler = FindAnyObjectByType<HandleScore>();
            if (scoreHandler != null)
            {
                scoreHandler.RegisterHit("Miss", lane);
            }
            Destroy(gameObject);
        }
    }
}