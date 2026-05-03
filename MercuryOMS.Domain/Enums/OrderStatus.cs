namespace MercuryOMS.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,        // mới tạo
        Confirmed,      // shop xác nhận
        Processing,     // đang chuẩn bị hàng
        Shipping,       // đang giao
        Completed,      // giao thành công
        Cancelled,      // bị hủy
        Returned,       // bị trả hàng
    }
}
