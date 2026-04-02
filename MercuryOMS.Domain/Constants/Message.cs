namespace MercuryOMS.Domain.Constants
{
    public static class Message
    {
        // Category
        public const string CategoryNotFound = "Không tìm thấy loại sản phẩm.";

        // Product
        public const string ProductNotFound = "Không tìm thấy sản phẩm.";
        public const string ProductUpdated = "Cập nhật sản phẩm thành công.";
        public const string ProductDeleted = "Xoá sản phẩm thành công.";
        public const string ProductCreated = "Tạo sản phẩm thành công.";

        // Inventory
        public const string InventoryNotFound = "Không tìm thấy tồn kho.";
        public const string InventoryReserved = "Giữ hàng thành công.";
        public const string InventoryCommitted = "Xác nhận trừ kho thành công.";
        public const string InventoryReleased = "Trả lại kho thành công.";
        public const string InventoryStockIn = "Nhập kho thành công.";
        public const string InventoryAdjusted = "Điều chỉnh kho thành công.";

        // Auth
        public const string LoginSuccess = "Đăng nhập thành công.";
        public const string RegisterSuccess = "Đăng ký thành công. Vui lòng xác nhận trong email.";

        public const string AuthEmailNotFound = "Email không tồn tại.";
        public const string AuthEmailAlreadyExists = "Email đã được sử dụng.";
        public const string AuthInvalidPassword = "Mật khẩu không đúng.";
        public const string AuthLockedOut = "Tài khoản đang bị khoá.";
        public const string AuthNotAllowed = "Vui lòng xác nhận email.";

        // Email
        public const string EmailConfirmSent = "Email xác nhận đã được gửi.";
        public const string EmailConfirmed = "Xác nhận email thành công.";
        public const string EmailAlreadyConfirmed = "Email đã được xác nhận trước đó.";
        public const string EmailConfirmInvalidToken = "Token không hợp lệ hoặc đã hết hạn.";
        public const string UserNotFound = "Không tìm thấy người dùng.";

        // Role
        public const string RoleAlreadyExists = "Role đã tồn tại.";
        public const string RoleNotFound = "Role không tồn tại.";
        public const string RoleCreated = "Tạo role thành công.";
        public const string RoleDeleted = "Xoá role thành công.";
        public const string RoleUpdated = "Cập nhật role thành công.";
    }
}