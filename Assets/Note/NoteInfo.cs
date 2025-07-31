public class NoteInfo
{
    public float startTime;
    public float endTime;
    public int lane;

    public bool IsHold => endTime > startTime;

    public NoteInfo(float time, int lane)
    {
        this.startTime = time;
        this.endTime = time; // Tap note
        this.lane = lane;
    }

    public NoteInfo(float start, float end, int lane)
    {
        this.startTime = start;
        this.endTime = end;
        this.lane = lane;
    }
}