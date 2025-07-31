public class NoteData
{
    public float startTime;
    public int lane;
    public bool isHold;
    public float endTime;

    public NoteData(float startTime, int lane, bool isHold = false, float endTime = 0)
    {
        this.startTime = startTime;
        this.lane = lane;
        this.isHold = isHold;
        this.endTime = endTime;
    }
}
