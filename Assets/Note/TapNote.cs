using UnityEngine;

public class TapNote : Note
{
    // Có thể thêm animation riêng ở đây
    public override void Init(float time, int lane)
    {
        base.Init(time, lane);
        // Nếu cần hiệu ứng hay màu riêng cho Tap, xử lý ở đây
    }

    // Update() kế thừa từ Note
    protected override void Update()
    {
        base.Update();
    }

}