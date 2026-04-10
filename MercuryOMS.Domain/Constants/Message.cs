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
        public const string ProductGetSuccess = "Lấy danh sách sản phẩm thành công.";
        public const string ProductGetFromCache = "Lấy danh sách sản phẩm từ cache.";
        public const string ProductFilterInvalid = "Filter không hợp lệ.";
        public const string ProductGetFailed = "Có lỗi xảy ra khi lấy danh sách sản phẩm.";

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

        public const string ExternalLoginProviderNotFound = "Không tìm thấy thông tin đăng nhập từ nhà cung cấp.";
        public const string ExternalEmailNotFound = "Không tìm thấy email từ nhà cung cấp đăng nhập.";

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

        // Cart
        public const string CartUserIdRequired = "UserId không được rỗng.";
        public const string CartProductIdRequired = "ProductId không được rỗng.";
        public const string CartQuantityGreaterThanZero = "Số lượng phải lớn hơn 0.";
        public const string CartQuantityMaxExceeded = "Số lượng không được vượt quá 100.";
        public const string CartItemNotFound = "Không tìm thấy sản phẩm trong giỏ.";

        public const string CartAddSuccessfully = "Thêm sản phẩm vào giỏ thành công.";
        public const string CartRemoveSuccessfully = "Đã xoá sản phẩm khỏi giỏ";

        // Order
        public const string OrderUserInvalid = "Người dùng không hợp lệ.";
        public const string OrderEmpty = "Đơn hàng phải có ít nhất một sản phẩm.";
        public const string OrderCreated = "Tạo đơn hàng thành công.";
        public const string OrderProductNotFound = "Một số sản phẩm không tồn tại.";
        public const string OrderQuantityInvalid = "Số lượng phải lớn hơn 0.";
        public const string OrderProductInactive = "Sản phẩm không hoạt động.";
        public const string OrderCreateFailed = "Tạo đơn hàng thất bại.";
        public const string OrderNotFound = "Không tìm thấy đơn hàng.";

        // Payment
        public const string PaymentCreated = "Tạo thanh toán thành công.";
        public const string PaymentFailed = "Thanh toán thất bại.";
        public const string PaymentNotFound = "Không tìm thấy thông tin thanh toán.";
        public const string PaymentInvalidAmount = "Số tiền thanh toán không hợp lệ.";
        public const string PaymentMethodNotSupported = "Phương thức thanh toán không được hỗ trợ.";
        public const string PaymentAlreadyProcessed = "Thanh toán đã được xử lý trước đó.";
        public const string PaymentProcessingError = "Có lỗi xảy ra trong quá trình xử lý thanh toán.";

        // Payment Event
        public const string PaymentPaidSuccess = "Thanh toán thành công.";
        public const string PaymentPaidPublishFailed = "Không thể gửi sự kiện thanh toán.";

        // Pagination & Filtering
        public const string PageIndexInvalid = "PageIndex phải lớn hơn hoặc bằng 1.";
        public const string PageSizeInvalid = "PageSize phải nằm trong khoảng từ 1 đến 100.";
        public const string MinPriceInvalid = "MinPrice phải lớn hơn hoặc bằng 0.";
        public const string MaxPriceInvalid = "MaxPrice phải lớn hơn hoặc bằng 0.";
        public const string MinPriceGreaterThanMaxPrice = "MinPrice không được lớn hơn MaxPrice.";

    }
}